using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;
[Command("next", "Return to the start of a for loop")]
[Argument(typeof(string), "identifier", "The variable name indicating which for loop to continue", Arity = Cliffer.ArgumentArity.ExactlyOne)]
internal class NextCommand {
    public int Execute(
        string identifier,
        ProgramService programService,
        ExpressionBuilder expressionBuilder,
        VariableStore variableStore) {

        programService.ContinueForLoop(identifier);
        return Result.Success;
    }
}
