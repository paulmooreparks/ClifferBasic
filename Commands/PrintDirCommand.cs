using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cliffer;

namespace ClifferBasic.Commands;

[Command("printdir", "Print the name of the current directory")]
internal class PrintDirCommand {
    public int Execute() {
        var currentDirectory = Directory.GetCurrentDirectory();
        Console.WriteLine(currentDirectory);
        return Result.Success;
    }
}
