using Microsoft.EntityFrameworkCore;

namespace Failure_Service
{
    public class EventsDb: DbContext
    {  
        public DbSet<Events> Events => Set<Events>();
        public EventsDb()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\makus\\source\\repos\\Failure_Service_API\\Failure_Service\\events.db");
        }       
    }
}
