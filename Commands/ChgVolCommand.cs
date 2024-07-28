using Cliffer;

using ClifferBasic.Model;
using ClifferBasic.Services;

namespace ClifferBasic.Commands;

[Command("chgvol", "Change the current file-system volume")]
[Argument(typeof(string), "volume", "The volume to change to")]
internal class ChgVolCommand {
    public int Execute(
        string volume,        
        SyntaxParser syntaxParser,
        VariableStore variableStore,
        ProgramService programService
        ) {

        var element = syntaxParser.ParseArg(volume);

        try {
            if (element is BasicExpression expression) {
                var eval = expression.Evaluate(variableStore);

                if (eval is not null) {
                    var volumeName = eval.ToString();

                    if (!string.IsNullOrEmpty(volumeName)) {
                        if (OperatingSystem.IsWindows()) {
                            if (volumeName.Length == 1 && char.IsLetter(volumeName[0])) {
                                volumeName = $"{volumeName.ToUpperInvariant()}:\\";

                                if (Directory.Exists(volumeName)) {
                                    Directory.SetCurrentDirectory(volumeName);
                                    return Result.Success;
                                }
                                else {
                                    Console.Error.WriteLine($"Error: Volume '{volumeName}' does not exist.");
                                    return Result.Error;
                                }
                            }
                        }
                        else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) {
                            if (Directory.Exists(volumeName)) {
                                Directory.SetCurrentDirectory(volumeName);
                                return Result.Success;
                            }
                            else {
                                Console.Error.WriteLine($"Error: Volume '{volumeName}' does not exist.");
                                return Result.Error;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception) {
            // Optional: Route this to debug logging if necessary
        }

        Console.Error.WriteLine($"Error: Invalid volume '{volume}'.");
        return Result.Error;
    }
}
