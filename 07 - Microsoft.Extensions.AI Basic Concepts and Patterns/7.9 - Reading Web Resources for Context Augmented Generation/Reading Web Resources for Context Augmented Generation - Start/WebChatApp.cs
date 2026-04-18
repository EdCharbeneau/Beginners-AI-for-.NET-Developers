using System;
using HtmlAgilityPack;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace MyFirstChat;

public partial class WebChatApp(HttpClient httpClient, IChatClient ai, IHostApplicationLifetime lifetime) : BackgroundService
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

        ChatMessage systemMessage = new(ChatRole.System, summarizationPrompt);
        systemMessage.Contents.Add(new TextContent("number_of_sentences=4"));
        history.Add(systemMessage);
        ChatResponse response = await ai.GetResponseAsync(history);
        Console.WriteLine("AI: " + response.Text);
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