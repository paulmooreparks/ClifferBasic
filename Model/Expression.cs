using ClifferBasic.Services;

namespace ClifferBasic.Model;

internal abstract class Expression {
    internal abstract object Evaluate(VariableStore variableStore);
}

internal abstract class LiteralExpression<T> : Expression  {
    internal T Value { get ; }

    internal LiteralExpression(T value) {
        Value = value;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return Value!;
    }

    public override string ToString() {
        return $"{Value?.ToString()}";
    }
}

internal class NumberExpression : LiteralExpression<object> {
    internal NumberExpression(object value) : base(value) {}

    internal int ToInt() {
        return Convert.ToInt32(Value);
    }

    internal double ToDouble() {
        return Convert.ToDouble(Value);
    }
}

internal class StringExpression : LiteralExpression<string> {
    internal StringExpression(string value) : base(value) { }

    internal override object Evaluate(VariableStore variableStore) {
        return Value.ToString()[1..^1];
    }
}

internal class BooleanLiteralExpression : LiteralExpression<bool> {
    internal BooleanLiteralExpression(bool value) : base(value) { }

    internal bool ToBool() {
        return (bool)Value;
    }
}

internal class VariableExpression : Expression {
    internal string Name { get; }

    internal VariableExpression(string name) {
        Name = name;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return variableStore.GetVariable(Name) ?? false;
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

internal class KeywordExpression : Expression {
    internal string Keyword { get; }

    internal KeywordExpression(string name) : base() {
        Keyword = name;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return Keyword;
    }
}

internal class ThenExpression : KeywordExpression {
    internal ThenExpression(string keyword) : base(keyword) { }
}

internal class ToExpression : KeywordExpression {
    internal double ToValue { get; }
    internal ToExpression(string keyword, double toValue) : base(keyword) {
        ToValue = toValue;
    }
}

internal class StepExpression : KeywordExpression {
    internal double StepValue { get; }
    internal StepExpression(string keyword, double stepValue) : base(keyword) { 
        StepValue = stepValue;
    }
}

internal class CommandExpression : Expression {
    internal string[] Args { get; }

    internal CommandExpression(string[] args) : base() { 
        Args = args;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return Args;
    }
}

internal class AssignmentExpression : Expression {
    internal VariableExpression Left { get; }
    internal Expression Right { get; }

    internal AssignmentExpression(VariableExpression left, Expression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return variableStore.SetVariable(Left.Name, Right.Evaluate(variableStore));
    }
}

internal class IntegerAssignmentExpression : Expression {
    internal IntegerVariableExpression Left { get; }
    internal Expression Right { get; }

    internal IntegerAssignmentExpression(IntegerVariableExpression left, Expression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return variableStore.SetVariable(Left.Name, Right.Evaluate(variableStore));
    }
}

internal class DoubleAssignmentExpression : Expression {
    internal DoubleVariableExpression Left { get; }
    internal Expression Right { get; }

    internal DoubleAssignmentExpression(DoubleVariableExpression left, Expression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return variableStore.SetVariable(Left.Name, Right.Evaluate(variableStore));
    }
}

internal class StringAssignmentExpression : Expression {
    internal StringVariableExpression Left { get; }
    internal Expression Right { get; }

    internal StringAssignmentExpression(StringVariableExpression left, Expression right) {
        Left = left;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        return variableStore.SetVariable(Left.Name, Right.Evaluate(variableStore));
    }
}

internal class GroupExpression : Expression {
    private Expression Enclosed { get; }

    internal GroupExpression(Expression enclosed) { 
        Enclosed = enclosed; 
    }

    internal override object Evaluate(VariableStore variableStore) {
        return Enclosed.Evaluate(variableStore);
    }
}

internal class UnaryExpression : Expression {
    internal Token Operator { get; }
    internal Expression Right { get; }

    internal UnaryExpression(Token operatorToken, Expression right) {
        Operator = operatorToken;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        throw new NotImplementedException();
    }
}

internal class BinaryExpression : Expression {
    internal Expression Left { get; }
    internal Token Operator { get; }
    internal Expression Right { get; }

    internal BinaryExpression(Expression left, Token operatorToken, Expression right) {
        Left = left;
        Operator = operatorToken;
        Right = right;
    }

    internal override object Evaluate(VariableStore variableStore) {
        Expression result =  Left switch {
            BinaryExpression lvalue => new BinaryExpression(new NumberExpression(lvalue.Evaluate(variableStore)), Operator, Right),
            GroupExpression lvalue => new BinaryExpression(new NumberExpression(lvalue.Evaluate(variableStore)), Operator, Right),
            NumberExpression lvalue => Right switch {
                BinaryExpression rvalue => new BinaryExpression(lvalue, Operator, new NumberExpression(rvalue.Evaluate(variableStore))),
                GroupExpression rvalue => new BinaryExpression(lvalue, Operator, new NumberExpression(rvalue.Evaluate(variableStore))),
                NumberExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToDouble() + rvalue.ToDouble()),
                    TokenType.Minus => new NumberExpression(lvalue.ToDouble() - rvalue.ToDouble()),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToDouble() * rvalue.ToDouble()),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToDouble() / rvalue.ToDouble()),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToDouble() == rvalue.ToDouble()),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToDouble() != rvalue.ToDouble()),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToDouble() > rvalue.ToDouble()),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble() >= rvalue.ToDouble()),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToDouble() < rvalue.ToDouble()),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble() <= rvalue.ToDouble()),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                IntegerVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToDouble() + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumberExpression(lvalue.ToDouble() - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToDouble() * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToDouble() / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToDouble() == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToDouble() != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToDouble() > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble() >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToDouble() < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble() <= rvalue.ToInt(variableStore)),
                    _ => throw new NotImplementedException()
                },
                DoubleVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToDouble() + rvalue.ToDouble(variableStore)),
                    TokenType.Minus => new NumberExpression(lvalue.ToDouble() - rvalue.ToDouble(variableStore)),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToDouble() * rvalue.ToDouble(variableStore)),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToDouble() / rvalue.ToDouble(variableStore)),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToDouble() == rvalue.ToDouble(variableStore)),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToDouble() != rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToDouble() > rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble() >= rvalue.ToDouble(variableStore)),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToDouble() < rvalue.ToDouble(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble() <= rvalue.ToDouble(variableStore)),
                    _ => throw new NotImplementedException()
                },
                _ => throw new Exception($"Invalid type: {Right}")
            },
            IntegerVariableExpression lvalue => Right switch {
                BinaryExpression rvalue => new BinaryExpression(lvalue, Operator, new NumberExpression(rvalue.Evaluate(variableStore))),
                GroupExpression rvalue => new BinaryExpression(lvalue, Operator, new NumberExpression(rvalue.Evaluate(variableStore))),
                NumberExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToInt(variableStore) + rvalue.ToInt()),
                    TokenType.Minus => new NumberExpression(lvalue.ToInt(variableStore) - rvalue.ToInt()),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToInt(variableStore) * rvalue.ToInt()),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToInt(variableStore) / rvalue.ToInt()),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToInt(variableStore) == rvalue.ToInt()),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) != rvalue.ToInt()),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToInt(variableStore) > rvalue.ToInt()),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) >= rvalue.ToInt()),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToInt(variableStore) < rvalue.ToInt()),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) <= rvalue.ToInt()),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                IntegerVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToInt(variableStore) + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumberExpression(lvalue.ToInt(variableStore) - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToInt(variableStore) * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToInt(variableStore) / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToInt(variableStore) == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToInt(variableStore) > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToInt(variableStore) < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) <= rvalue.ToInt(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                DoubleVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToInt(variableStore) + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumberExpression(lvalue.ToInt(variableStore) - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToInt(variableStore) * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToInt(variableStore) / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToInt(variableStore) == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToInt(variableStore) > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToInt(variableStore) < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToInt(variableStore) <= rvalue.ToInt(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                BooleanLiteralExpression rvalue => Operator.Type switch {
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToBool(variableStore) == rvalue.ToBool()),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToBool(variableStore) != rvalue.ToBool()),
                    _ => throw new NotImplementedException()
                },
                _ => throw new Exception($"Invalid type: {Right}")
            },
            DoubleVariableExpression lvalue => Right switch {
                BinaryExpression rvalue => new BinaryExpression(lvalue, Operator, new NumberExpression(rvalue.Evaluate(variableStore))),
                GroupExpression rvalue => new BinaryExpression(lvalue, Operator, new NumberExpression(rvalue.Evaluate(variableStore))),
                NumberExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToDouble(variableStore) + rvalue.ToDouble()),
                    TokenType.Minus => new NumberExpression(lvalue.ToDouble(variableStore) - rvalue.ToDouble()),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToDouble(variableStore) * rvalue.ToDouble()),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToDouble(variableStore) / rvalue.ToDouble()),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) == rvalue.ToDouble()),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) != rvalue.ToDouble()),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) > rvalue.ToDouble()),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) >= rvalue.ToDouble()),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) < rvalue.ToDouble()),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) <= rvalue.ToDouble()),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                IntegerVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToDouble(variableStore) + rvalue.ToInt(variableStore)),
                    TokenType.Minus => new NumberExpression(lvalue.ToDouble(variableStore) - rvalue.ToInt(variableStore)),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToDouble(variableStore) * rvalue.ToInt(variableStore)),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToDouble(variableStore) / rvalue.ToInt(variableStore)),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) == rvalue.ToInt(variableStore)),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) != rvalue.ToInt(variableStore)),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) > rvalue.ToInt(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) >= rvalue.ToInt(variableStore)),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) < rvalue.ToInt(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) <= rvalue.ToInt(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                DoubleVariableExpression rvalue => Operator.Type switch {
                    TokenType.Plus => new NumberExpression(lvalue.ToDouble(variableStore) + rvalue.ToDouble(variableStore)),
                    TokenType.Minus => new NumberExpression(lvalue.ToDouble(variableStore) - rvalue.ToDouble(variableStore)),
                    TokenType.Asterisk => new NumberExpression(lvalue.ToDouble(variableStore) * rvalue.ToDouble(variableStore)),
                    TokenType.ForwardSlash => new NumberExpression(lvalue.ToDouble(variableStore) / rvalue.ToDouble(variableStore)),
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) == rvalue.ToDouble(variableStore)),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) != rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThan => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) > rvalue.ToDouble(variableStore)),
                    TokenType.GreaterThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) >= rvalue.ToDouble(variableStore)),
                    TokenType.LessThan => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) < rvalue.ToDouble(variableStore)),
                    TokenType.LessThanOrEqual => new BooleanLiteralExpression(lvalue.ToDouble(variableStore) <= rvalue.ToDouble(variableStore)),
                    _ => throw new Exception($"Invalid operator: {Operator.Lexeme}")
                },
                BooleanLiteralExpression rvalue => Operator.Type switch {
                    TokenType.Equal => new BooleanLiteralExpression(lvalue.ToBool(variableStore) == rvalue.ToBool()),
                    TokenType.NotEqual => new BooleanLiteralExpression(lvalue.ToBool(variableStore) != rvalue.ToBool()),
                    _ => throw new NotImplementedException()
                },
                _ => throw new Exception($"Invalid type: {Right}")
            },
            _ => throw new Exception($"Invalid type: {Left}")
        };

        return result.Evaluate(variableStore);
    }
}

