using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CalendarAppServer.Models
{
    public class AppUser : IdentityUser
    {
        public List<TodoItem> TodoItems { get; set; }

    }
}
