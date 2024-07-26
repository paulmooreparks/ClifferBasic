using Cliffer;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;
[Command("end", "End the currently running program")]
internal class EndCommand {
    public void Execute(ProgramService programService) {
        programService.End();
        return;
    }
}
