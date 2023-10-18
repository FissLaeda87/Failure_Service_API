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
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\baliv\\Desktop\\Новая папка\\Failure_Service\\events.db");
        }       
    }
}
