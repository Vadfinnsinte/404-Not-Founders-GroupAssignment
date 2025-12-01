using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;


namespace _404_not_founders.Menus
{
    // ----- The START/MAIN LOOP of the application -----
    public class RunApp
    {
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;
        private readonly GeminiAIService _aiService; // AI-tjänst
        // Constructor with dependency injection
        public RunApp(UserService userService, ProjectService projectService, GeminiAIService aiService)
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
                // If not logged in
                if (!loggedIn)
                {
                    // Show login/register menu
                    menuHelper.ShowLoginRegisterMenu(users, out loggedIn, out currentUser, ref running);

                    // Om man lyckats logga in eller registrera, sätt aktuell användare
                    if (loggedIn && currentUser != null)
                    {
                        // Get the logged-in user object and set it in the logged-in menu
                        var user = users.FirstOrDefault(u => u.Username == currentUser);
                        if (user != null)
                        {
                            loggedInMenu.SetCurrentUser(user); // Sätt aktuell användare i menyn
                            _currentUser = user;
                        }
                    }
                }
                // If logged in
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