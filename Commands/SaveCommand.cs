using Cliffer;
using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("save", "Save a program to persistent storage")]
[Argument(typeof(string), "filename", "The name of the file to write to disk", arity: ArgumentArity.ExactlyOne)]
internal class SaveCommand {
    public int Execute(
        string filename, 
        SyntaxParser syntaxParser, 
        VariableStore variableStore, 
        ProgramService programService
        ) 
    {
        var element = syntaxParser.ParseArgs(filename);

        try {
            if (element is BasicExpression expression) {
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

