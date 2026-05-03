using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MyFirstChatUI.Models;
using OpenAI;
using OpenAI.Chat;
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
    // The ID of the vector store created used for document search.
    private string vectorStoreId = null!;
    // Private constructor to prevent direct instantiation
    public ChatClientAgent Agent { get; private set; } = null!;
    private CoffeeFileAgent(OpenAIClient openAIClient, CoffeeData coffeeDataService)
    {
        this.coffeeDataService = coffeeDataService;
        storeClient = openAIClient.GetVectorStoreClient();
        responsesClient = openAIClient.GetResponsesClient();
        fileClient = openAIClient.GetOpenAIFileClient();
    }

    // Static factory method to create an instance of CoffeeFileAgent
    public static async Task<CoffeeFileAgent> CreateAsync(OpenAIClient openAIClient, CoffeeData coffeeDataService)
    {
        var agent = new CoffeeFileAgent(openAIClient, coffeeDataService);
        await agent.InitializeAsync();
        return agent;
    }

    public async Task<string> CreateNewStore()
    {
        var store = await storeClient.CreateVectorStoreAsync();
        if(store?.Value is not { Id: var storeId }) throw new Exception("Store was not created.");
        foreach (string fileName in coffeeDataService.GetMarkdownFileNames())
        {
            var fullPath = Path.Combine(coffeeDataService.DataPath, fileName);
            OpenAIFile fileInfo = await fileClient.UploadFileAsync(fullPath, FileUploadPurpose.Assistants);
            await storeClient.AddFileToVectorStoreAsync(storeId, fileInfo.Id);
        }
        return storeId;
    }

    private async Task InitializeAsync()
    {
        vectorStoreId = storeClient.GetVectorStores()?.FirstOrDefault()?.Id ?? string.Empty;
        if (string.IsNullOrEmpty(vectorStoreId))
        {
            vectorStoreId = await CreateNewStore();
        }

        // Create an Agent
string prompt = """
    The document store contains the text of coffee descriptions.
    Always analyze the document store to provide an answer to the user's question.
    Never rely on your knowledge not included in the document store.
    Always format response using markdown.
    """;

        Agent = responsesClient.AsAIAgent(
            model: "gpt-4o-mini",
            instructions: prompt, 
            name: "Coffee Agent", 
            description: "Coffee Search Agent", 
            tools: [new HostedFileSearchTool()
            {
                Inputs = [new HostedVectorStoreContent(vectorStoreId)]
            }]);
}
}
#pragma warning restore OPENAI001