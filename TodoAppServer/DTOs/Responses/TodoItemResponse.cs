using System;

namespace TodoAppServer.DTOs.Responses
{
    public class TodoItemResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public bool Removed { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
