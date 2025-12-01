using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;

namespace _404_not_founders.Menus
{
    /// Main application controller
    /// Manages the application lifecycle and navigation between login and main menu
    public class RunApp
    {
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;
        private readonly GeminiAIService _aiService;

        /// Constructor with dependency injection
        /// Initializes all required services
        public RunApp(UserService userService, ProjectService projectService, GeminiAIService aiService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _aiService = aiService;
        }

        /// Main application loop
        /// Handles navigation between login screen and logged-in menu
        /// Continues until user exits the application
        public async Task Run()
        {
            bool running = true;        // Application is running
            bool loggedIn = false;      // User login status
            string currentUser = null;  // Currently logged-in username

            // Get user list from service
            var users = _userService.Users;

            // Initialize menu controllers
            var menuHelper = new MenuHelper(_userService, _projectService);
            var loggedInMenu = new LoggedInMenu(_userService, _projectService);

            // Main application loop
            while (running)
            {
                // Show login/register menu when not logged in
                if (!loggedIn)
                {
                    // Display authentication menu
                    var menuResult = menuHelper.ShowLoginRegisterMenu(users, running);
                    loggedIn = menuResult.loggedIn;
                    currentUser = menuResult.currentUser;
                    running = menuResult.running;

                    // Set current user in logged-in menu if login/registration was successful
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
                // Show main menu when logged in
                else
                {
                    // Display logged-in user menu
                    var menuResult = await loggedInMenu.ShowLoggedInMenu(loggedIn, currentUser);
                    loggedIn = menuResult.loggedIn;
                    currentUser = menuResult.currentUser;

                    // Clear current user on logout
                    if (!loggedIn)
                    {
                        _currentUser = null;
                    }
                }
            }

            // Exit message
            ConsoleHelpers.Info("Thank you for using the app, see you next time");
            ConsoleHelpers.Info("Closing down...");
            ConsoleHelpers.DelayAndClear();
        }
    }
}
