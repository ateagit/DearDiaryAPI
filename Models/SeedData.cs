using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DearDiaryLogs.Models
{
    public class SeedData
    {
        // Static methods are not associated with any instance of a class, it is just part of a class.
        public static void Initialize(IServiceProvider serviceProvider) 
        {
            // using statement initializes the parameter, and executes the scope in a try block, calling the destructor afterwards.
            // in this case, creates an instance of the DearDiaryLogsContext with the constructor being something pulled from the services DI container.
            using (var context = new DearDiaryLogsContext(serviceProvider.GetRequiredService<DbContextOptions<DearDiaryLogsContext>>()))
            {
                if(context.DiaryLog.Count() > 0) // Sees how many entries in the DbSet IEnumerator?
                {
                    Console.WriteLine(context.GetType());
                    return;
                }
                else
                { // If there are no diary log entry, add a seed dummy entry so the db knows how to format the cols/rows
                    context.DiaryLog.AddRange(
                            new DiaryLog
                            {
                                EventName = "MSA time",
                                EventStory = "During this time I did nothing",
                                StartTime = "19/10/2018 10:09:52 PM",
                                EndTime = "19/10/2018 10:09:53 PM",
                                Url = "https://example.com/url-to-progress-pic-img.jpg",
                                Width = "680",
                                Height = "680"
                            }
                        );

                    context.SaveChanges();
                }
                
            }
        }
    }
}
