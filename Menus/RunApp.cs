using _404_not_founders.Models;         // User, Project, Character, World, Storyline
using _404_not_founders.Services;       // UserService, ProjectService
using _404_not_founders.UI;             // ConsoleHelpers, MenuChoises
using System;                           // Console, ArgumentNullException
using System.Collections.Generic;       // List
using System.Linq;                      // FirstOrDefault
using System.Text;                      // StringBuilder
using System.Threading.Tasks;           // Async/await

namespace _404_not_founders.Menus
{
    // ----- APPENS START/HUVUDLOOP -----
    public class RunApp
    {
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;
        private readonly GeminiAIService _aiService;

        public RunApp(UserService userService, ProjectService projectService, GeminiAIService aiService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _aiService = aiService;
        }

        public async Task Run()
        {
            bool running = true;
            bool loggedIn = false;
            string currentUser = null;

            // Lista över alla användare
            var users = _userService.Users;
            // Meny-helpers och menyn för inloggat läge
            var menuHelper = new MenuHelper(_userService, _projectService);
            var loggedInMenu = new LoggedInMenu(_userService, _projectService);

            while (running)
            {
                if (!loggedIn)
                {
                    // Meny för login/register, tuple-retur
                    var result = await menuHelper.ShowLoginRegisterMenu(users);
                    loggedIn = result.loggedIn;
                    currentUser = result.currentUser;
                    running = result.running;

                    // Om man lyckats logga in eller registrera, sätt aktuell användare
                    if (loggedIn && currentUser != null)
                    {
                        var user = users.FirstOrDefault(u => u.Username == currentUser);
                        if (user != null)
                        {
                            loggedInMenu.SetCurrentUser(user);
                            _currentUser = user;
                        }
                    }
                }
                else
                {
                    // Meny för inloggat läge (t.ex. projekt, edit user)
                    var menuResult = await loggedInMenu.ShowLoggedInMenu(loggedIn, currentUser);
                    loggedIn = menuResult.loggedIn;
                    currentUser = menuResult.currentUser;

                    // Om utloggning ska _currentUser sättas till null också
                    if (!loggedIn)
                    {
                        _currentUser = null;
                    }
                }
            }

            ConsoleHelpers.Info("Thank you for using the app, see you next time");
            ConsoleHelpers.Info("Closing down...");
            ConsoleHelpers.DelayAndClear();
        }
    }
}