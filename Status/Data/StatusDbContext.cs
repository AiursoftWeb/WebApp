using Aiursoft.Status.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Aiursoft.Status.Data
{
    public class StatusDbContext : DbContext
    {
        public StatusDbContext(DbContextOptions<StatusDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<MonitorRule> MonitorRules { get; set; }

        public void Seed()
        {
            var existingData = MonitorRules.ToList();
            foreach (var item in SeedData.GetRules())
            {
                if (!existingData.Exists(t => t.ProjectName == item.ProjectName))
                {
                    MonitorRules.Add(item);
                }
            }
            SaveChanges();
        }
    }
}
