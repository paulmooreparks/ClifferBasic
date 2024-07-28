using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DITransient(typeof(SyntaxParser))]
internal class SyntaxParser {
    Tokenizer _tokenizer;
    TokenParser _tokenParser;
    VariableStore _variableStore;

    public SyntaxParser(
        Tokenizer tokenizer,
        TokenParser tokenParser,
        VariableStore variableStore) 
    { 
        _tokenizer = tokenizer;
        _tokenParser = tokenParser;
        _variableStore = variableStore;
    }

    internal SyntaxElement? ParseArgs(IEnumerable<string> args) {
        var tokens = _tokenizer.Tokenize(args);
        var expression = _tokenParser.Parse(tokens);
        return expression;
    }

    internal SyntaxElement? ParseArg(string arg) {
        var tokens = _tokenizer.Tokenize(arg);
        var expression = _tokenParser.Parse(tokens);
        return expression;
    }

    internal SyntaxElement? Continue() {
        var expression = _tokenParser.Parse();
        return expression;
    }
}
