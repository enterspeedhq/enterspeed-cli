using System.CommandLine;

namespace Enterspeed.Cli.Commands.Deploy
{
    public class DeployCommands
    {
        public static Command[] BuildCommands()
        {
            return new[] { "deployment", "dp" }.Select(commandName => new Command(commandName)
            {
                new DeployCommand(),
                new ExtractCommand()
            }).ToArray();
        }
    }
}