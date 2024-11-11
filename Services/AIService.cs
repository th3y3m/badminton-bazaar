using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Services.Interface;
using Services.Models;
using System.Net.Http.Json;

namespace Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["LocalAI:URL"];
        }

        public async Task<string> GetResponseAsyncUsingLocalAI(string userMessage)
        {
            try
            {
                var messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        role = "system",
                        content = "You're a helpful chat bot. Answer short and concise in 150 tokens only."
                    },
                    new ChatMessage
                    {
                        role = "user",
                        content = userMessage
                    }
                };

                var request = new ChatRequest
                {
                    messages = messages,
                    temperature = 0.9,
                    max_tokens = 150
                };

                var response = await _httpClient.PostAsJsonAsync(_apiUrl, request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                return result?.choices?.FirstOrDefault()?.message?.content ?? "No response received";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
