using MyFirstChatUI.Models;
using OpenAI;
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
    }
}
#pragma warning restore OPENAI001