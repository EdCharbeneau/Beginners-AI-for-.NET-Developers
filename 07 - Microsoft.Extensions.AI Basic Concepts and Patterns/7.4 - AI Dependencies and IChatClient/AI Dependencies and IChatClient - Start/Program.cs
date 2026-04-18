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

host.Services.AddHostedService<ChatApp>();

var app = host.Build();

Console.WriteLine($"{endpoint} - {apikey}");
