using System.Linq.Expressions;

using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("chgdir", "Change the current directory")]
[Argument(typeof(string), "directory", "The directory to change to")]
internal class ChgDirCommand {
    public int Execute(
        string directory,
        SyntaxParser syntaxParser,
        VariableStore variableStore,
        ProgramService programService
        ) {

        var element = syntaxParser.ParseArg(directory);

        try {
            if (element is BasicExpression expression) {
                var eval = expression.Evaluate(variableStore);

                if (eval is not null) {
                    var directoryName = eval.ToString();

                    if (!string.IsNullOrEmpty(directoryName)) {
                        if (Directory.Exists(directoryName)) {
                            Directory.SetCurrentDirectory(directoryName);
                            return Result.Success;
                        }
                        else {
                            Console.Error.WriteLine($"Error: Directory '{directoryName}' does not exist.");
                            return Result.Error;
                        }
                    }
                }
            }
        }
        catch (Exception) {
            // Optional: Route this to debug logging if necessary
        }

        Console.Error.WriteLine($"Error: Invalid directory '{directory}'.");
        return Result.Error;
    }
}
