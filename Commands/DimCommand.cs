using System.Formats.Asn1;

using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("dim", "Define a new array")]
[Argument(typeof(IEnumerable<string>), "args", "The dimensions of the array", Cliffer.ArgumentArity.OneOrMore)]
internal class DimCommand {
    public int Execute(
        IEnumerable<string> args,
        SyntaxParser syntaxParser,
        VariableStore variableStore) {

        try {
            var element = syntaxParser.ParseArgs(args);

            if (element is ArrayVariableExpression arrayVariableExpression) {
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

                foreach (var dimension in dimensions) {
                    Console.WriteLine(dimension.ToString());
                }

                var variableExpression = arrayVariableExpression.VariableExpression;

                if (variableExpression is DoubleVariableExpression doubleVariableExpression) {
                    var arrayVariable = new DoubleArrayVariable(dimensions);
                    variableStore.SetVariable(doubleVariableExpression.Name, arrayVariable);
                }
                else if (variableExpression is IntegerVariableExpression integerVariableExpression) {
                    var arrayVariable = new IntegerArrayVariable(dimensions);
                    variableStore.SetVariable(integerVariableExpression.Name, arrayVariable);
                }
                else if (variableExpression is StringVariableExpression stringVariableExpression) {
                    var arrayVariable = new StringArrayVariable(dimensions);
                    variableStore.SetVariable(stringVariableExpression.Name, arrayVariable);
                }
                else {
                    Console.Error.WriteLine($"Error: Invalid variable name");
                    return Result.Error;
                }
            }
            else {
                Console.Error.WriteLine($"Error: Invalid dimension parameter");
                return Result.Error;
            }
        }
        catch (Exception ex) when (ex is FormatException || ex is InvalidCastException || ex is OverflowException) {
            Console.Error.WriteLine($"Error: Invalid dimension parameter");
            return Result.Error;
        }
        catch (Exception) {
            Console.Error.WriteLine($"Error: Syntax error");
            return Result.Error;
        }

        return Result.Success;
    }
}
