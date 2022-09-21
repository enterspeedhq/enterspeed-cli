using System.CommandLine;

namespace Enterspeed.Cli.Commands.Domain;

public static class DomainCommands
{
    public static Command BuildCommands()
    {
        var domain = new Command("domain", "Domain")
        {
            new GetDomainCommand(),
            new ListDomainsCommand(),
            new CreateDomainCommand(),
            new UpdateDomainCommand(),
            new DeleteDomainCommand()
        };
        return domain;
    }
}