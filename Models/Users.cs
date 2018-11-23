using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearDiaryLogs.Models
{
    public class Users
    {
        // A model representing user information in a table

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // One user can have many entries

        public ICollection<DiaryLog> DiaryLogs { get; set; }
    }
}
