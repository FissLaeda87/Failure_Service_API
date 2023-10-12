namespace Failure_Service
{
    public class Event
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }

        public bool IsArchived { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
