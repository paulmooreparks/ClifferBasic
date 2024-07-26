using Cliffer;

namespace ClifferBasic.Commands;

[Command("cls", "Clear the screen")]
internal class ClsCommand {
    public int Execute() {
        Console.Clear();
        return Result.Success;
    }
}
