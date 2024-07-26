using System.Globalization;
using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DITransient(typeof(Tokenizer))]
internal class Tokenizer {
    public Tokenizer() { }

    internal IEnumerable<Token> Tokenize(IEnumerable<string> inputs) {
        var tokenList = new List<Token>();

        foreach (var input in inputs) {
            tokenList.AddRange(Tokenize(input));
        }

        return tokenList;
    }

    private string _scanString = string.Empty;

    private int Start { get; set; } = 0;

    private int Position { get; set; } = 0;

    private string ScanString {
        get {
            return _scanString;
        }
        set {
            Start = Position = 0;
            _scanString = value;
        }
    }

    private char CurrentChar { 
        get {
            if (Position >= ScanString.Length) { return '\0'; }
            return ScanString[Position]; 
        } 
    }

    private string CurrentString {
        get {
            if (Start >= ScanString.Length || Position >= ScanString.Length) { return string.Empty; }
            return ScanString.Substring(Start, Position - (Start - 1));
        }
    }

    private char Peek {
        get {
            if (Position + 1 >= ScanString.Length) { return '\0'; }
            return ScanString[Position + 1];
        }
    }

    private string Remaining {
        get {
            if (Position >= ScanString.Length) { return string.Empty; }
            return ScanString[Position..];
        }
    }

    private char Advance() {
        ++Position;
        Start = Position;
        return CurrentChar;
    }

    private string Expand() {
        ++Position;
        return CurrentString;
    }

    private bool IsVariableChar(char c) {
        return char.IsLetterOrDigit(c) | c == '_' | c == '$' | c == '#' | c == '%';
    }

    private readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>() {
        {"then", TokenType.Then},
        {"to", TokenType.To},
        {"step", TokenType.Step},
        {"goto", TokenType.CommandName},
        {"gosub", TokenType.CommandName},
        {"print", TokenType.CommandName},
        {"let", TokenType.CommandName},
    };

    private bool IsKeyword(string compare, out TokenType tokenType) {
        return Keywords.TryGetValue(compare, out tokenType);
    }

    private readonly Dictionary<string, TokenType> Constants = new Dictionary<string, TokenType>() {
        {"true", TokenType.True },
        {"false", TokenType.False },
    };

    private bool IsConstant(string compare, out TokenType tokenType) {
        return Constants.TryGetValue(compare, out tokenType);
    }

    internal IEnumerable<Token> Tokenize(string input) {
        var tokenList = new List<Token>();
        ScanString = input;

        if (string.IsNullOrEmpty(input)) throw new ArgumentNullException(nameof(input));

        while (CurrentChar != '\0') {
            if (Remaining.First() == '"' && Remaining.Last() == '"') {
                var lexeme = Remaining;
                tokenList.Add(new Token(lexeme, TokenType.String, lexeme));
                return tokenList;
            }
            else if (char.IsDigit(CurrentChar) || CurrentChar.ToString() == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) {
                while (char.IsDigit(Peek) || Peek.ToString() == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) {
                    Expand();
                }

                if (double.TryParse(CurrentString, out double doubleValue)) {
                    tokenList.Add(new Token(CurrentString, TokenType.Number, doubleValue));
                }
            }
            else if (char.IsLetterOrDigit(CurrentChar) | CurrentChar == '_') {
                while (IsVariableChar(Peek)) {
                    Expand();
                }

                TokenType tokenType;

                if (!IsKeyword(CurrentString, out tokenType) && !IsConstant(CurrentString, out tokenType)) {
                    tokenType = CurrentString.Last() switch {
                        '$' => TokenType.StringVariableName,
                        '#' => TokenType.IntegerVariableName,
                        '%' => TokenType.DoubleVariableName,
                        _ => TokenType.DoubleVariableName
                    };
                }

                tokenList.Add(new Token(CurrentString, tokenType, CurrentString));
            }
            else {
                switch (CurrentChar) {
                    case '(':
                        tokenList.Add(new Token(CurrentString, TokenType.LeftParenthesis)); 
                        break;
                    case ')':
                        tokenList.Add(new Token(CurrentString, TokenType.RightParenthesis));
                        break;
                    case '+':
                        tokenList.Add(new Token(CurrentString, TokenType.Plus));
                        break;
                    case '-':
                        tokenList.Add(new Token(CurrentString, TokenType.Minus));
                        break;
                    case '*':
                        tokenList.Add(new Token(CurrentString, TokenType.Asterisk));
                        break;
                    case '/':
                        tokenList.Add(new Token(CurrentString, TokenType.ForwardSlash));
                        break;
                    case '&':
                        tokenList.Add(new Token(CurrentString, TokenType.Ampersand));
                        break;
                    case '=':
                        tokenList.Add(new Token(CurrentString, TokenType.Equal));
                        break;
                    case '>':
                        if (Peek == '=') {
                            Expand();
                            tokenList.Add(new Token(CurrentString, TokenType.GreaterThanOrEqual));
                        }
                        else {
                            tokenList.Add(new Token(CurrentString, TokenType.GreaterThan));
                        }
                        break;
                    case '<':
                        if (Peek == '=') {
                            Expand();
                            tokenList.Add(new Token(CurrentString, TokenType.LessThanOrEqual));
                        }
                        else {
                            tokenList.Add(new Token(CurrentString, TokenType.LessThan));
                        }
                        break;

                    default:
                        tokenList.Add(new Token(CurrentString, TokenType.None));
                        break;
                }
            }

            Advance();
        }

        return tokenList;
    }
}

