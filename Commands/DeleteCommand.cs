using Cliffer;

using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("delete", "Delete a line from the current program in memory", aliases:["del"])]
[Argument(typeof(int), "lineNumber", "The number of the line to delete")]
internal class DeleteCommand {
    public void Execute(int lineNumber, ProgramService programService) {
        if (programService.HasLine(lineNumber)) {
            programService.RemoveLine(lineNumber);
            return;
        }

        Console.Error.WriteLine($"Line {lineNumber} not found");
    }
}
