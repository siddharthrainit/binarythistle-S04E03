using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext? context, bool isProduction)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if(isProduction)
            {
                Console.WriteLine("Attempty to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Could not run migration");
                    Console.WriteLine(ex.ToString());
                }
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("---- Seeding data ...");
                context.Platforms.AddRange(new Platform()
                {
                    Name = "dot net",
                    Publisher = "Microsoft",
                    Cost = "Free",
                    ExternalId = Guid.NewGuid(),
                }, new Platform()
                {
                    Name = "Sql Server Express",
                    Publisher = "Microsoft",
                    Cost = "Free",
                    ExternalId = Guid.NewGuid(),
                }, new Platform()
                {
                    Name = "Kubernetes",
                    Publisher = "Cloud Native Computing Foundation",
                    Cost = "Free",
                    ExternalId = Guid.NewGuid(),
                });

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("---- We already have data");
            }

        }
    }
}
