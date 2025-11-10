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
            userService.LoadUserService();
            var menuHelper = new MenuHelper(userService);

            // Starta huvudprogrammet och skicka med UserService
            menuHelper.RunApp();
        }
    }
}
