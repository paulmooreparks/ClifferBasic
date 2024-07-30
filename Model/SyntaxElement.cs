using ClifferBasic.Services;

namespace ClifferBasic.Model;

internal abstract class SyntaxElement {
}

internal abstract class BasicExpression : SyntaxElement {
    internal abstract object Evaluate(VariableStore variableStore);
}

internal abstract class Literal<T> : BasicExpression {
    internal virtual T Value { get ; }

    internal Literal(T value) {
        Value = value;
    }

    public override string ToString() {
        return $"{Value?.ToString()}";
    }

    internal override object Evaluate(VariableStore variableStore) {
        return Value!;
    }
}

#if false
internal class NumericLiteral : Literal<object> {
    internal NumericLiteral(object value) : base(value) {}

    internal int ToInt() {
        return Convert.ToInt32(Value);
    }

    internal double ToDouble() {
        return Convert.ToDouble(Value);
    }
}
#endif

internal class StringExpression : Literal<string> {
    internal override string Value => base.Value.ToString()[1..^1];

    internal StringExpression(string value) : base(value) { }

    public override string ToString() {
        return Value;
    }
}

internal class BooleanExpression : Literal<bool> {
    internal BooleanExpression(bool value) : base(value) { }

    internal bool ToBool() {
        return (bool)Value;
    }
}

internal class VariableExpression : BasicExpression {
    internal string Name { get; }

    internal VariableExpression(string name) {
        Name = name;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return variableStore.GetVariable(Name);
    }
}

internal class ArrayVariableExpression : VariableExpression {
    internal VariableExpression VariableExpression { get; }
    internal ListExpression DimensionExpression { get; }

    internal ArrayVariableExpression(VariableExpression variableExpression, ListExpression dimensionExpression) : base(variableExpression.Name) {
        VariableExpression = variableExpression;
        DimensionExpression = dimensionExpression;
    }

    internal override Variable Evaluate(VariableStore variableStore) {
        return variableStore.GetVariable(Name);
    }
}

internal class IntegerVariableExpression : VariableExpression {
    internal IntegerVariableExpression(string name) : base(name) { }

    internal int ToInt(VariableStore store) {
        return Convert.ToInt32(Evaluate(store));
    }

    internal bool ToBool(VariableStore store) {
        return Convert.ToBoolean(Evaluate(store));
    }

    internal override object Evaluate(VariableStore variableStore) {
        var variable = variableStore.GetVariable(Name) as IntegerVariable;
        return variable?.Value ?? throw new InvalidDataException($"Invalid integer variable: {Name}");
    }
}

internal class DoubleVariableExpression : VariableExpression {
    internal DoubleVariableExpression(string name) : base(name) { }

    internal double ToDouble(VariableStore store) {
        return Convert.ToDouble(Evaluate(store));
    }

    internal double ToInt(VariableStore store) {
        return Convert.ToInt32(Evaluate(store));
    }

    internal bool ToBool(VariableStore store) {
        return Convert.ToBoolean(Evaluate(store));
    }

    internal override object Evaluate(VariableStore variableStore) {
        var variable = variableStore.GetVariable(Name) as DoubleVariable;
        return variable?.Value ?? throw new InvalidDataException($"Invalid double variable: {Name}");
    }
}

internal class StringVariableExpression : VariableExpression {
    internal StringVariableExpression(string name) : base(name) { }

    internal string ToString(VariableStore variableStore) {
        return variableStore.GetVariable(Name)?.ToString() ?? string.Empty;
    }

    internal override object Evaluate(VariableStore variableStore) {
        var variable = variableStore.GetVariable(Name) as StringVariable;
        return variable?.Value ?? throw new InvalidDataException($"Invalid string variable: {Name}");
    }
}

internal class NumericExpression : BasicExpression {
    internal object Value { get; }

    internal NumericExpression(object value) {
        Value = value;
    }

    internal int ToInt() {
        return Convert.ToInt32(Value);
    }

    internal double ToDouble() {
        return Convert.ToDouble(Value);
    }
    internal override object Evaluate(VariableStore variableStore) {
        return Value;
    }
}

