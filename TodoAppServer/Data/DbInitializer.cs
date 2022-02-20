using System.Linq;

namespace TodoAppServer.Data
{
    public class DbInitializer
    {
        public static void Initialize(TodoAppContext context)
        {
            context.Database.EnsureCreated();

            if (context.TodoItems.Any() || context.Users.Any())
            {
                return;
            }
        }
    }
}
