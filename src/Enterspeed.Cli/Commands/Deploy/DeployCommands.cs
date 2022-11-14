using System.CommandLine;

namespace Enterspeed.Cli.Commands.Deploy
{
    public class DeployCommands
    {
        public static Command BuildCommands()
        {
            var command = new Command("deploy")
            {
                //new DeployCommand(),
                new ExtractCommand()
            };

            return command;
        }
    }
}