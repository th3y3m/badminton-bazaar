﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class TranscribeSpeechFromFileRequest
    {
        public IFormFile Audio { get; set; }
    }
}