internal class KeywordElement : SyntaxElement {
    internal string Keyword { get; }

    internal KeywordElement(string name) : base() {
        Keyword = name;
    }

    public override string ToString() {
        return Keyword;
    }
}

internal class OperatorElement : SyntaxElement {
    internal string Operator { get; }

    internal OperatorElement(string name) : base() {
        Operator = name;
    }

    public override string ToString() {
        return Operator;
    }
}

internal class ThenExpression : KeywordElement {
    internal ThenExpression(string keyword) : base(keyword) { }
}

internal class LineConcatOperator : OperatorElement {
    internal LineConcatOperator(string op) : base(op) { }
}

internal class ToKeyword : KeywordElement {
    internal BasicExpression ToValue { get; }
    internal ToKeyword(string keyword, BasicExpression toValue) : base(keyword) {
        ToValue = toValue;
    }
}

internal class StepExpression : BasicExpression {
    internal double StepValue { get; }

    internal StepExpression(double stepValue) : base() { 
        StepValue = stepValue;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return StepValue;
    }
}

internal class CommandExpression : BasicExpression {
    internal string[] Args { get; }

    internal CommandExpression(string[] args) : base() { 
        Args = args;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return Args;
    }
}

internal class AssignmentExpression : BasicExpression {
    internal VariableExpression Left { get; }
    internal BasicExpression Right { get; }

    internal AssignmentExpression(VariableExpression left, BasicExpression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        var result = Right.Evaluate(variableStore);

        if (result is Variable variable) {
            return variableStore.SetVariable(Left.Name, variable);
        }

        return result;
    }
}

internal class IntegerAssignmentExpression : BasicExpression {
    internal IntegerVariableExpression Left { get; }
    internal BasicExpression Right { get; }

    internal IntegerAssignmentExpression(IntegerVariableExpression left, BasicExpression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        var result = Right.Evaluate(variableStore);

        if (result is Variable variable) {
            return variableStore.SetVariable(Left.Name, variable);
        }

        return result;
    }
}

internal class DoubleAssignmentExpression : BasicExpression {
    internal DoubleVariableExpression Left { get; }
    internal BasicExpression Right { get; }

    internal DoubleAssignmentExpression(DoubleVariableExpression left, BasicExpression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        var result = Right.Evaluate(variableStore);

        if (result is Variable variable) {
            return variableStore.SetVariable(Left.Name, variable);
        }

        return result;
    }
}

internal class StringAssignmentExpression : BasicExpression {
    internal StringVariableExpression Left { get; }
    internal BasicExpression Right { get; }

    internal StringAssignmentExpression(StringVariableExpression left, BasicExpression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        var result = Right.Evaluate(variableStore);

        if (result is Variable variable) {
            return variableStore.SetVariable(Left.Name, variable);
        }

        return result;
    }
}

internal class ParentheticalExpression : BasicExpression {
    private BasicExpression Enclosed { get; }

    internal ParentheticalExpression(BasicExpression enclosed) { 
        Enclosed = enclosed; 
    }

    internal override object Evaluate(VariableStore variableStore) {
        return Enclosed.Evaluate(variableStore);
    }
}

internal class UnaryExpression : BasicExpression {
    internal Token? Operator { get; }
    internal SyntaxElement Right { get; }

    internal UnaryExpression(Token operatorToken, SyntaxElement right) {
        Operator = operatorToken;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        var result = Right switch {
            BasicExpression rvalue => rvalue.Evaluate(variableStore),
            _ => throw new NotImplementedException()
        };

        return result;
    }
}

internal class ListExpression : BasicExpression {
    internal List<BasicExpression> Expressions { get; }

    public ListExpression(List<BasicExpression> expressions) {
        Expressions = expressions;
    }

    internal override object Evaluate(VariableStore variableStore) {
        var results = new List<object>();

        foreach (var expression in Expressions) {
            results.Add(expression.Evaluate(variableStore));
        }

        return results;
    }
}

internal class BinaryExpression : BasicExpression {
    internal BasicExpression Left { get; }
    internal Token Operator { get; }
    internal BasicExpression Right { get; }

