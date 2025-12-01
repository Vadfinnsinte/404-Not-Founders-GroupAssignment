using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;


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
                    // Visa login/register
                    menuHelper.ShowLoginRegisterMenu(users, out loggedIn, out currentUser, ref running);

                    if (loggedIn && currentUser != null)
                    {
                        // Hämta User-objektet och sätt det i LoggedInMenu
                        var user = users.FirstOrDefault(u => u.Username == currentUser);
                        if (user != null)
                            loggedInMenu.SetCurrentUser(user);
                    }
                }
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
