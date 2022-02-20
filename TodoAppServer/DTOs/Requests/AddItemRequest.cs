namespace TodoAppServer.DTOs.Requests
{
    public class AddItemRequest
    {
        public string Description { get; set; }
        public bool Completed { get; set; }
        public bool Removed { get; set; }
    }
}
