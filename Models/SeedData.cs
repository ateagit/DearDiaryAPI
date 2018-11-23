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
                int seedId = 0;
                if(context.Users.Count() == 0) // Check how many entries in DbSet (collection of objects locally)
                {
                    Users seedValues = new Users
                    {
                        Username = "randUser",
                        Password = "randPass"
                    };

                    // Create an instance of users reprenting one user.

                    // Add to context (local memory)
                    context.Add(seedValues);

                    // Save (push) to db
                    context.SaveChanges();

                    seedId = seedValues.Id;
                }
                if(context.DiaryLog.Count() == 0) // Sees how many entries in the DbSet
                {
                    DiaryLog seedValues = new DiaryLog
                    {
                        EventName = "MSA time",
                        UserId = seedId,
                        StoryUrl = "During this time I did nothing",
                        StartTime = "19/10/2018 10:09:52 PM",
                        EndTime = "19/10/2018 10:09:53 PM",
                    };

                    context.DiaryLog.Add(seedValues);
                    context.SaveChanges();

                    seedId = seedValues.Id;
                }
                if (context.DiaryImage.Count() == 0)
                {
                    DiaryImage seedValues = new DiaryImage
                    {
                        EntryId = seedId,
                        ImageURL = "https://example.com/url-to-progress-pic-img.jpg",
                        Width = "10",
                        Height = "20"
                    };

                    context.DiaryImage.Add(seedValues);
                    context.SaveChanges();
                }
                return;
            }
        }
    }
}
