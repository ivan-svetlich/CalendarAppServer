using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAppServer.DTOs.Responses
{
    public class IntervalResponse
    {
        public DateTime DueDate { get; set; }
        public List<TodoItemResponse> TodoItems { get; set; }
    }
}
