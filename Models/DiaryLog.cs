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
        // foreign key representing the userID 
        public int UserId { get; set; }
        public string EventName { get; set; }
        public string StoryUrl { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public virtual ICollection<DiaryImage> Images { get; set; } // a navigation property that links the images to a log.

        // Navigation property that allows foreign key relations.
        // virtual for lazy loading
        public virtual Users User { get; set; }
    }
}
