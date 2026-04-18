// dotnet add package Azure.AI.OpenAI
// dotnet add package Microsoft.Extensions.AI
// dotnet add package Microsoft.Extensions.AI.OpenAI
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
