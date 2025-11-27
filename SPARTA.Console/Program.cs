using Azure;
using Azure.AI.Inference;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Reflection;
using System.Text.Json;

// ======================================================
// CONFIGURACIÓN
// ======================================================

// Azure OpenAI
const string modelName = "gpt-4o-mini";
const string endpoint = "https://hackaton2025violeta.openai.azure.com/";
const string apiKey = "7QHQSXyiI5xIyNlLzeyIZLh1d2bbjdS8upnaIWVi8OmGg2mBitfTJQQJ99BKACLArgHXJ3w3AAABACOGVIp6";

// MCP GitHub Copilot
const string mcpEndpoint = "https://api.githubcopilot.com/mcp/";
const string mcpAuthToken = "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";

const string defaultGitHubOwner = "mizura"; // Cambia esto por tu usuario o organización de GitHub




IChatClient GetChatClient()
{
    return new ChatCompletionsClient(
             endpoint: new Uri(endpoint),
             new AzureKeyCredential(apiKey)
             )
             .AsIChatClient(modelName);
}





//var azureClient = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

var headers = new Dictionary<string, string>
{
    { "Authorization", $"Bearer {mcpAuthToken}" }
};

var transport = new SseClientTransport(
    new SseClientTransportOptions
    {
        Name = "GitHubMCP",
        Endpoint = new Uri(mcpEndpoint),
        AdditionalHeaders = headers
    });

await using var mcp = await McpClientFactory.CreateAsync(transport);
var mcpTools = await mcp.ListToolsAsync();


IChatClient chatClient = GetChatClient();
var chatoptions = new ChatOptions
{
    Tools = [.. mcpTools],
    ModelId = modelName
};
var result = await chatClient.GetResponseAsync("Listame todos los repositorios", chatoptions);

// Crear opciones de chat
//var chatOptions = new ChatCompletionsOptions()
//{
//    Messages = { new ChatMessage(ChatRole.System, "asd") },
//    Temperature = 0.4f,
//    MaxTokens = 2000,
//    NucleusSamplingFactor = 0.95f,
//    FrequencyPenalty = 0,
//    PresencePenalty = 0,
//};

//var response = await azureClient.GetChatCompletionsAsync(modelName, chatOptions);
