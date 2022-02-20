using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TodoAppServer.Models
{
    public class AppUser : IdentityUser
    {
        public List<TodoItem> TodoItems { get; set; }

    }
}
