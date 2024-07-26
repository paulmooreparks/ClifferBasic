using Cliffer;

namespace ClifferBasic.Commands;

[Command("cls", "Clear the screen")]
internal class ClsMethod {
    public int Execute() {
        Console.Clear();
        return Result.Success;
    }
}
