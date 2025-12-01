using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;

namespace _404_not_founders.Menus
{
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

            var users = _userService.Users;
            var menuHelper = new MenuHelper(_userService, _projectService);
            var loggedInMenu = new LoggedInMenu(_userService, _projectService);

            while (running)
            {
                if (!loggedIn)
                {
                    var menuResult = menuHelper.ShowLoginRegisterMenu(users, running);
                    loggedIn = menuResult.loggedIn;
                    currentUser = menuResult.currentUser;
                    running = menuResult.running;

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
                    var menuResult = await loggedInMenu.ShowLoggedInMenu(loggedIn, currentUser);
                    loggedIn = menuResult.loggedIn;
                    currentUser = menuResult.currentUser;

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
