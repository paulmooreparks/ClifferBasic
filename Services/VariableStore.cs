using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DISingleton(typeof(VariableStore))]
internal class VariableStore {
    private readonly Dictionary<string, object> _variables = new();

    public VariableStore() { 
    }

    internal object SetVariable(string name, object value) {
        _variables[name] = value;
        return value;
    }

    internal object? GetVariable(string name) {
        if (_variables.TryGetValue(name, out object? value)) {
            return value;
        }

        return null;
    }

    internal Dictionary<string, object> GetAllVariables() {
        return _variables;
    }
}

