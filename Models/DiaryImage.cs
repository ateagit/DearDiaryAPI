using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearDiaryLogs.Models
{
    public class DiaryImage
    {
        public int Id { get; set; } // Primary Key
        public int EntryId { get; set; } // Foreign key
        public string ImageURL { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public virtual DiaryLog Entry { get; set; } // Navigation property allows navigation from one end of an association to the other end.
    }
}
