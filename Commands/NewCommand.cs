using Cliffer;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("new", "Clear the current program from memory")]
internal class NewCommand {
    public void Execute(ProgramService programService) {
        programService.New();
    }
}
