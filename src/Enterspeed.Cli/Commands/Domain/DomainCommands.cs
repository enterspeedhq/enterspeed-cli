using System.CommandLine;

namespace Enterspeed.Cli.Commands.Domain;

public static class DomainCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "domain", "d" }.Select(commandName => new Command(commandName, "Domain")
        {
            new GetDomainCommand(),
            new ListDomainsCommand(),
            new CreateDomainCommand(),
            new UpdateDomainCommand(),
            new DeleteDomainCommand()
        }).ToArray();
        
    }
}