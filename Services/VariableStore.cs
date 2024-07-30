using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DISingleton(typeof(VariableStore))]
internal class VariableStore {
    private readonly Dictionary<string, Variable> _variables = new();

    public VariableStore() { 
    }

    internal object SetVariable(string name, Variable value) {
        _variables[name] = value;
        return value;
    }

    internal Variable GetVariable(string name) {
        if (_variables.TryGetValue(name, out Variable? value)) {
            return value;
        }

        throw new InvalidOperationException($"Error: Variable {name} does not exist");
    }

    internal Dictionary<string, Variable> GetAllVariables() {
        return _variables;
    }

    internal int[] GetArrayIndices(ArrayVariableExpression arrayVariableExpression) {
        var dimensions = new List<int>();
        var dimensionExpression = arrayVariableExpression.DimensionExpression;

        if (dimensionExpression is not null) {
            var dimensionArgs = arrayVariableExpression.DimensionExpression.Evaluate(this);

            if (dimensionArgs is double singleDimension) {
                dimensions.Add((int)singleDimension);
            }
            else if (dimensionArgs is List<object> dimensionList) {
                foreach (var dimension in dimensionList) {
                    if (dimension is BasicExpression listExpression) {
                        var dimensionEval = listExpression.Evaluate(this);
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

        return dimensions.ToArray();
    }
}

