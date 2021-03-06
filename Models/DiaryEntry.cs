﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearDiaryLogs.Models
{
    public class DiaryEntry
    {
        // A model representing what we get from the user during uploads
        public int UserID { get; set; }
        public string Event { get; set; }
        public string Story { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public IEnumerable<IFormFile> Images { get; set; }
    }
}
