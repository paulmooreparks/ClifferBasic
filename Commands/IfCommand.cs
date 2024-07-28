using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Reflection.Metadata;

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
        SyntaxParser syntaxParser,
        VariableStore variableStore, 
        ProgramService programService
        ) 
    {
        var element = syntaxParser.ParseArgs(args);

        if (element is BasicExpression expression) {
            var testResult = expression.Evaluate(variableStore);

            if (Convert.ToBoolean(testResult)) {
                element = syntaxParser.Continue();

                if (element is ThenExpression) {
                    element = syntaxParser.Continue();

                    if (element is NumericExpression numberExpression) {
                        var lineNumber = numberExpression.ToInt();
                        programService.Goto(lineNumber, out var programLine);
                    }
                    else if (element is CommandExpression commandExpression) {
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
