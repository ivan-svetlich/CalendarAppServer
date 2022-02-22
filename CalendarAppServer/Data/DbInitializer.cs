using System.Linq;

namespace CalendarAppServer.Data
{
    public class DbInitializer
    {
        public static void Initialize(CalendarAppContext context)
        {
            context.Database.EnsureCreated();

            if (context.TodoItems.Any() || context.Users.Any())
            {
                return;
            }
        }
    }
}
