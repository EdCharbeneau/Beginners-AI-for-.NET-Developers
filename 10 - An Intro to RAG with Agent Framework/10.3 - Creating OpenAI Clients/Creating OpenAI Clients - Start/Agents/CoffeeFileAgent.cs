using MyFirstChatUI.Models;
using OpenAI;

namespace MyFirstChatUI.Agents;

public class CoffeeFileAgent
{
    // Private constructor to prevent direct instantiation
    private CoffeeFileAgent(OpenAIClient openAIClient, CoffeeData coffeeDataService)
    {
    }

    // Static factory method to create an instance of CoffeeFileAgent
    public static CoffeeFileAgent CreateAsync(OpenAIClient openAIClient, CoffeeData coffeeDataService)
    {
        var agent = new CoffeeFileAgent(openAIClient, coffeeDataService);
        // await agent.InitializeAsync();
        return agent;
    }
}