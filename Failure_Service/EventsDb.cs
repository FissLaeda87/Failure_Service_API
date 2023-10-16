using Microsoft.EntityFrameworkCore;

namespace Failure_Service
{
    public class EventsDb: DbContext
    {  
        public DbSet<Events> Events => Set<Events>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=events.db");
        }       
    }
}
