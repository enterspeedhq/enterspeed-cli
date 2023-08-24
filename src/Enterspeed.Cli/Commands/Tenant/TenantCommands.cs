using System.CommandLine;

namespace Enterspeed.Cli.Commands.Tenant;

public static class TenantCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "tenant", "t" }.Select(commandName => new Command(commandName, "Tenant")
        {
            new ListTenantsCommand(),
            new CurrentTenantCommand(),
            new SetActiveTenantCommand()
        }).ToArray();
    }
}