    internal BinaryExpression(BasicExpression left, Token operatorToken, BasicExpression right) {
        Left = left;
        Operator = operatorToken;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        BasicExpression result =  Left switch {
            null => throw new Exception($"Invalid type: {Left}"),
            BinaryExpression lvalue => new BinaryExpression(new NumericExpression(lvalue.Evaluate(variableStore)), Operator, Right),
            ParentheticalExpression lvalue => new BinaryExpression(new NumericExpression(lvalue.Evaluate(variableStore)), Operator, Right),
            NumericExpression lvalue => Right switch {
                BinaryExpression rvalue => new BinaryExpression(lvalue, Operator, new NumericExpression(rvalue.Evaluate(variableStore))),
                ParentheticalExpression rvalue => new BinaryExpression(lvalue, Operator, new NumericExpression(rvalue.Evaluate(variableStore))),
                NumericExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToDouble() + rvalue.ToDouble()),
                    TokenType.Minus => new NumericExpression(lvalue.ToDouble() - rvalue.ToDouble()),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToDouble() * rvalue.ToDouble()),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToDouble() / rvalue.ToDouble()),
                    TokenType.Equal => new BooleanExpression(lvalue.ToDouble() == rvalue.ToDouble()),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToDouble() != rvalue.ToDouble()),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToDouble() > rvalue.ToDouble()),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToDouble() >= rvalue.ToDouble()),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToDouble() < rvalue.ToDouble()),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToDouble() <= rvalue.ToDouble()),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                IntegerVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToDouble() + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumericExpression(lvalue.ToDouble() - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToDouble() * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToDouble() / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanExpression(lvalue.ToDouble() == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToDouble() != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToDouble() > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToDouble() >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToDouble() < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToDouble() <= rvalue.ToInt(variableStore)),
                    _ => throw new NotImplementedException()
                },
                DoubleVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToDouble() + rvalue.ToDouble(variableStore)),
                    TokenType.Minus => new NumericExpression(lvalue.ToDouble() - rvalue.ToDouble(variableStore)),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToDouble() * rvalue.ToDouble(variableStore)),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToDouble() / rvalue.ToDouble(variableStore)),
                    TokenType.Equal => new BooleanExpression(lvalue.ToDouble() == rvalue.ToDouble(variableStore)),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToDouble() != rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToDouble() > rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToDouble() >= rvalue.ToDouble(variableStore)),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToDouble() < rvalue.ToDouble(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToDouble() <= rvalue.ToDouble(variableStore)),
                    _ => throw new NotImplementedException()
                },
                _ => throw new Exception($"Invalid type: {Right}")
            },
            IntegerVariableExpression lvalue => Right switch {
                BinaryExpression rvalue => new BinaryExpression(lvalue, Operator, new NumericExpression(rvalue.Evaluate(variableStore))),
                ParentheticalExpression rvalue => new BinaryExpression(lvalue, Operator, new NumericExpression(rvalue.Evaluate(variableStore))),
                NumericExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToInt(variableStore) + rvalue.ToInt()),
                    TokenType.Minus => new NumericExpression(lvalue.ToInt(variableStore) - rvalue.ToInt()),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToInt(variableStore) * rvalue.ToInt()),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToInt(variableStore) / rvalue.ToInt()),
                    TokenType.Equal => new BooleanExpression(lvalue.ToInt(variableStore) == rvalue.ToInt()),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToInt(variableStore) != rvalue.ToInt()),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToInt(variableStore) > rvalue.ToInt()),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToInt(variableStore) >= rvalue.ToInt()),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToInt(variableStore) < rvalue.ToInt()),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToInt(variableStore) <= rvalue.ToInt()),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                IntegerVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToInt(variableStore) + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumericExpression(lvalue.ToInt(variableStore) - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToInt(variableStore) * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToInt(variableStore) / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanExpression(lvalue.ToInt(variableStore) == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToInt(variableStore) != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToInt(variableStore) > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToInt(variableStore) >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToInt(variableStore) < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToInt(variableStore) <= rvalue.ToInt(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                DoubleVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToInt(variableStore) + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumericExpression(lvalue.ToInt(variableStore) - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToInt(variableStore) * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToInt(variableStore) / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanExpression(lvalue.ToInt(variableStore) == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToInt(variableStore) != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToInt(variableStore) > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToInt(variableStore) >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToInt(variableStore) < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToInt(variableStore) <= rvalue.ToInt(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                BooleanExpression rvalue => Operator.Type switch {
                    TokenType.Equal => new BooleanExpression(lvalue.ToBool(variableStore) == rvalue.ToBool()),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToBool(variableStore) != rvalue.ToBool()),
                    _ => throw new NotImplementedException()
                },
                _ => throw new Exception($"Invalid type: {Right}")
            },
            DoubleVariableExpression lvalue => Right switch {
                BinaryExpression rvalue => new BinaryExpression(lvalue, Operator, new NumericExpression(rvalue.Evaluate(variableStore))),
                ParentheticalExpression rvalue => new BinaryExpression(lvalue, Operator, new NumericExpression(rvalue.Evaluate(variableStore))),
                NumericExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToDouble(variableStore) + rvalue.ToDouble()),
                    TokenType.Minus => new NumericExpression(lvalue.ToDouble(variableStore) - rvalue.ToDouble()),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToDouble(variableStore) * rvalue.ToDouble()),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToDouble(variableStore) / rvalue.ToDouble()),
                    TokenType.Equal => new BooleanExpression(lvalue.ToDouble(variableStore) == rvalue.ToDouble()),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToDouble(variableStore) != rvalue.ToDouble()),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToDouble(variableStore) > rvalue.ToDouble()),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToDouble(variableStore) >= rvalue.ToDouble()),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToDouble(variableStore) < rvalue.ToDouble()),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToDouble(variableStore) <= rvalue.ToDouble()),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                IntegerVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToDouble(variableStore) + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumericExpression(lvalue.ToDouble(variableStore) - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToDouble(variableStore) * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToDouble(variableStore) / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanExpression(lvalue.ToDouble(variableStore) == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToDouble(variableStore) != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToDouble(variableStore) > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToDouble(variableStore) >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToDouble(variableStore) < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToDouble(variableStore) <= rvalue.ToInt(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                DoubleVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumericExpression(lvalue.ToDouble(variableStore) + rvalue.ToDouble(variableStore)),
                    TokenType.Minus => new NumericExpression(lvalue.ToDouble(variableStore) - rvalue.ToDouble(variableStore)),
                    TokenType.Asterisk => new NumericExpression(lvalue.ToDouble(variableStore) * rvalue.ToDouble(variableStore)),
                    TokenType.ForwardSlash => new NumericExpression(lvalue.ToDouble(variableStore) / rvalue.ToDouble(variableStore)),
                    TokenType.Equal => new BooleanExpression(lvalue.ToDouble(variableStore) == rvalue.ToDouble(variableStore)),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToDouble(variableStore) != rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThan => new BooleanExpression(lvalue.ToDouble(variableStore) > rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanExpression(lvalue.ToDouble(variableStore) >= rvalue.ToDouble(variableStore)),
                    TokenType.LessThan => new BooleanExpression(lvalue.ToDouble(variableStore) < rvalue.ToDouble(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanExpression(lvalue.ToDouble(variableStore) <= rvalue.ToDouble(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                BooleanExpression rvalue => Operator.Type switch {
                    TokenType.Equal => new BooleanExpression(lvalue.ToBool(variableStore) == rvalue.ToBool()),
                    TokenType.NotEqual => new BooleanExpression(lvalue.ToBool(variableStore) != rvalue.ToBool()),
                    _ => throw new NotImplementedException()
                },
                _ => throw new Exception($"Invalid type: {Right}")
            },
            _ => throw new Exception($"Invalid type: {Left}")
        };

        return result.Evaluate(variableStore);
    }
}

