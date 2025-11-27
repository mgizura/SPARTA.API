using Azure;
using Azure.AI.OpenAI;
using ModelContextProtocol.Client;
using System.Text.Json;
using System.Reflection;

// ======================================================
// CONFIGURACIÓN
// ======================================================

// Azure OpenAI
const string modelName = "gpt-4o-mini";
const string endpoint = "https://hackaton2025violeta.openai.azure.com/";
const string apiKey = "   ";

// MCP GitHub Copilot
const string mcpEndpoint = "https://api.githubcopilot.com/mcp/";
const string mcpAuthToken = "    ";

// Configuración por defecto para GitHub
// TODO: Reemplaza con tu usuario/organización de GitHub
const string defaultGitHubOwner = "mizura"; // Cambia esto por tu usuario o organización de GitHub

// ======================================================
// 1. Inicializar Azure OpenAI
// ======================================================
var azureClient = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

// ======================================================
// 2. Conectar al MCP y obtener tools disponibles
// ======================================================
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

// Obtener todas las tools disponibles del MCP
Console.WriteLine("🔍 Obteniendo tools disponibles del MCP...");

// Lista para almacenar información de las tools disponibles
var availableTools = new List<(string Name, string? Description)>();

try
{
    // Usar var para inferir el tipo de retorno de ListToolsAsync
    var mcpTools = await mcp.ListToolsAsync();

    // Intentar acceder directamente - puede que la propiedad tenga otro nombre
    var toolsEnumerable = mcpTools as System.Collections.IEnumerable;

    if (toolsEnumerable == null && mcpTools != null)
    {
        // Intentar acceder a propiedades comunes usando reflexión
        var type = mcpTools.GetType();
        Console.WriteLine($"📋 Tipo de resultado: {type.FullName}");

        // Mostrar todas las propiedades disponibles para debugging
        var properties = type.GetProperties();
        Console.WriteLine($"📋 Propiedades disponibles:");
        foreach (var prop in properties)
        {
            Console.WriteLine($"   - {prop.Name} ({prop.PropertyType.Name})");
        }

        var toolsProperty = type.GetProperty("Tools")
                         ?? type.GetProperty("Items")
                         ?? type.GetProperty("Data")
                         ?? type.GetProperty("Value");

        if (toolsProperty != null)
        {
            var toolsValue = toolsProperty.GetValue(mcpTools);
            toolsEnumerable = toolsValue as System.Collections.IEnumerable;
            Console.WriteLine($"✅ Encontrada propiedad: {toolsProperty.Name}");
        }
        else
        {
            Console.WriteLine("⚠️ No se encontró propiedad Tools, Items, Data o Value");
        }
    }

    if (toolsEnumerable != null)
    {
        foreach (var toolObj in toolsEnumerable)
        {
            try
            {
                // Usar dynamic para acceder a las propiedades
                dynamic tool = toolObj;
                string toolName = tool.Name ?? "unknown_tool";
                string? toolDesc = tool.Description;

                availableTools.Add((toolName, toolDesc));
                Console.WriteLine($"  ✓ {toolName}: {toolDesc ?? "Sin descripción"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ Error al procesar tool: {ex.Message}");
            }
        }
    }
    else
    {
        Console.WriteLine("⚠️ No se pudo obtener la lista de tools del MCP");
    }

    Console.WriteLine($"✅ Encontradas {availableTools.Count} tools disponibles\n");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error al obtener tools del MCP: {ex.Message}");
    Console.WriteLine($"   Tipo de error: {ex.GetType().Name}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Error interno: {ex.InnerException.Message}");
    }
    Console.WriteLine("⚠️ Continuando sin tools del MCP...\n");
}

Console.WriteLine();

// Opciones de chat base
var baseSystemMessage = @"Sos un asistente experto que combina Azure OpenAI + GitHub MCP. 
Puedes usar las tools disponibles del MCP para obtener información sobre repositorios, issues, historias, etc.
Cuando el usuario te pida algo que requiera información del MCP, usa las tools apropiadas.";

// ======================================================
// LOOP PRINCIPAL
// ======================================================
var conversationHistory = new List<ChatMessage>();

