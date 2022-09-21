using System.CommandLine;

namespace Enterspeed.Cli.Commands
{
    public  class ConfirmCommand : Command
    {
        public ConfirmCommand(string name, string description) : base(name, description)
        {
            AddOption(new Option<bool>(new[] {"-y", "--yes"}, "Do not prompt for confirmation."));
        }
    }
}
