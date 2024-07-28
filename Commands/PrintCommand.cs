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

        while (element is not null) {
            if (element is BasicExpression expression) {
                try {
                    var result = expression.Evaluate(variableStore);
                    Console.Write(result);
                    element = syntaxParser.Continue();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    return Result.Error;
                }
            }

            if (element is LineConcatOperator) {
                element = syntaxParser.Continue();
            }
            else {
                Console.WriteLine();
            }
        }

        return Result.Success;
    }
}

