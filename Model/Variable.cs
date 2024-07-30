namespace ClifferBasic.Model;

internal abstract class Variable<T> {
    internal T Value { get; set; }

    internal Variable(T value) {
        Value = value;
    }
}

internal class NumericVariable<T> : Variable<T> {
    public NumericVariable(T value) : base(value) {
        Value = value;
    }

    internal bool ToBool() => Convert.ToBoolean(Value);

    internal int ToInt() => Convert.ToInt32(Value);

    internal double ToDouble() => Convert.ToDouble(Value);

    public override string? ToString() => Value?.ToString() ?? string.Empty;
}

internal class BooleanVariable : NumericVariable<bool> {
    internal BooleanVariable(object value) : base(Convert.ToBoolean(value)) { }
}

internal class IntegerVariable : NumericVariable<int> {
    internal IntegerVariable(object value) : base(Convert.ToInt32(value)) { }
}

internal class DoubleVariable : NumericVariable<double> {
    internal DoubleVariable(object value) : base(Convert.ToDouble(value)) { }
}

internal class StringVariable : Variable<string> {
    internal StringVariable(string value) : base(value) {
    }

    internal StringVariable(object value) : base(value?.ToString() ?? string.Empty) {
    }

    public override string? ToString() => Value;
}

internal class ArrayVariable<T> {
    internal List<int> Dimensions { get; }
    internal Array Values { get; }

    internal ArrayVariable(List<int> dimensions) { 
        Dimensions = dimensions;
        Values = Array.CreateInstance(typeof(T), dimensions.ToArray());
    }

    internal virtual T GetVariable(params int[] indices) {
        return (T)Values.GetValue(indices)!;
    }

    internal void SetVariable(T variable, params int[] indices) {
        Values.SetValue(variable, indices);
    }
}

internal class DoubleArrayVariable : ArrayVariable<DoubleVariable> {
    internal DoubleArrayVariable(List<int> dimensions) : base(dimensions) {
    }

    internal double GetValue(params int[] indices) {
        var variable = (DoubleVariable)base.GetVariable(indices);
        return variable.ToDouble();
    }
}

internal class IntegerArrayVariable : ArrayVariable<IntegerVariable> {
    internal IntegerArrayVariable(List<int> dimensions) : base(dimensions) {
    }

    internal int GetValue(params int[] indices) {
        var variable = (IntegerVariable)base.GetVariable(indices);
        return variable.ToInt();
    }
}

internal class StringArrayVariable : ArrayVariable<StringVariable> {
    internal StringArrayVariable(List<int> dimensions) : base(dimensions) {
    }

    internal string GetValue(params int[] indices) {
        var variable = (StringVariable)base.GetVariable(indices);
        return variable.ToString() ?? string.Empty;
    }
}

