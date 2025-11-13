using _404_not_founders.Menus;
using _404_not_founders.Models;
using _404_not_founders.Services;

namespace _404_not_founders
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Skapa och initiera UserService (laddar/läser JSON - en gång vid start)
            var userService = new UserService();
            var projectService = new ProjectService(userService);

            userService.LoadUserService();

            // Skapa huvudmenyn och skicka med userService så att den kan hantera användare och sparning
            var menuHelper = new MenuHelper(userService, projectService);
                

            // Starta applikationens huvudloop
            menuHelper.RunApp();
        }
    }
}
