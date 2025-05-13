using System.CommandLine.Invocation;
using System.CommandLine;
using ClifferBasic.Services;
using Cliffer;
using System.Reflection;

namespace ClifferBasic;

internal class BasicReplContext(Command currentCommand) : Cliffer.DefaultReplContext(currentCommand) {
    private readonly CommandSplitter _splitter = Utility.GetService<CommandSplitter>()!;
    private readonly ProgramService _programService = Utility.GetService<ProgramService>()!;

    public override string TitleMessage  {
        get {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? version = assembly.GetName().Version;
            string versionString = version?.ToString() ?? "Unknown";
            return $"Cliffer Basic v{versionString}";
        }
    }

    public override string GetPrompt(Command command, InvocationContext context) => "> ";

    public override string[] ExitCommands => ["bye", "exit", "goodbye"];
    public override string[] PopCommands => [];

    public override void OnEntry() {
        base.OnEntry();
    }

    public override string[] SplitCommandLine(string input) {
        return _splitter.Split(input).ToArray();
    }

    public override Task<int> RunAsync(Command command, string[] args) {
        if (args.Length > 1) {
            if (int.TryParse(args[0], out int lineNumber)) {
                _programService.SetLine(lineNumber, args.Skip(1).ToArray());
                return Task.FromResult(Result.Success);
            }
        }

        ClifferEventHandler.PreprocessArgs(args);
        return base.RunAsync(command, args);
    }
}
