using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace MyFirstChat;

public class ChatApp(IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) 
    {
        Console.WriteLine("You are an AI assistant that tries to answer the user's query.");
        lifetime.StopApplication();
        return Task.CompletedTask;
    }
}