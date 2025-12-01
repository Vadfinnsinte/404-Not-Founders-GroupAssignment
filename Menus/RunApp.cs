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
        // Constructor with dependency injection
        public RunApp(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        public void Run()
        {
            bool running = true;
            bool loggedIn = false;
            string currentUser = null;

            var users = _userService.Users;
            var menuHelper = new MenuHelper(_userService, _projectService);
            var loggedInMenu = new LoggedInMenu(_userService, _projectService);


            while (running)
            {
                // If not logged in
                if (!loggedIn)
                {
                    // Show login/register menu
                    menuHelper.ShowLoginRegisterMenu(users, out loggedIn, out currentUser, ref running);

                    if (loggedIn && currentUser != null)
                    {
                        // Get the logged-in user object and set it in the logged-in menu
                        var user = users.FirstOrDefault(u => u.Username == currentUser);
                        if (user != null)
                            loggedInMenu.SetCurrentUser(user);
                    }
                }
                // If logged in
                else
                {
                    loggedInMenu.ShowLoggedInMenu(ref loggedIn, ref currentUser);
                }
            }

            ConsoleHelpers.Info("Thank you for using the app, see you next time");
            ConsoleHelpers.Info("Closing down...");
            ConsoleHelpers.DelayAndClear();
        }
    }
}
