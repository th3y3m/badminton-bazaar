using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAIService
    {
        Task<string> GetResponseAsyncUsingLocalTextGenerationAI(string userMessage);
        Task<string> GetResponseAsyncUsingLocalImageGenerationAI(GetResponseAsyncUsingLocalImageGenerationAIRequest requestBody);
        Task<ChatResponseWithConfig> GetResponseAsyncUsingLocalTextGenerationAIWithConfig(GetResponseAsyncUsingTextGenerationAIRequest request);
        Task<GoogleChatResponse> GetResponseAsyncUsingGoogleAI(GoogleChatRequest request);
        Task<GoogleChatResponse> GetResponseAsyncUsingGoogleAIAndDb(GoogleChatRequest request);
    }
}
