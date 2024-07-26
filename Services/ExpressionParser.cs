using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DITransient(typeof(ExpressionParser))]
internal class ExpressionParser {
    private IEnumerator<Token> _tokens = new List<Token>().GetEnumerator();
    private Token? _next;
    private Token? _current;

    public ExpressionParser() { }

    private bool Check(TokenType type) {
        return Peek?.Type == type;
    }

    private Token? Advance() {
        _current = _next;

        if (_tokens.MoveNext()) {
            _next = _tokens.Current;
        }
        else {
            IsAtEnd = true;
            _next = null;
        }


        return _current;
    }

    internal Token? Consume(TokenType type, string errorMessage) {
        if (Check(type)) {
            return Advance();
        }

        throw new InvalidOperationException(errorMessage);
    }

    private bool IsAtEnd { get; set; } = false;

    private Token? Peek => _next;

    private Token? Current => _current;

    internal Expression Parse(IEnumerable<Token> tokens) {
        _tokens = tokens.GetEnumerator();
        Advance();
        return Expression();
    }

    internal Expression? Parse() {
        if (Peek is null) {
            return null;
        }

        return Expression();
    }

    internal Expression Expression() {
        return Equality();
    }

    internal Expression Equality() {
        Expression expr = Comparison();

        while (Peek?.Type == TokenType.Equal || Peek?.Type == TokenType.NotEqual) {
            Token op = Peek;
            Advance();
            Expression right = Comparison();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    internal Expression Comparison() {
        Expression expr = Term();

        while (Peek?.Type == TokenType.GreaterThan || Peek?.Type == TokenType.GreaterThanOrEqual || 
            Peek?.Type == TokenType.LessThan || Peek?.Type == TokenType.LessThanOrEqual) 
        {
            Token op = Peek;
            Advance();
            Expression right = Term();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    internal Expression Term() {
        Expression expr = Factor();

        while (Peek?.Type == TokenType.Minus || Peek?.Type == TokenType.Plus) {
            Token op = Peek;
            Advance();
            Expression right = Factor();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    internal Expression Factor() {
        Expression expr = Unary();

        while (Peek?.Type == TokenType.Asterisk || Peek?.Type == TokenType.ForwardSlash) {
            Token op = Peek;
            Advance();
            Expression right = Unary();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    internal Expression Unary() {
        if (Peek?.Type == TokenType.Minus || Peek?.Type == TokenType.Not) {
            Token op = Peek;
            Advance();
            Expression right = Unary();
            return new UnaryExpression(op, right);
        };

        return Primary();
    }

    internal Expression Primary() {
        if (Peek?.Type == TokenType.False) {
            Advance();
            return new BooleanLiteralExpression(false);
        }
        
        if (Peek?.Type == TokenType.True) {
            Advance();
            return new BooleanLiteralExpression(true);
        }
        
        if (Peek?.Type == TokenType.Number) {
            var literal = Peek.Literal;
            Advance();
            return new NumberExpression(literal!);
        }
        
        if (Peek?.Type == TokenType.String) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new StringExpression(literal);
        }

        if (Peek?.Type == TokenType.IntegerVariableName) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new IntegerVariableExpression(literal);
        }

        if (Peek?.Type == TokenType.DoubleVariableName) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new DoubleVariableExpression(literal);
        }

        if (Peek?.Type == TokenType.StringVariableName) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new StringVariableExpression(literal);
        }

        if (Peek?.Type == TokenType.Then) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new ThenExpression(literal);
        }

        if (Peek?.Type == TokenType.To) {
            var keyword = Peek.Literal!.ToString()!;
            Advance();
            double toValue = Convert.ToDouble(Peek.Literal);
            Advance();
            return new ToExpression(keyword, toValue);
        }

        if (Peek?.Type == TokenType.Step) {
            var keyword = Peek.Literal!.ToString()!;
            Advance();
            double stepValue = Convert.ToDouble(Peek.Literal);
            Advance();
            return new StepExpression(keyword, stepValue);
        }

        if (Peek?.Type == TokenType.Keyword) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new KeywordExpression(literal);
        }

        if (Peek?.Type == TokenType.CommandName) {
            var args = new List<string>();

            while (Peek != null) {
                var arg = Peek.Literal!.ToString()!;
                args.Add(arg);
                Advance();
            }

            return new CommandExpression(args.ToArray());
        }

        if (Peek?.Type == TokenType.LeftParenthesis) {
            Advance();
            Expression expression = Expression();
            Consume(TokenType.RightParenthesis, "Expected closing parenthesis");
            return new GroupExpression(expression);
        }

        throw new InvalidOperationException($"Unexpected token: {Peek}");
    }
}

