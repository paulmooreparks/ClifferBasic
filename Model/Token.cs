namespace ClifferBasic.Model;

internal class Token {
    internal string Lexeme { get; set; }
    internal TokenType Type { get; set; }
    internal object? Literal { get; set; } = null;

    public Token(string lexeme, TokenType type) {
        Lexeme = lexeme;
        Type = type;
    }

    public Token(string lexeme, TokenType type, object literal) : this(lexeme, type) {
        Literal = literal;
    }

    public override string ToString() {
        return Lexeme;
    }
}

internal enum TokenType {
    None,
    SingleQuote,
    DoubleQuote,
    EscapedDoubleQuote,
    EscapedSingleQuote,
    String,
    Number,
    Boolean,
    VariableName,
    StringVariableName,
    IntegerVariableName,
    DoubleVariableName,
    CommandName,
    Assignment,
    LeftParenthesis,
    RightParenthesis,
    Equal,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    NotEqual,
    Plus,
    Minus,
    Asterisk,
    ForwardSlash, 
    Backslash,
    Ampersand,
    DollarSign,
    Hash,
    BackTick,
    And,
    Or,
    Not,
    Xor,
    False,
    True,
    Then,
    To,
    Step,
    Keyword,
    Unknown
}
