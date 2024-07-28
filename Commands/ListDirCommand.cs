using Cliffer;

namespace ClifferBasic.Commands;

[Command("listdir", "List the contents of the current directory")]
internal class ListDirCommand {
    public int Execute() {
        var currentDirectory = Directory.GetCurrentDirectory();
        var files = Directory.GetFiles(currentDirectory);
        var directories = Directory.GetDirectories(currentDirectory);

        foreach (var directory in directories) {
            Console.WriteLine($"<DIR> {Path.GetFileName(directory)}");
        }

        foreach (var file in files) {
            Console.WriteLine($"     {Path.GetFileName(file)}");
        }

        return Result.Success;
    }
}