using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DearDiaryLogs.Models
{
    public class DearDiaryLogsContext : DbContext
    {
        // This coordinates the entity framework functionality.
        // It essentially allows the data in the database to be represented as objects rather than the rows/columns.
        // The Context is the bridge between the database and the model (entity class).
        public DearDiaryLogsContext (DbContextOptions<DearDiaryLogsContext> options)
            : base(options)
        {
        }

        public DbSet<DearDiaryLogs.Models.DiaryLog> DiaryLog { get; set; }
    }
}
