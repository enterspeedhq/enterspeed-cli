﻿using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;

namespace Enterspeed.Cli.Commands;

public class BaseCommandHandler
{
    public OutputStyle Output { get; set; }

    public bool Yes { get; set; }

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

    protected async Task<GetEnvironmentsResponse> GetEnvironmentToDeployTo(string environmentName, IMediator mediator)
    {
        var environments = await mediator.Send(new GetEnvironmentsRequest());
        var environmentToDeployTo = environments.FirstOrDefault(e => e.Name == environmentName);
        return environmentToDeployTo;
    }
}