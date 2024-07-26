using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("list", "List the current program in memory")]
[Argument(typeof(IEnumerable<int>), "lineNumbers", "One or more line numbers to list", ArgumentArity.ZeroOrMore)]
internal class ListCommand {
    public int Execute(IEnumerable<int> lineNumbers, ProgramService programService) {
        if (lineNumbers.Any()) {
            foreach (var lineNumber in lineNumbers) {
                var line = programService.GetLine(lineNumber);

                if (line is not null) {
                    Console.WriteLine(line);
                }
                else {
                    Console.Error.WriteLine($"Line {lineNumber} not found");
                    return Result.Error;
                }
            }

            return Result.Success;
        }

        programService.Reset();

        while (programService.Next(out var programLine)) {
            Console.WriteLine(programLine);
        }

        return Result.Success;
    }
}
