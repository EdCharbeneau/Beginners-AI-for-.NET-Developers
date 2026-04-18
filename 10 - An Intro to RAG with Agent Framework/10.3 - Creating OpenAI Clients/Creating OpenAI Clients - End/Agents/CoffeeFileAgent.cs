using MyFirstChatUI.Models;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Responses;
using OpenAI.VectorStores;

namespace MyFirstChatUI.Agents;

#pragma warning disable OPENAI001
public class CoffeeFileAgent
{
        // Client for interacting with the OpenAI Assistant API.
    private readonly ResponsesClient responsesClient;

    // Client for managing vector stores (document stores for embeddings).
    private readonly VectorStoreClient storeClient;

    // Client for uploading and managing files in OpenAI.
    private readonly OpenAIFileClient fileClient;

    // Service for accessing coffee data and file names.
    private readonly CoffeeData coffeeDataService;
    // Private constructor to prevent direct instantiation
    private CoffeeFileAgent(OpenAIClient azureOpenAIClient, CoffeeData coffeeDataService)
    {
        this.coffeeDataService = coffeeDataService;
        storeClient = azureOpenAIClient.GetVectorStoreClient();
        responsesClient = azureOpenAIClient.GetResponsesClient();
        fileClient = azureOpenAIClient.GetOpenAIFileClient();
    }

    // Static factory method to create an instance of CoffeeFileAgent
    public static CoffeeFileAgent CreateAsync(OpenAIClient azureOpenAIClient, CoffeeData coffeeDataService)
    {
        var agent = new CoffeeFileAgent(azureOpenAIClient, coffeeDataService);
        // await agent.InitializeAsync();
        return agent;
    }
}
#pragma warning restore OPENAI001