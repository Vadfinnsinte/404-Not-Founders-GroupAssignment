using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.Menus
{
    // ----- APPENS START/HUVUDLOOP -----
    public class RunApp
    {
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;
        public RunApp(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        public void Run()
        {
            bool running = true, loggedIn = false;
            string currentUser = null;
            var users = _userService.Users;
            var menuHelper = new MenuHelper(_userService, _projectService);
            while (running)
            {
                if (!loggedIn)
                    menuHelper.ShowLoginRegisterMenu(users, out loggedIn, out currentUser, ref running);
                else
                    menuHelper.ShowLoggedInMenu(ref loggedIn, ref currentUser, ref running);
            }

            ConsoleHelpers.Info("Thank you for using the app, see you next time");
            ConsoleHelpers.Info("Closing down...");
            ConsoleHelpers.DelayAndClear();
        }
    }
}
