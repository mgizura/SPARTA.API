using Azure;
using Azure.AI.OpenAI;
using SPARTA.Service.Feature.History.Interfaces;
using System;


namespace SPARTA.Service.Feature.History.CaseUse
{

    public class HistoryService : IHistoryService
    {
        private const string modelName = "gpt-4o-mini";
        private const string endpoint = "https://hackaton2025violeta.openai.azure.com/";
        private const string apiKey = "   ";

        public HistoryService()
        {

        }


        public async Task<string> GenerateAsync(string prompt, bool hasTest, bool hasUpgrade)
        {
            prompt = $"'{prompt}'";
            if (hasTest && hasUpgrade)
            {
                prompt += " sobre esta historia que te paso necesito que me la mejores y me generes casos de test, Y NO QUIERO QUE ME ARROJES CODIGO DE PROGRAMACION";
            }
            else if (hasUpgrade)
            {
                prompt += " sobre esta historia que te paso, necesito que me la mejores, NO ME GENERES CASOS DE TEST, Y TAMPOCO CASOS PRACTICO EN NINGUN STACK TECNOLOGICO";
            }
            else if (hasTest)
            {
                prompt += "  sobre esta historia que te paso, necesito que me generes los casos de testcase posibles";
            }


            OpenAIClient client = new OpenAIClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey));

            ChatCompletionsOptions options = new ChatCompletionsOptions()
            {
                Messages = { new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information.") },
                Temperature = (float)0.7,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            while (true)
            {
                options.Messages.Add(new ChatMessage(ChatRole.User, prompt));

                Response<ChatCompletions> response =
                    await client.GetChatCompletionsAsync(
                    deploymentOrModelName: modelName,
                    options);

                ChatCompletions completions = response.Value;
                return completions.Choices[0].Message.Content;
            }
        }
    }
}

