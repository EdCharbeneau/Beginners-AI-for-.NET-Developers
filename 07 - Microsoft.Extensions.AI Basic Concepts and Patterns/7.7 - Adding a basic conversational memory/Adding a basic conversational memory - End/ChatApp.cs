using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace MyFirstChat;

public class ChatApp(IChatClient ai, IHostApplicationLifetime lifetime) : BackgroundService
{
    
    private static bool exitRequested = false;
    List<ChatMessage> history = [];
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nCtrl+C detected. Exiting gracefully...");
            e.Cancel = true; // Prevent the process from terminating.
            lifetime.StopApplication();
            exitRequested = true;
        };


        ChatMessage systemMessage = new ChatMessage(ChatRole.System, "You are an AI assistant that tries to answer the user's query.");
        history.Add(systemMessage);
        ChatResponse response = await ai.GetResponseAsync(systemMessage);
        Console.WriteLine("AI" + response.Text);
        while (stoppingToken.IsCancellationRequested == false)
        {
            Console.Write("Prompt > ");
            string? userMessage = Console.ReadLine();
            if (userMessage == null || exitRequested)
                break;
            history.Add(new ChatMessage(ChatRole.User, userMessage));
            ChatResponse chatResponse = await ai.GetResponseAsync(history);
            history.AddMessages(chatResponse);
            foreach (var msg in chatResponse.Messages)
            {
                Console.WriteLine($"{msg.Role}: {msg.Text}");
            }
        }
    }
}