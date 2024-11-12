using Microsoft.Extensions.Configuration;
using Services.Interface;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["HuggingFace:Token"];
            _apiUrl = configuration["HuggingFace:URL"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetResponseAsyncUsingGoogleFlanT5Large(string userMessage)
        {
            try
            {

                var requestBody = new { inputs = userMessage };

                var response = await _httpClient.PostAsJsonAsync(_apiUrl, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(result);
                    return jsonDoc.RootElement[0].GetProperty("generated_text").GetString();
                }
                else
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetResponseAsyncUsingGoogleFlanT5Large method", ex);
            }
        }
    }
}
