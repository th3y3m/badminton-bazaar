using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IVoskService
    {
        Task<string> ConvertSpeechToTextAsyncFromFile(IFormFile audio);
    }
}
