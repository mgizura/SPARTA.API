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
// IMPORTANTE: El modelName debe coincidir exactamente con el nombre del deployment en Azure OpenAI
// Verifica en Azure Portal > Azure OpenAI > Deployments el nombre exacto de tu deployment
const string modelName = "gpt-4o-mini"; // Asegúrate de que este sea el nombre del deployment, no solo el modelo
const string endpoint = "https://hackaton2025violeta.openai.azure.com/";
const string apiKey = "7QHQSXyiI5xIyNlLzeyIZLh1d2bbjdS8upnaIWVi8OmGg2mBitfTJQQJ99BKACLArgHXJ3w3AAABACOGVIp6";

// MCP GitHub Copilot
const string mcpEndpoint = "https://api.githubcopilot.com/mcp/";
const string mcpAuthToken = "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";

const string defaultGitHubOwner = "mgizura"; // Cambia esto por tu usuario o organización de GitHub




IChatClient GetChatClient()
{
    return new ChatCompletionsClient(
                   endpoint: new Uri(endpoint),
                   new AzureKeyCredential(apiKey)
                   )
                     .AsIChatClient("xai/grok-3-mini")
                   .AsBuilder()
                   .UseFunctionInvocation()
                   .Build();
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
    ModelId = "xai/grok-3-mini"
};
var result = await chatClient.GetResponseAsync("Listame todos los repositorios", chatoptions);


