using ClifferBasic.Services;

namespace ClifferBasic.Model;

internal class ProgramModel {
    private LinkedList<ProgramLine> _lines = new ();
    private SortedDictionary<int, LinkedListNode<ProgramLine>> _lineIndex = new ();
    private readonly CommandSplitter _splitter;
    private Stack<int> _returnStack = new();
    private Dictionary<string, ForLoopState> _forMap = new();
    private LinkedListNode<ProgramLine>? Pointer { get; set; }
    private LinkedListNode<ProgramLine>? Current { get; set; }


    internal string[] Listing { 
        get {
            var lines = new List<string> ();
            var lineNumbers = _lineIndex.Keys.OrderBy(x => x).ToList();

            foreach (var lineNumber in lineNumbers) {
                lines.Add(GetLine(lineNumber));
            }

            return lines.ToArray();
        }
        set {
            foreach (var line in value) {
                var tokens = _splitter.Split(line).ToArray();
                if (tokens.Length > 1 && int.TryParse(tokens[0], out int lineNumber)) {
                    SetLine(lineNumber, tokens.Skip(1).ToArray());
                }
            }
        }
    }

    public ProgramModel(CommandSplitter splitter) { 
        _splitter = splitter;
    }

    internal void SetLine(int lineNumber, string[] tokens) {
        /*
        This logic is fairly ugly, but it's because I'm keeping a linked list of program lines and a separate map 
        of line numbers to linked-list nodes. The reasoning for that is because, in order to support goto, gosub and return, I need 
        to be able to jump to any programLine in the list and start serial execution from that point, but I also need to be able to quickly 
        insert new lines into the program while editing. So, I quickly implemented a VERY, VERY UGLY binary search that covers both 
        data structures and keeps them in synch. I'll come back around and clean this up later. Maybe.
        */
        if (_lineIndex.ContainsKey(lineNumber)) {
            var node = _lineIndex[lineNumber];
            node.ValueRef.Tokens = tokens;
        }
        else {
            if (_lines.Any()) {
                var chunk = _lineIndex.Chunk(_lineIndex.Count());
                var chunks = chunk.First().Chunk(Math.Max(1, (chunk.First().Count() / 2)) + 1);

                while (chunks is not null) {
                    var first = chunks.First();
                    var last = chunks.Count() > 1 ? chunks.Last() : null;

                    var linkedListNode = first.Last().Value;
                    var programLine = linkedListNode.Value;

                    if (first.Count() == 1) {
                        if (lineNumber < programLine.LineNumber) {
                            linkedListNode = _lines.AddBefore(linkedListNode, new ProgramLine() { LineNumber = lineNumber, Tokens = tokens });
                        }
                        else {
                            if (last is not null) {
                                linkedListNode = last.First().Value;

                                if (lineNumber > programLine.LineNumber) {
                                    linkedListNode = _lines.AddAfter(linkedListNode, new ProgramLine() { LineNumber = lineNumber, Tokens = tokens });
                                }
                            }
                            else {
                                linkedListNode = _lines.AddAfter(linkedListNode, new ProgramLine() { LineNumber = lineNumber, Tokens = tokens });
                            }
                        }

                        _lineIndex.Add(lineNumber, linkedListNode);
                        return;
                    }

                    if (lineNumber < programLine.LineNumber || last is null) {
                        chunks = first.Chunk(first.Count() / 2);
                    }
                    else {
                        chunks = last.Chunk(Math.Max((last.Count() / 2) + 1, 1));
                    }
                }
            }
            else {
                var linkedListNode = _lines.AddFirst(new ProgramLine() { LineNumber = lineNumber, Tokens = tokens });
                _lineIndex.Add(lineNumber, linkedListNode);
                return;
            }
        }
    }

    internal string GetLine(int lineNumber) {
        if (_lineIndex.TryGetValue(lineNumber, out LinkedListNode<ProgramLine>? line)) {
            return line.Value.ToString();
        }

        return string.Empty;
    }

    internal void RemoveLine(int lineNumber) {
        if (_lineIndex.ContainsKey(lineNumber)) {
            var node = _lineIndex[lineNumber];
            _lines.Remove(node);
            _lineIndex.Remove(lineNumber);
        }
    }

    internal bool HasLine(int lineNumber) => _lineIndex.ContainsKey(lineNumber);

    internal void New() { 
        _lines.Clear();
        _lineIndex.Clear();
    }

    internal void Renumber() {
    }

    internal ProgramLine? Reset() {
        Pointer = _lines.First;
        Current = null;
        return Pointer?.Value;
    }

    internal ProgramLine? Next() {
        if (Pointer == null) {
            return null;
        }

        Current = Pointer;
        Pointer = Pointer.Next;
        return Current.Value;
    }

    internal void End() {
        Pointer = null;
    }

    internal ProgramLine? Goto(int lineNumber) {
        if (HasLine(lineNumber)) {
            Pointer = _lineIndex[lineNumber];
            return Pointer?.Value;
        }
        else if (lineNumber == int.MaxValue) {
            End();
            return new ProgramLine();
        }

        return null;
    }

    internal ProgramLine? Gosub(int lineNumber) {
        if (Current is not null && HasLine(lineNumber)) {
            if (Current.Next is null) {
                _returnStack.Push(int.MaxValue);
            }
            else {
                var returnLineNumber = Current.Next.Value.LineNumber;
                _returnStack.Push(returnLineNumber);
            }

            return Goto(lineNumber);
        }

        return null;
    }

    internal ProgramLine? Return() {
        if (_returnStack.Any()) {
            int lineNumber = _returnStack.Pop();
            return Goto(lineNumber);
        }

        return null;
    }

    internal bool EnterForLoop(string identifier) {
        _forMap.Remove(identifier);

        if (Current is not null) {
            int lineNumber = Current.Value.LineNumber;
            _forMap.Add(identifier, new ForLoopState(identifier, lineNumber));
            return true;
        }

        return false;
    }

    internal bool ContinueForLoop(string identifier) {
        if (_forMap.TryGetValue(identifier, out var state)) {
            if (Current is not null) {
                if (Current.Next is null) {
                    state.EndLineNumber = int.MaxValue;
                }
                else {
                    state.EndLineNumber = Current.Next.Value.LineNumber;
                }

                Goto(state.StartLineNumber);
                return true;
            }
        }

        return false;
    }
    internal bool ExitForLoop(string identifier) {
        if (_forMap.TryGetValue(identifier, out var state)) {
            _forMap.Remove(identifier);
            Goto(state.EndLineNumber);
            return true;
        }

        return false;
    }

    internal bool IsForLoopActive(string identifier) {
        if (_forMap.TryGetValue(identifier, out var state)) {
            return state.IsActive;
        }

        return false;
    }
}

internal struct ProgramLine {
    internal int LineNumber { get; set; }
    internal string[] Tokens { get; set; }

    public override string ToString() {
        string line = string.Join(" ", Tokens);
        return $"{LineNumber} {line}";
    }
}

internal class ForLoopState {
    public string Identifier { get; set; }
    internal int StartLineNumber { get; set; }
    internal int EndLineNumber { get; set; } = int.MaxValue;
    internal bool IsActive { get; set; }

    public ForLoopState(string identifier, int lineNumber) {
        Identifier = identifier;
        StartLineNumber = lineNumber;
        IsActive = true;
    }
}

