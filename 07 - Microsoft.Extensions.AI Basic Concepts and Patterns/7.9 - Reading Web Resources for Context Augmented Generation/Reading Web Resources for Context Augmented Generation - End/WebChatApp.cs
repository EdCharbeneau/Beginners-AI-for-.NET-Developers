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

        string data = await httpClient.GetStringAsync("https://dometrain.com/course/from-zero-to-hero-working-with-null-in-csharp/");
        // Parse HTML and extract text from <p> tags
        var doc = new HtmlDocument();
        doc.LoadHtml(data);
        var paragraphNodes = doc.DocumentNode.SelectNodes("//p");
        string paragraphData = string.Empty;

        if (paragraphNodes != null)
        {
            paragraphData = string.Join(" ", paragraphNodes.Select(node => node.InnerText.Trim()));
        }

        history.Add(new ChatMessage(ChatRole.Assistant, paragraphData));

        ChatResponse response = await ai.GetResponseAsync(history);

        Console.WriteLine("--------------------------------");
        Console.WriteLine("AI: " + response.Text);
        Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");

        history.AddMessages(response);

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