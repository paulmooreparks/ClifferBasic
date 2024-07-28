using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cliffer;

using ClifferBasic.Services;
using ClifferBasic.Model;
using ClifferBasic.Commands;

namespace ClifferBasic.Extensions;
internal static class CommandExtensions {
    public static int Implementation(this Command command, 
        SyntaxParser syntaxParser, 
        VariableStore variableStore, 
        SyntaxElement? expression = null
        ) 
    {
        return command.Implementation(syntaxParser, variableStore, expression);
    }
}
