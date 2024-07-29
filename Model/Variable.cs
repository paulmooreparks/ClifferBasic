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

internal class ArrayVariable : Variable {
    internal List<int> Dimensions { get; }
    internal Array Values { get; }

    internal ArrayVariable(Type type, List<int> dimensions) : base(type) { 
        Dimensions = dimensions;
        Values = Array.CreateInstance(type, dimensions.ToArray());
    }

    internal virtual object GetVariable(params int[] indices) {
        return Values.GetValue(indices)!;
    }

    internal void SetVariable(object variable, params int[] indices) {
        Values.SetValue(variable, indices);
    }
}

internal class DoubleArrayVariable : ArrayVariable {
    internal DoubleArrayVariable(List<int> dimensions) : base(typeof(DoubleVariable), dimensions) {
    }

    internal double GetValue(params int[] indices) {
        var variable = (DoubleVariable)base.GetVariable(indices);
        return variable.ToDouble();
    }
}

internal class IntegerArrayVariable : ArrayVariable {
    internal IntegerArrayVariable(List<int> dimensions) : base(typeof(IntegerVariable), dimensions) {
    }

    internal int GetValue(params int[] indices) {
        var variable = (IntegerVariable)base.GetVariable(indices);
        return variable.ToInt();
    }
}

internal class StringArrayVariable : ArrayVariable {
    internal StringArrayVariable(List<int> dimensions) : base(typeof(StringVariable), dimensions) {
    }

    internal string GetValue(params int[] indices) {
        var variable = (StringVariable)base.GetVariable(indices);
        return variable.ToString() ?? string.Empty;
    }
}

