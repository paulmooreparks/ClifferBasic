using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DITransient(typeof(TokenParser))]
internal class TokenParser {
    private IEnumerator<Token> _tokens = new List<Token>().GetEnumerator();
    private Token? _next;
    private Token? _current;

    public TokenParser() { }

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

    internal SyntaxElement Parse(IEnumerable<Token> tokens) {
        _tokens = tokens.GetEnumerator();
        Advance();
        return Element();
    }

    internal SyntaxElement? Parse() {
        if (Peek is null) {
            return null;
        }

        return Element();
    }

    internal SyntaxElement Element() {
        return Equality();
    }

    internal SyntaxElement Equality() {
        SyntaxElement element = Comparison();

        if (element is BasicExpression left) {
            while (Peek?.Type == TokenType.Equal || Peek?.Type == TokenType.NotEqual) {
                Token op = Peek;
                Advance();
                element = Comparison();

                if (element is BasicExpression right) {
                    element = new BinaryExpression(left, op, right);
                }
                else {
                    throw new InvalidOperationException("Error: Expected left");
                }
            }
        }

        return element;
    }

    internal SyntaxElement Comparison() {
        SyntaxElement element = List();

        if (element is BasicExpression left) {
            while (Peek?.Type == TokenType.GreaterThan || Peek?.Type == TokenType.GreaterThanOrEqual ||
                Peek?.Type == TokenType.LessThan || Peek?.Type == TokenType.LessThanOrEqual) {
                Token op = Peek;
                Advance();
                element = List();

                if (element is BasicExpression right) {
                    element = new BinaryExpression(left, op, right);
                }
                else {
                    throw new InvalidOperationException("Error: Expected dimensionExpression");
                }
            }
        }

        return element;
    }

    internal SyntaxElement List() {
        SyntaxElement element = Term();

        if (element is BasicExpression expression && Peek?.Type == TokenType.Comma) {
            var list = new List<BasicExpression> {
                expression
            };

            while (Peek?.Type == TokenType.Comma) {
                Advance();
                element = Term();

                if (element is BasicExpression expressionNext) {
                    list.Add(expressionNext);
                }
                else {
                    throw new InvalidOperationException("Error: Expected dimensionExpression");
                }
            }

            return new ListExpression(list);
        }

        return element;
    }

    internal SyntaxElement Term() {
        SyntaxElement element = Factor();

        if (element is BasicExpression left) {
            while (Peek?.Type == TokenType.Minus || Peek?.Type == TokenType.Plus) {
                Token op = Peek;
                Advance();
                element = Factor();

                if (element is BasicExpression right) {
                    element = new BinaryExpression(left, op, right);
                }
                else {
                    throw new InvalidOperationException("Error: Expected dimensionExpression");
                }
            }
        }

        return element;
    }

    internal SyntaxElement Factor() {
        SyntaxElement element = Unary();

        if (element is BasicExpression left) {
            while (Peek?.Type == TokenType.Asterisk || Peek?.Type == TokenType.ForwardSlash) {
                Token op = Peek;
                Advance();
                element = Unary();

                if (element is BasicExpression right) {
                    element = new BinaryExpression(left, op, right);
                }
                else {
                    throw new InvalidOperationException("Error: Expected dimensionExpression");
                }
            }
        }

        return element;
    }

    internal SyntaxElement Unary() {
        if (Peek?.Type == TokenType.Minus || Peek?.Type == TokenType.Not) {
            Token op = Peek;
            Advance();
            SyntaxElement right = Unary();
            return new UnaryExpression(op, right);
        };

        return Variable();
    }

    internal SyntaxElement Variable() {
        if (Peek?.Type == TokenType.IntegerVariableName || Peek?.Type == TokenType.DoubleVariableName || Peek?.Type == TokenType.StringVariableName) {
            var tokenType = Peek?.Type;
            var variableName = Peek?.Literal!.ToString()!;
            Advance();

            if (Peek?.Type == TokenType.LeftParenthesis) {
                Advance();
                SyntaxElement element = Element();
                Consume(TokenType.RightParenthesis, "Expected closing parenthesis");
                ListExpression? listExpression = null;

                if (element is ListExpression dimensionExpression) {
                    listExpression = dimensionExpression;
                }
                else if (element is NumericExpression numericExpression) {
                    var list = new List<BasicExpression> {
                        numericExpression
                    };
                    listExpression = new ListExpression(list);
                }

                if (listExpression is not null) {
                    if (tokenType == TokenType.IntegerVariableName) {
                        return new ArrayVariableExpression(new IntegerVariableExpression(variableName), listExpression);
                    }

                    if (tokenType == TokenType.DoubleVariableName) {
                        return new ArrayVariableExpression(new DoubleVariableExpression(variableName), listExpression);
                    }

                    if (tokenType == TokenType.StringVariableName) {
                        return new ArrayVariableExpression(new StringVariableExpression(variableName), listExpression);
                    }
                }
            }

            if (tokenType == TokenType.IntegerVariableName) {
                return new IntegerVariableExpression(variableName);
            }

            if (tokenType == TokenType.DoubleVariableName) {
                return new DoubleVariableExpression(variableName);
            }

            if (tokenType == TokenType.StringVariableName) {
                return new StringVariableExpression(variableName);
            }
        }

        return Primary();
    }

    internal SyntaxElement Primary() {
        if (Peek?.Type == TokenType.False) {
            Advance();
            return new BooleanExpression(false);
        }
        
        if (Peek?.Type == TokenType.True) {
            Advance();
            return new BooleanExpression(true);
        }
        
        if (Peek?.Type == TokenType.Number) {
            var literal = Peek.Literal;
            Advance();
            return new NumericExpression(literal!);
        }
        
        if (Peek?.Type == TokenType.String) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new StringExpression(literal);
        }

        if (Peek?.Type == TokenType.Then) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new ThenExpression(literal);
        }

        if (Peek?.Type == TokenType.Semicolon) {
            var literal = Peek.Lexeme;
            Advance();
            return new LineConcatOperator(literal);
        }

        if (Peek?.Type == TokenType.To) {
            var keyword = Peek.Literal!.ToString()!;
            Advance();
            SyntaxElement element = Element();

            if (element is BasicExpression toValue) {
                return new ToKeyword(keyword, toValue);
            }

            throw new InvalidOperationException($"Unexpected token: {Peek}");
        }

        if (Peek?.Type == TokenType.Step) {
            var keyword = Peek.Literal!.ToString()!;
            Advance();
            SyntaxElement element = Element();

            if (element is BasicExpression stepValue) {
                return new StepExpression(stepValue);
            }

            return element;
        }

        if (Peek?.Type == TokenType.Keyword) {
            var literal = Peek.Literal!.ToString()!;
            Advance();
            return new KeywordElement(literal);
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
            SyntaxElement element = Element();
            Consume(TokenType.RightParenthesis, "Expected closing parenthesis");

            if (element is BasicExpression expression) {
                return new ParentheticalExpression(expression);
            }
        }

        throw new InvalidOperationException($"Unexpected token: {Peek}");
    }
}

