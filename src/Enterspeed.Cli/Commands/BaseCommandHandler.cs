using System.CommandLine.Invocation;

namespace Enterspeed.Cli.Commands;

public class BaseCommandHandler
{
    public OutputStyle Output { get; set; }
    public bool Yes { get; set; }

    public BaseCommandHandler()
    {
        
    }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public bool GetConfirmation()
    {
        if (Yes) return true;

        Console.WriteLine("Are you sure?");
        var confirm = Console.ReadLine();
        if (confirm != null && (confirm.ToLower() == "y" || confirm.ToLower() == "yes")) 
            return true;
        return false;
    }
}