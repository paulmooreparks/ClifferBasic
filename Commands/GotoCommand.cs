using Cliffer;
using ClifferBasic.Commands;
using ClifferBasic.Services;

[Command("goto", "Jump to a line and continue execution")]
[Argument(typeof(int), "lineNumber", "The line number to jump to")]
internal class GotoCommand {
    public void Execute(int lineNumber, ProgramService programService) {
        var success = programService.Goto(lineNumber, out var programLine);

        if (!success) {
            Console.Error.WriteLine($"Line {lineNumber} not found.");
            var command = new EndCommand();
            command.Execute(programService);
        }
    }
}
