using Microsoft.Extensions.DependencyInjection;
using Cliffer;
using ClifferBasic.Services;
using DIAttrib;
using System.Reflection;

namespace ClifferBasic;

internal class ClifferBasic {
    static async Task<int> Main(string[] args) {
        var cli = new ClifferBuilder()
            .ConfigureServices(services => {
                services.AddAttributedServices(Assembly.GetExecutingAssembly());
            })
            .Build();

        Utility.SetServiceProvider(cli.ServiceProvider);

        ClifferExitHandler.OnExit += () => {
        };

        return await cli.RunAsync(args);
    }
}

