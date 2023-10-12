using Microsoft.EntityFrameworkCore;

namespace Failure_Service
{
    public class EventsDb: DbContext
    {
        public EventsDb(DbContextOptions<EventsDb> options)
        : base(options) { }

        public DbSet<Event> Events => Set<Event>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=events.db");
        }
    }
}
