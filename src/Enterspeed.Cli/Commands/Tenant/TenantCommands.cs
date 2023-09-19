using System.CommandLine;

namespace Enterspeed.Cli.Commands.Tenant;

public static class TenantCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("tenant", "Tenant")
        {
            new ListTenantsCommand(),
            new CurrentTenantCommand(),
            new SetActiveTenantCommand()
        };
        command.AddAlias("t");
        return command;
    }
}