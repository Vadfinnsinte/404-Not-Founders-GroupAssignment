using _404_not_founders.Menus;
using _404_not_founders.Models;
using _404_not_founders.Services;

namespace _404_not_founders
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create and initialize UserService (loads/reads JSON – once at startup)
            var userService = new UserService();
            var projectService = new ProjectService(userService);

            userService.LoadUserService();

            // Create the app and pass in userService so it can manage users and saving
            var runApp = new RunApp(userService, projectService);


            // Start the application's main loop
            runApp.Run();
        }
    }
}
