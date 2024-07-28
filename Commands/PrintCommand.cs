using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("print", "Print text to the screen")]
[Argument(typeof(IEnumerable<string>), "args", "The text to print or element to evaluate", Cliffer.ArgumentArity.ZeroOrMore)]
internal class PrintCommand : BasicCommand {
    public int Execute(
        IEnumerable<string> args,
        SyntaxParser syntaxParser,
        VariableStore variableStore
        ) 
    {
        if (!args.Any()) {
            Console.WriteLine();
            return Result.Success;
        }

        var element = syntaxParser.ParseArgs(args);

        if (element is BasicExpression expression) {
            try {
                var result = expression.Evaluate(variableStore);
                Console.WriteLine(result);
                return Result.Success;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return Result.Error;
            }
        }

        Console.Error.WriteLine("Invalid element");
        return Result.Error;
    }
}

