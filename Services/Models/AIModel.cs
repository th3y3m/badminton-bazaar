﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class ChatMessage
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class ChatRequest
    {
        public string model { get; set; }
        public List<ChatMessage> messages { get; set; }
        public double temperature { get; set; }
        public int max_tokens { get; set; }
    }

    public class ChatResponse
    {
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public ChatMessage message { get; set; }
    }

    public class GetResponseAsyncUsingLocalImageGenerationAIRequest
    {
        public string user_message { get; set; }
        public int steps { get; set; }
    }

    public class GetResponseAsyncUsingTextGenerationAIRequest
    {
        public string? session_id { get; set; } = "";
        public string user_message { get; set; }
    }

    public class ChatResponseWithConfig
    {
        public string session_id { get; set; }
        public string response { get; set; }
    }

    public class GoogleChatResponse
    {
        public string response { get; set; }
    }

    public class GoogleChatRequest
    {
        public string user_message { get; set; }
    }
}
