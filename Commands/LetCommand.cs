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
            var variableValue = binaryExpression.Right.Evaluate(variableStore);
            VariableExpression? variableExpression = null;

            if (binaryExpression.Left is ArrayVariableExpression arrayVariableExpression) {
                variableExpression = arrayVariableExpression.VariableExpression;
                var dimensionExpression = arrayVariableExpression.DimensionExpression;

                if (dimensionExpression != null) {
                    var dimensionArgs = arrayVariableExpression.DimensionExpression.Evaluate(variableStore);
                    var dimensions = new List<int>();

                    if (dimensionArgs is double singleDimension) {
                        dimensions.Add((int)singleDimension);
                    }
                    else if (dimensionArgs is List<object> dimensionList) {
                        foreach (var dimension in dimensionList) {
                            if (dimension is BasicExpression listExpression) {
                                var dimensionEval = listExpression.Evaluate(variableStore);
                                var dimensionValue = Convert.ToInt32(dimensionEval);
                                dimensions.Add(Convert.ToInt32(dimensionValue));
                            }
                            else if (dimension is double listDimension) {
                                var dimensionValue = Convert.ToInt32(listDimension);
                                dimensions.Add(dimensionValue);
                            }
                        }
                    }
                }
            }
            else if (binaryExpression.Left is VariableExpression leftExpression) {
                variableExpression = leftExpression;
            }

            if (variableExpression is not null) {
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

            Console.Error.WriteLine($"Error: Left-hand side of assignment must be a variable");
            return Result.Error;
        }

        Console.Error.WriteLine($"Error: Invalid assignment expression");
        return Result.Error;
    }
}
