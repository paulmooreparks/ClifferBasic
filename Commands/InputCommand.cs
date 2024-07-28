using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("input", "Accept input from the user")]
[Argument(typeof(IEnumerable<string>), "args", "A variable to accept the input and optional text to output as a prompt", Cliffer.ArgumentArity.OneOrMore)]
internal class InputCommand {
    public int Execute(
        IEnumerable<string> args,
        SyntaxParser syntaxParser,
        VariableStore variableStore) {

        if (args.Count() == 0) {
            Console.Error.WriteLine("Error: No parameters");
            return Result.Error;
        }

        var element = syntaxParser.ParseArgs(args);

        if (element is StringExpression stringExpression) {
            var prompt = stringExpression.Value;
            Console.Write(prompt);
            element = syntaxParser.Continue();
        }

        if (element is LineConcatOperator) {
            element = syntaxParser.Continue();
        }
        else {
            Console.WriteLine();
        }

        if (element is VariableExpression variableExpression) {
            string? input;

            // Check if input is piped
            if (Console.IsInputRedirected) {
                input = Console.In.ReadToEnd().Trim();
            }
            else {
                Console.Write("? ");
                input = Console.ReadLine();
            }

            if (input != null) {
                if (variableExpression is DoubleVariableExpression doubleVariable) {
                    variableStore.SetVariable(doubleVariable.Name, new DoubleVariable(input));
                    return Result.Success;
                }
                else if (variableExpression is IntegerVariableExpression integerVariable) {
                    variableStore.SetVariable(integerVariable.Name, new IntegerVariable(input));
                    return Result.Success;
                }
                else if (variableExpression is StringVariableExpression stringVariable) {
                    variableStore.SetVariable(stringVariable.Name, new StringVariable(input));
                    return Result.Success;
                }

                Console.Error.WriteLine($"Error: Invalid variable element");
                return Result.Error;
            }
            else {
                Console.Error.WriteLine($"Error: Invalid input");
                return Result.Error;
            }
        }
        else {
            Console.Error.WriteLine($"Error: Missing input variable");
            return Result.Error;
        }
    }
}
