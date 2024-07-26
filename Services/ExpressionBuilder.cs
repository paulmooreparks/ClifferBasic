using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DITransient(typeof(ExpressionBuilder))]
internal class ExpressionBuilder {
    Tokenizer _tokenizer;
    ExpressionParser _expressionParser;
    VariableStore _variableStore;

    public ExpressionBuilder(
        Tokenizer tokenizer,
        ExpressionParser expressionParser,
        VariableStore variableStore) 
    { 
        _tokenizer = tokenizer;
        _expressionParser = expressionParser;
        _variableStore = variableStore;
    }

    internal Expression? BuildExpression(IEnumerable<string> args) {
        var tokens = _tokenizer.Tokenize(args);
        var expression = _expressionParser.Parse(tokens);
        return expression;
    }

    internal Expression? BuildExpression(string arg) {
        var tokens = _tokenizer.Tokenize(arg);
        var expression = _expressionParser.Parse(tokens);
        return expression;
    }

    internal Expression? BuildExpression() {
        var expression = _expressionParser.Parse();
        return expression;
    }
}
