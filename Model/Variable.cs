namespace ClifferBasic.Model;

internal abstract class Variable {

    internal Variable() {
    }

    internal abstract void SetValue(object value);
}

internal abstract class ScalarVariable<T> : Variable {
    internal T Value { get; set; }

    internal ScalarVariable(T value) {
        Value = value;
    }

    internal abstract void SetValue(T value);
}

internal abstract class NumericVariable<T> : ScalarVariable<T> {
    public NumericVariable(T value) : base(value) {
        Value = value;
    }

    internal bool ToBool() => Convert.ToBoolean(Value);

    internal int ToInt() => Convert.ToInt32(Value);

    internal double ToDouble() => Convert.ToDouble(Value);

    internal override void SetValue(T value) => Value = value;
    
    public override string? ToString() => Value?.ToString() ?? string.Empty;
}

internal class BooleanVariable : NumericVariable<bool> {
    internal BooleanVariable(object value) : base(Convert.ToBoolean(value)) { }
    internal override void SetValue(object value) => Value = Convert.ToBoolean(Value);
}

internal class IntegerVariable : NumericVariable<int> {
    internal IntegerVariable(object value) : base(Convert.ToInt32(value)) { }
    internal override void SetValue(object value) => Value = Convert.ToInt32(Value);
}

internal class DoubleVariable : NumericVariable<double> {
    internal DoubleVariable(object value) : base(Convert.ToDouble(value)) { }
    internal override void SetValue(object value) => Value = Convert.ToDouble(Value);
}

internal class StringVariable : ScalarVariable<string> {
    internal StringVariable(string value) : base(value) {
    }

    internal StringVariable(object value) : base(value?.ToString() ?? string.Empty) {
    }

    internal override void SetValue(object value) => Value = value?.ToString() ?? string.Empty;

    internal override void SetValue(string value) => Value = value;

    public override string? ToString() => Value;
}

internal abstract class ArrayVariable : Variable {
    internal List<int> Dimensions { get; }

    internal ArrayVariable(List<int> dimensions) { 
        Dimensions = dimensions;
    }

    internal override void SetValue(object value) {
        throw new NotImplementedException();
    }

    internal abstract Variable GetVariable(params int[] indices); 
}

internal abstract class TypedArrayVariable<T> : ArrayVariable where T : Variable {
    internal Array Values { get; }

    internal TypedArrayVariable(List<int> dimensions) : base(dimensions) {
        Values = Array.CreateInstance(typeof(T), dimensions.ToArray());
    }

    internal void SetVariable(T variable, params int[] indices) {
        Values.SetValue(variable, indices);
    }

    internal override T GetVariable(params int[] indices) {
        var retval = Values.GetValue(indices);

        if (retval is not null) {
            return (T)retval;
        }

        throw new IndexOutOfRangeException(nameof(indices));
    }
}

internal class DoubleArrayVariable : TypedArrayVariable<DoubleVariable> {
    internal DoubleArrayVariable(List<int> dimensions) : base(dimensions) {
    }

    internal double GetValue(params int[] indices) {
        var variable = GetVariable(indices);
        return variable.Value;
    }
}

internal class IntegerArrayVariable : TypedArrayVariable<IntegerVariable> {
    internal IntegerArrayVariable(List<int> dimensions) : base(dimensions) {
    }

    internal int GetValue(params int[] indices) {
        var variable = (IntegerVariable)base.GetVariable(indices);
        return variable.ToInt();
    }
}

internal class StringArrayVariable : TypedArrayVariable<StringVariable> {
    internal StringArrayVariable(List<int> dimensions) : base(dimensions) {
    }

    internal string GetValue(params int[] indices) {
        var variable = (StringVariable)base.GetVariable(indices);
        return variable.ToString() ?? string.Empty;
    }
}

