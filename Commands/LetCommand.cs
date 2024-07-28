﻿using Cliffer;
using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("let", "Assign a value to a variable")]
[Argument(typeof(IEnumerable<string>), "args", "The assignment variable", Cliffer.ArgumentArity.ZeroOrMore)]
internal class LetCommand {
    public int Execute(
        IEnumerable<string> args,
        SyntaxParser syntaxParser,
        VariableStore variableStore) {

        if (args.Count() == 0) {
            Console.Error.WriteLine("Error: No parameters");
            return Result.Error;
        }

        var element = syntaxParser.ParseArgs(args);

        if (element is BinaryExpression binaryExpression && binaryExpression.Operator.Lexeme == "=") {
            if (binaryExpression.Left is DoubleVariableExpression doubleVariable) {
                var variableValue = binaryExpression.Right.Evaluate(variableStore);
                variableStore.SetVariable(doubleVariable.Name, new DoubleVariable(variableValue));
                return Result.Success;
            }
            else if (binaryExpression.Left is IntegerVariableExpression integerVariable) {
                var variableValue = binaryExpression.Right.Evaluate(variableStore);
                variableStore.SetVariable(integerVariable.Name, new IntegerVariable(variableValue));
                return Result.Success;
            }
            else if (binaryExpression.Left is StringVariableExpression stringVariable) {
                var variableValue = binaryExpression.Right.Evaluate(variableStore);
                variableStore.SetVariable(stringVariable.Name, new StringVariable(variableValue));
                return Result.Success;
            }

            Console.Error.WriteLine($"Error: Left-hand side of assignment must be a variable");
            return Result.Error;
        }

        Console.Error.WriteLine($"Error: Invalid assignment element");
        return Result.Error;
    }
}
