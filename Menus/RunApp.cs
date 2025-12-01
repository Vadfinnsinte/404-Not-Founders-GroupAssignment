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
        private readonly UserService _userService; // Användartjänst
        private User? _currentUser; // Aktuell inloggad användare
        private readonly ProjectService _projectService; // Projekttjänst
        private readonly GeminiAIService _aiService; // AI-tjänst

        public RunApp(UserService userService, ProjectService projectService, GeminiAIService aiService) // Konstruktör
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService)); // Kontrollera null
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService)); // Kontrollera null
            _aiService = aiService; // AI-tjänst
        }

        public async Task Run() // Huvudloop för appen
        {
            bool running = true; // Appen körs
            bool loggedIn = false; // Användaren är inte inloggad
            string currentUser = null; // Ingen aktuell användare

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
                    var result = await menuHelper.ShowLoginRegisterMenu(users); // Async anrop
                    loggedIn = result.loggedIn; // Uppdatera inloggningsstatus
                    currentUser = result.currentUser; // Uppdatera aktuell användare
                    running = result.running; // Uppdatera körstatus

                    // Om man lyckats logga in eller registrera, sätt aktuell användare
                    if (loggedIn && currentUser != null)
                    {
                        var user = users.FirstOrDefault(u => u.Username == currentUser); // Hitta användaren
                        if (user != null)
                        {
                            loggedInMenu.SetCurrentUser(user); // Sätt aktuell användare i menyn
                            _currentUser = user;
                        }
                    }
                }
                else
                {
                    // Meny för inloggat läge (t.ex. projekt, edit user)
                    var menuResult = await loggedInMenu.ShowLoggedInMenu(loggedIn, currentUser); // Async anrop
                    loggedIn = menuResult.loggedIn; // Uppdatera inloggningsstatus
                    currentUser = menuResult.currentUser; // Uppdatera aktuell användare

                    // Om utloggning ska _currentUser sättas till null också
                    if (!loggedIn)
                    {
                        _currentUser = null; // Ingen aktuell användare
                    }
                }
            }

            ConsoleHelpers.Info("Thank you for using the app, see you next time");
            ConsoleHelpers.Info("Closing down...");
            ConsoleHelpers.DelayAndClear();
        }
    }
}