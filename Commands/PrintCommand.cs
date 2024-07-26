using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("print", "Print text to the screen")]
[Argument(typeof(IEnumerable<string>), "args", "The text to print or expression to evaluate", Cliffer.ArgumentArity.ZeroOrMore)]
internal class PrintCommand : BasicCommand {
    public int Execute(
        IEnumerable<string> args,
        ExpressionBuilder expressionBuilder,
        VariableStore variableStore
        ) 
    {
        if (!args.Any()) {
            Console.WriteLine();
            return Result.Success;
        }

        var expression = expressionBuilder.BuildExpression(args);

        if (expression is not null) {
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

        Console.Error.WriteLine("Invalid expression");
        return Result.Error;
    }
}

