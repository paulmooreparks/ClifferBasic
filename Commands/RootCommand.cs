using System.CommandLine.Invocation;
using System.CommandLine;
using Cliffer;

namespace ClifferBasic.Commands;

[RootCommand("Cliffer Basic")]
internal class RootCommand {
    public async Task<int> Execute(int lineNumber, Command command, IServiceProvider serviceProvider, InvocationContext context) {
        return await command.Repl(serviceProvider, context, new BasicReplContext());
    }
}

