using Cliffer;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("save", "Save a program to persistent storage")]
[Argument(typeof(string), "filename", "The name of the file to write to disk", arity: ArgumentArity.ExactlyOne)]
internal class SaveCommand {
    public int Execute(
        string filename, 
        ExpressionBuilder expressionBuilder, 
        VariableStore variableStore, 
        ProgramService programService
        ) 
    {
        var expression = expressionBuilder.BuildExpression(filename);

        try {
            if (expression is not null) {
                var eval = expression.Evaluate(variableStore);

                if (eval is not null) {
                    var file = eval.ToString();

                    if (!string.IsNullOrEmpty(file)) {
                        programService.Save(file);
                        return Result.Success;
                    }
                }
            }

            Console.WriteLine($"Invalid filename {filename}");
            return Result.Error;
        }
        catch (Exception) {
            Console.WriteLine($"Invalid filename {filename}");
            return Result.Error;
        }
    }
}

