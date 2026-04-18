using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyFirstChat;

var host = Host.CreateApplicationBuilder(args);

//  Configure the host
host.Configuration.AddUserSecrets<Program>();

//  The Uri of your provider
var endpoint = host.Configuration["Chat:AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Missing configuration: Endpoint. See the README for details.");
//  The API Key for your provider
var apikey = host.Configuration["Chat:AzureOpenAI:Key"] ?? throw new InvalidOperationException("Missing configuration: ApiKey. See the README for details.");

var client = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureKeyCredential(apikey)
    );

string model = "gpt-4o-mini";
IChatClient innerClient = client.GetChatClient(model).AsIChatClient();

host.Services.AddChatClient(innerClient);
host.Services.AddHostedService<ChatApp>();

var app = host.Build();

//  Run the app
await app.RunAsync();