using Cliffer;
using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("let", "Assign a value to a variableItem")]
[Argument(typeof(IEnumerable<string>), "args", "The assignment variableItem", Cliffer.ArgumentArity.ZeroOrMore)]
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
            var variableValue = binaryExpression.Right.Evaluate(variableStore);
            VariableExpression? variableExpression = null;

            if (binaryExpression.Left is ArrayVariableExpression arrayVariableExpression) {
                variableExpression = arrayVariableExpression.VariableExpression;
                var indices = variableStore.GetArrayIndices(arrayVariableExpression);
                var variable = variableStore.GetVariable(arrayVariableExpression.Name);


                if (variable is ArrayVariable arrayVariable) {
                    if (arrayVariable is DoubleArrayVariable doubleArrayVariable) {
                        doubleArrayVariable.SetValue(variableValue, indices);
                        return Result.Success;
                    }
                    else if (arrayVariable is IntegerArrayVariable intArrayVariable) {
                        intArrayVariable.SetValue(variableValue, indices);
                        return Result.Success;
                    }
                    else if (arrayVariable is StringArrayVariable stringArrayVariable) {
                        stringArrayVariable.SetValue(variableValue, indices);
                        return Result.Success;
                    }
                    else {
                        // TODO: Error
                    }
                }
                else {
                    if (variableExpression is DoubleVariableExpression doubleVariable) {
                        variableStore.SetVariable(doubleVariable.Name, new DoubleVariable(variableValue));
                        return Result.Success;
                    }
                    else if (binaryExpression.Left is IntegerVariableExpression integerVariable) {
                        variableStore.SetVariable(integerVariable.Name, new IntegerVariable(variableValue));
                        return Result.Success;
                    }
                    else if (binaryExpression.Left is StringVariableExpression stringVariable) {
                        variableStore.SetVariable(stringVariable.Name, new StringVariable(variableValue));
                        return Result.Success;
                    }
                }
            }
            else if (binaryExpression.Left is VariableExpression leftExpression) {
                variableExpression = leftExpression;
            }

            Console.Error.WriteLine($"Error: Left-hand side of assignment must be a variableItem");
            return Result.Error;
        }

        Console.Error.WriteLine($"Error: Invalid assignment expression");
        return Result.Error;
    }
}
