// dotnet add package Azure.AI.OpenAI
// dotnet add package Microsoft.Extensions.AI
// dotnet add package Microsoft.Extensions.AI.OpenAI
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using MyFirstChatUI.Agents;
using MyFirstChatUI.Components;
using MyFirstChatUI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents(
	options =>
	{
		// Avoid enabling in production due to sensitive info in error details.
		options.DetailedErrors = builder.Environment.IsDevelopment();
	}
);

// 🌐 The Uri of your provider
var endpoint = builder.Configuration["Chat:AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Missing configuration: Endpoint. See the README for details.");
// 🔑 The API Key for your provider
var apikey = builder.Configuration["Chat:AzureOpenAI:Key"] ?? throw new InvalidOperationException("Missing configuration: ApiKey. See the README for details.");
// 🧠 The model name or azure deployment name
var model = "gpt-4o-mini";

var client = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureKeyCredential(apikey)
    );

IChatClient innerClient = client.GetChatClient(model).AsIChatClient();

builder.Services.AddSingleton(client);
builder.Services.AddChatClient(innerClient).UseFunctionInvocation().UseLogging();

// Register CoffeeData service
builder.Services.AddScoped<CoffeeData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();


app.Run();
