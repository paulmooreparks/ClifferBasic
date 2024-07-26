using ClifferBasic.Model;

using DIAttrib;

namespace ClifferBasic.Services;

[DISingleton(typeof(ProgramService))]
internal class ProgramService {
    private string _filePath = string.Empty;

    private ProgramModel Program { get; }

    public ProgramService(CommandSplitter splitter) {
        Program = new ProgramModel(splitter);
    }

    internal ProgramModel Load(string filename) {
        try {
            if (File.Exists(filename)) {
                _filePath = Path.GetFullPath(filename);
                var splitter = Utility.GetService<CommandSplitter>()!;
                var lines = File.ReadAllLines(_filePath);
                Program.New();

                foreach (var line in lines) {
                    var tokens = splitter.Split(line).ToArray();
                    if (tokens.Length > 1 && int.TryParse(tokens[0], out int lineNumber)) {
                        Program.SetLine(lineNumber, tokens.Skip(1).ToArray());
                    }
                    else {
                        throw new ApplicationException($"Invalid line: {line}");
                    }
                }
            }
            else {
                throw new ApplicationException($"File does not exist: {filename}");
            }

            return Program;
        }
        finally {
        }
    }

    internal void Save(string filename) {
        try {
            if (File.Exists(filename)) {
                File.Delete(filename);
            }

            File.AppendAllLines(filename, Program.Listing);
            _filePath = Path.GetFullPath(filename);
        }
        finally {
        }
    }

    internal bool HasLine(int lineNumber) => Program.HasLine(lineNumber);

    internal void New() {
        Program.New();
    }

    internal void Renumber() {
    }

    internal bool Reset() {
        var _ = Program.Reset();
        return _ != null;
    }

    internal bool Reset(out ProgramLine? programLine) {
        programLine = Program.Reset();
        return programLine is not null;
    }

    internal bool Next(out ProgramLine programLine) {
        var tmp = Program.Next();

        if (tmp is not null) {
            programLine = tmp.Value;
            return true;
        }

        programLine = new();
        return false;
    }

    internal void End() {
        Program.End();
    }

    internal bool Goto(int lineNumber, out ProgramLine? programLine) {
        programLine = Program.Goto(lineNumber);
        return programLine is not null;
    }

    internal bool Gosub(int lineNumber, out ProgramLine? programLine) {
        programLine = Program.Gosub(lineNumber);
        return programLine is not null;
    }

    internal bool Return(out ProgramLine? programLine) {
        programLine = Program.Return();
        return programLine is not null;
    }

    internal void SetLine(int lineNumber, string[] tokens) {
        Program.SetLine(lineNumber, tokens);
    }

    internal string GetLine(int lineNumber) {
        return Program.GetLine(lineNumber);
    }

    internal void RemoveLine(int lineNumber) {
        Program.RemoveLine(lineNumber);
    }

    internal bool EnterForLoop(string identifier) {
        return Program.EnterForLoop(identifier);
    }

    internal bool ContinueForLoop(string identifier) {
        return Program.ContinueForLoop(identifier);
    }

    internal bool ExitForLoop(string identifier) {
        return Program.ExitForLoop(identifier);
    }

    internal bool IsForLoopActive(string identifier) {
        return Program.IsForLoopActive(identifier);
    }
}
