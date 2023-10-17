namespace Failure_Service
{
    public class Events
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? status { get; set; }

        public bool isArchived { get; set; }
        public string? message { get; set; }
        public DateTime timestamp { get; set; }
    }
}
