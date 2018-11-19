using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearDiaryLogs.Models
{
    public class DiaryLog
    {
        // This is an Entity Model that represents the data in the database as an object, or in this case a Diary Log.
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventStory { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Url { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }
}
