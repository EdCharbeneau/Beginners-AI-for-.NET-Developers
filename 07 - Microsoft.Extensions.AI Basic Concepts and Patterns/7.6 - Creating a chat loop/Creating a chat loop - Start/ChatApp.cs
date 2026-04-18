using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace MyFirstChat;

public class ChatApp(IChatClient ai, IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
    {
        ChatMessage systemMessage = new ChatMessage(ChatRole.System, "You are an AI assistant that tries to answer the user's query.");
        ChatResponse response = await ai.GetResponseAsync(systemMessage);
        Console.WriteLine("AI" + response.Text);
        lifetime.StopApplication();
    }
}