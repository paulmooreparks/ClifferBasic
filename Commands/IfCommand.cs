using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("if", "Take an action conditionally based on a Boolean evaluation")]
[Argument(typeof(IEnumerable<string>), "args", "The condition and action to take", Arity = Cliffer.ArgumentArity.OneOrMore)]
internal class IfCommand {
    public async Task<int> Execute(
        IEnumerable<string> args,
        InvocationContext context,
        ExpressionBuilder expressionBuilder,
        VariableStore variableStore
        ) 
    {
        var expression = expressionBuilder.BuildExpression(args);

        if (expression is not null) {
            var testResult = expression.Evaluate(variableStore);

            if (Convert.ToBoolean(testResult)) {
                expression = expressionBuilder.BuildExpression();

                if (expression is ThenExpression) {
                    expression = expressionBuilder.BuildExpression();

                    if (expression is CommandExpression commandExpression) {
                        var parseResult = context.Parser.Parse(commandExpression.Args);

                        if (parseResult != null) {
                            var commandName = parseResult.CommandResult.Command.Name;
                            await parseResult.CommandResult.Command.InvokeAsync(commandExpression.Args);
                        }
                    }
                }
                else {
                    Console.Error.WriteLine("Invalid if statement");
                    return Result.Error;
                }
            }
        }

        return Result.Success; 
    }
}