while (true)
{
    Console.Write("\nTu pregunta> ");
    string prompt = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(prompt))
        continue;

    // Analizar el prompt para determinar qué tool del MCP usar
    string? selectedTool = null;
    Dictionary<string, object?> toolArgs = new Dictionary<string, object?>();

    // Detectar palabras clave para seleccionar la tool apropiada
    var lowerPrompt = prompt.ToLower();

    if (lowerPrompt.Contains("historia") || lowerPrompt.Contains("historias") || lowerPrompt.Contains("issue") || lowerPrompt.Contains("issues"))
    {
        // Buscar tool relacionada con historias/issues
        selectedTool = availableTools.FirstOrDefault(t =>
            t.Name.ToLower().Contains("issue") ||
            t.Name.ToLower().Contains("historia") ||
            t.Description?.ToLower().Contains("issue") == true ||
            t.Description?.ToLower().Contains("historia") == true
        ).Name;

        if (string.IsNullOrEmpty(selectedTool))
        {
            // Intentar con list_issues como fallback
            selectedTool = availableTools.FirstOrDefault(t => t.Name.Contains("list") && t.Name.Contains("issue")).Name;
        }
    }
    else if (lowerPrompt.Contains("repositorio") || lowerPrompt.Contains("repos"))
    {
        selectedTool = availableTools.FirstOrDefault(t =>
            t.Name.ToLower().Contains("repo") ||
            t.Description?.ToLower().Contains("repo") == true
        ).Name;
    }

    // Si se detectó una tool, usar Azure OpenAI para extraer parámetros del prompt
    string mcpContext = "";
    if (!string.IsNullOrEmpty(selectedTool))
    {
        Console.WriteLine($"\n🔍 Analizando prompt para extraer parámetros de la tool: {selectedTool}");

        // Crear un prompt para Azure OpenAI que extraiga los parámetros necesarios
        var toolsList = string.Join(", ", availableTools.Select(t => $"{t.Name}: {t.Description ?? "Sin descripción"}"));
        var parameterExtractionPrompt = $@"Analiza el siguiente prompt del usuario y extrae los parámetros necesarios para ejecutar la tool '{selectedTool}' del MCP.

Prompt del usuario: ""{prompt}""

Tools disponibles:
{toolsList}

Responde SOLO con un JSON válido que contenga los parámetros necesarios. Por ejemplo, si la tool requiere 'owner' y 'repo', responde:
{{
  ""owner"": ""nombre_del_owner"",
  ""repo"": ""nombre_del_repo""
}}

Si no puedes determinar algún parámetro, usa null o una cadena vacía. Si el prompt menciona 'SPARTA.API' o 'SPARTA', asume que el repo es 'SPARTA.API'. Si no se menciona el owner, intenta inferirlo del contexto o usa un valor por defecto razonable.

Responde SOLO con el JSON, sin explicaciones adicionales:";

        try
        {
            // Llamar a Azure OpenAI para extraer parámetros
            var extractionOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "Eres un asistente que extrae parámetros de prompts en formato JSON. Responde SOLO con JSON válido, sin explicaciones."),
                    new ChatMessage(ChatRole.User, parameterExtractionPrompt)
                },
                Temperature = 0.1f,
                MaxTokens = 500,
            };

            var extractionResponse = await azureClient.GetChatCompletionsAsync(modelName, extractionOptions);
            var extractionResult = extractionResponse.Value.Choices[0].Message.Content?.Trim() ?? "{}";

            // Limpiar el resultado (puede venir con markdown code blocks)
            if (extractionResult.StartsWith("```json"))
            {
                extractionResult = extractionResult.Substring(7);
            }
            if (extractionResult.StartsWith("```"))
            {
                extractionResult = extractionResult.Substring(3);
            }
            if (extractionResult.EndsWith("```"))
            {
                extractionResult = extractionResult.Substring(0, extractionResult.Length - 3);
            }
            extractionResult = extractionResult.Trim();

            Console.WriteLine($"📝 Parámetros extraídos: {extractionResult}");

            // Deserializar los parámetros
            try
            {
                toolArgs = JsonSerializer.Deserialize<Dictionary<string, object?>>(extractionResult) ?? new Dictionary<string, object?>();

                // Si no se encontró owner pero hay repo, intentar inferir owner común
                if (!toolArgs.ContainsKey("owner") || toolArgs["owner"] == null ||
                    (toolArgs["owner"] is string ownerStr && string.IsNullOrWhiteSpace(ownerStr)))
                {
                    // Intentar extraer owner del prompt o usar valores por defecto
                    if (lowerPrompt.Contains("sparta") || toolArgs.ContainsKey("repo"))
                    {
                        toolArgs["owner"] = defaultGitHubOwner;
                        Console.WriteLine($"ℹ️ Usando owner por defecto: {defaultGitHubOwner}");
                    }
                }

                // Si hay repo pero no owner, usar el owner por defecto
                if (toolArgs.ContainsKey("repo") && toolArgs["repo"] != null)
                {
                    if (!toolArgs.ContainsKey("owner") || string.IsNullOrWhiteSpace(toolArgs["owner"]?.ToString()))
                    {
                        toolArgs["owner"] = defaultGitHubOwner;
                        Console.WriteLine($"ℹ️ Usando owner por defecto: {defaultGitHubOwner}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al parsear parámetros extraídos: {ex.Message}");
                Console.WriteLine($"   Usando parámetros vacíos");
                toolArgs = new Dictionary<string, object?>();
            }

            Console.WriteLine($"\n🔧 Ejecutando tool del MCP: {selectedTool}");
            Console.WriteLine($"📋 Con parámetros: {JsonSerializer.Serialize(toolArgs, new JsonSerializerOptions { WriteIndented = true })}");

            var mcpResponse = await mcp.CallToolAsync(selectedTool, toolArgs);
            var mcpResultJson = JsonSerializer.Serialize(
                mcpResponse,
                new JsonSerializerOptions { WriteIndented = true }
            );

            Console.WriteLine($"\n📦 Resultado del MCP:");
            Console.WriteLine(mcpResultJson);

            // Agregar el contexto del MCP al prompt
            mcpContext = $"\n\nInformación obtenida del MCP (tool: {selectedTool}):\n{mcpResultJson}\n\nUsa esta información para responder al usuario de manera clara y útil.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error al ejecutar la tool: {ex.Message}");
            Console.WriteLine($"   Tipo de error: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Error interno: {ex.InnerException.Message}");
            }
            mcpContext = $"\n\nError al obtener información del MCP: {ex.Message}. Por favor, proporciona los parámetros necesarios (como owner y repo) en tu pregunta.";
        }
    }
    else if (availableTools.Count > 0)
    {
        // Si no se detectó una tool específica pero hay tools disponibles,
        // mostrar las opciones disponibles
        var toolsList = string.Join(", ", availableTools.Select(t => t.Name));
        mcpContext = $"\n\nTools disponibles del MCP: {toolsList}";
    }

    // Crear el prompt final con el contexto del MCP
    string finalPrompt = prompt + mcpContext;

    // Agregar mensaje del usuario al historial
    conversationHistory.Add(new ChatMessage(ChatRole.User, finalPrompt));

    // Crear opciones de chat
    var chatOptions = new ChatCompletionsOptions()
    {
        Messages = { new ChatMessage(ChatRole.System, baseSystemMessage) },
        Temperature = 0.4f,
        MaxTokens = 2000,
        NucleusSamplingFactor = 0.95f,
        FrequencyPenalty = 0,
        PresencePenalty = 0,
    };

    // Agregar historial de conversación
    foreach (var msg in conversationHistory)
    {
        chatOptions.Messages.Add(msg);
    }

    // Enviar a Azure OpenAI
    try
    {
        var response = await azureClient.GetChatCompletionsAsync(modelName, chatOptions);
        var output = response.Value.Choices[0].Message.Content;

        Console.WriteLine("\n💬 RESPUESTA:");
        Console.WriteLine(output);

        // Agregar la respuesta del asistente al historial
        conversationHistory.Add(new ChatMessage(ChatRole.Assistant, output ?? ""));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error al llamar a Azure OpenAI: {ex.Message}");
        Console.WriteLine($"   Tipo de error: {ex.GetType().Name}");
    }
}
