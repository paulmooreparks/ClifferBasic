using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

using Cliffer;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("run", "Run the program currently in memory, or optionally load a program from storage and run it")]
[Argument(typeof(string), "filename", "The name of the file to load into memory", arity: Cliffer.ArgumentArity.ZeroOrOne)]
internal class RunCommand {
    private readonly List<string> _illegalCommands = new () { 
        "add",
        "bye", 
        "del",
        "delete",
        "exit",
        "goodbye",
        "list", 
        "new", 
        "renumber", 
        "run", 
    };

    public RunCommand() { 
        _illegalCommands.Sort();
    }

    public async Task<int> Execute(string filename, CommandSplitter splitter, InvocationContext context, ProgramService programService) {
        if (!string.IsNullOrEmpty(filename)) {
            programService.Load(filename);
        }

        programService.Reset();

        while (programService.Next(out var programLine)) {
            var tokens = programLine.Tokens;
            var parseResult = context.Parser.Parse(tokens);

            if (parseResult != null) {
                var commandName = parseResult.CommandResult.Command.Name;

                if (string.Equals("end", commandName)) {
                    await parseResult.CommandResult.Command.InvokeAsync([]);
                    return Result.Success;
                }

                if (_illegalCommands.Contains(commandName)) {
                    Console.Error.WriteLine($"Illegal command: {commandName}");
                    return Result.Error;
                }

                var result = await parseResult.InvokeAsync();

                if (result == Result.Error) {
                    Console.Error.WriteLine($"Program terminated at line {programLine.LineNumber}");
                    return result;
                }
            }
            else {
                Console.Error.WriteLine($"Invalid command: {programLine.ToString()}");
                return Result.Error;
            }
        }

        return Result.Success;
    }
}
