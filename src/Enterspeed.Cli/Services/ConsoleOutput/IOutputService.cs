﻿namespace Enterspeed.Cli.Services.ConsoleOutput;

public interface IOutputService
{
    void Write<T>(T value);
}