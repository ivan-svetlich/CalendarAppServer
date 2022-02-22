using System;

namespace TodoAppServer.DTOs.Requests
{
    public class UpdateItemRequest
    {
        public string Description { get; set; }
        public bool Completed { get; set; }
        public bool Removed { get; set; }
        public DateTime DueDate { get; set; }
    }
}
