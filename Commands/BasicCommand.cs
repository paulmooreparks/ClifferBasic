using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cliffer;
using ClifferBasic.Services;
using ClifferBasic.Model;

namespace ClifferBasic.Commands;
internal class BasicCommand {
    internal virtual int Implementation(
        ExpressionBuilder expressionBuilder,
        VariableStore variableStore,
        Expression? expression = null
        ) 
    {
        return Result.Success;
    }
}
