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

    internal BooleanVariable() : base(false) { }

    internal override void SetValue(bool value) {
        Value = value;
    }

    internal override void SetValue(object value) {
        SetValue(Convert.ToBoolean(value));
    }
}

internal class IntegerVariable : NumericVariable<int> {
    internal IntegerVariable(object value) : base(Convert.ToInt32(value)) { }

    public IntegerVariable() : base(0) { }

    internal override void SetValue(int value) {
        Value = value;
    }

    internal override void SetValue(object value) {
        SetValue(Convert.ToInt32(value));
    }
}

internal class DoubleVariable : NumericVariable<double> {
    internal DoubleVariable(object value) : base(Convert.ToDouble(value)) { }

    public DoubleVariable() : base(0) { }

    internal override void SetValue(double value) { 
        Value = value; 
    }

    internal override void SetValue(object value) {
        SetValue(Convert.ToDouble(value));
    }
}

internal class StringVariable : ScalarVariable<string> {
    internal StringVariable(string value) : base(value) { }

    internal StringVariable(object value) : base(value?.ToString() ?? string.Empty) { }

    public StringVariable() : base(string.Empty) { }

    internal override void SetValue(string value) {
        Value = value?.ToString() ?? string.Empty;
    }

    internal override void SetValue(object value) {
        SetValue(value.ToString() ?? string.Empty);
    }

    public override string? ToString() => Value;
}

internal abstract class ArrayVariable : Variable {
    internal List<int> Dimensions { get; }

    internal ArrayVariable(List<int> dimensions) {
        Dimensions = dimensions;
    }
}

internal abstract class TypedArrayVariable<T, U> : ArrayVariable where T : Variable, new() where U : notnull {
    internal Array Values { get; }

    internal TypedArrayVariable(List<int> dimensions) : base(dimensions) {
        Values = Array.CreateInstance(typeof(T), dimensions.ToArray());
    }

    internal void SetVariable(T variable, params int[] indices) {
        Values.SetValue(variable, indices);
    }

    internal void SetValue(U value, params int[] indices) {
        var variable = GetVariable(indices);
        variable.SetValue(value);
        Values.SetValue(variable, indices);
    }

    internal void SetValue(object value, params int[] indices) {
        var variable = GetVariable(indices);
        variable.SetValue(value);
        Values.SetValue(variable, indices);
    
    }

    internal T GetVariable(params int[] indices) {
        var retval = Values.GetValue(indices);

        if (retval is not null) {
            
            return (T)retval;
        }

        T newvar = new T();
        SetVariable(newvar, indices);
        return newvar;
    }
}

internal class DoubleArrayVariable : TypedArrayVariable<DoubleVariable, double> {
    internal DoubleArrayVariable(List<int> dimensions) : base(dimensions) { }

    internal double GetValue(params int[] indices) {
        var variable = GetVariable(indices);
        return variable.Value;
    }

    internal override void SetValue(object value) {
        // var variable = GetVariable(indices);
        // variable.SetValue(value);
    }
}

internal class IntegerArrayVariable : TypedArrayVariable<IntegerVariable, int> {
    internal IntegerArrayVariable(List<int> dimensions) : base(dimensions) { }

    internal int GetValue(params int[] indices) {
        var variable = (IntegerVariable)base.GetVariable(indices);
        return variable.ToInt();
    }
    
    internal override void SetValue(object value) {
    }
}

internal class StringArrayVariable : TypedArrayVariable<StringVariable, string> {
    internal StringArrayVariable(List<int> dimensions) : base(dimensions) { }

    internal string GetValue(params int[] indices) {
        var variable = (StringVariable)base.GetVariable(indices);
        return variable.ToString() ?? string.Empty;
    }

    internal override void SetValue(object value) {
    }
}

