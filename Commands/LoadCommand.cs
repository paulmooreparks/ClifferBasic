using Cliffer;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("load", "Load a program from persistent storage")]
[Argument(typeof(string), "filename", "The name of the file to load into memory", arity: ArgumentArity.ExactlyOne)]
internal class LoadCommand {
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
                        var newProgram = programService.Load(file);

                        if (newProgram is not null) {
                            return Result.Success;
                        }
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
