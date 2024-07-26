namespace ClifferBasic.Model;

internal class Variable {
    internal Type Type { get; }

    internal Variable(Type type) {
        Type = type;
    }
}

internal class NumericVariable : Variable {
    internal object Value { get; set; }

    public NumericVariable(Type type, object value) : base(type) {
        Value = value;
    }

    internal bool ToBool() => Convert.ToBoolean(Value);

    internal int ToInt() => Convert.ToInt32(Value);

    internal double ToDouble() => Convert.ToDouble(Value);

    public override string? ToString() => Value.ToString();
}

internal class BooleanVariable : NumericVariable {
    internal BooleanVariable(object value) : base(typeof(bool), value) { }
}

internal class IntegerVariable : NumericVariable {
    internal IntegerVariable(object value) : base(typeof(int), value) { }
}

internal class DoubleVariable : NumericVariable {
    internal DoubleVariable(object value) : base(typeof(double), value) { }
}

internal class StringVariable : Variable {
    internal string Value { get; set; }

    internal StringVariable(string value) : base(typeof(string)) {
        Value = value;
    }

    internal StringVariable(object value) : base(typeof(string)) {
        Value = value?.ToString() ?? string.Empty;
    }

    public override string? ToString() => Value;
}

