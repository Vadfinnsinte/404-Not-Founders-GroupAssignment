using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    /// Main menu shown when user is logged in
    /// Provides access to project management, account editing, and logout
    public class LoggedInMenu
    {
        private readonly MenuHelper _menuHelper;
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;

        /// Constructor with dependency injection
        public LoggedInMenu(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _menuHelper = new MenuHelper(_userService, _projectService);
        }

        /// Sets the currently logged-in user
        public void SetCurrentUser(User? user) => _currentUser = user;

        /// Main menu loop for logged-in users
        /// Returns tuple with login status and current username
        /// Handles project creation, navigation, and account management
        public async Task<(bool loggedIn, string currentUser)> ShowLoggedInMenu(bool loggedIn, string currentUser)
        {
            bool running = true;
            while (running)
            {
                // Security check - ensure user is still logged in
                if (_currentUser == null)
                {
                    Console.Clear();
                    loggedIn = false;
                    currentUser = null;
                    return (loggedIn, currentUser);
                }

                // Display main menu header
                Console.Clear();
                ConsoleHelpers.Info("Main menu");
                AnsiConsole.MarkupLine($"User: {_currentUser.Username}");

                // Present main menu options
                var choice = MenuChoises.Menu("What would you like to do?",
                                  "Add project",
                                  "Handle project",
                                  "Latest project",
                                  "Edit account",
                                  "Log out");

                // Handle user choice
                switch (choice)
                {
                    case "Log out":
                        // Clear session and return to login screen
                        ConsoleHelpers.Result(true, "Logging out...");
                        ConsoleHelpers.DelayAndClear();
                        loggedIn = false;
                        currentUser = null;
                        _currentUser = null;
                        running = false;
                        break;

                    case "Add project":
                        // Create new project with manual or AI generation
                        ConsoleHelpers.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                        var newProject = new Project();
                        var addedProject = await newProject.Add(_currentUser, _userService);

                        // Handle cancelled creation
                        if (addedProject == null)
                        {
                            ConsoleHelpers.Info("Project creation cancelled.");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }
                        ConsoleHelpers.DelayAndClear();

                        // Open newly created project in edit menu
                        var projectMenu = new ProjectChoisesMenu(_currentUser, _projectService, _userService);
                        await projectMenu.ProjectEditMenu(addedProject);
                        break;

                    case "Handle project":
                        // Navigate to project list and selection menu
                        var projectMenu2 = new ProjectChoisesMenu(_currentUser, _projectService, _userService);
                        await projectMenu2.ShowProjectMenu();
                        break;

                    case "Latest project":
                        // Quick access to last selected project
                        var projectMenu3 = new ProjectChoisesMenu(_currentUser, _projectService, _userService);
                        var username = currentUser;
                        var user = _userService.Users.FirstOrDefault(u => u.Username == username);

                        // Validate user exists before proceeding
                        if (user != null)
                            await projectMenu3.ShowLastProjectMenu(user);
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Could not find current user.[/]");
                            Console.ReadKey(true);
                        }
                        break;

                    case "Edit account":
                        // Open account editing menu
                        var editResult = await EditUserMenu(currentUser);
                        currentUser = editResult.currentUser;  // Update username if changed
                        break;
                }
            }
            return (loggedIn, currentUser);
        }

        /// Opens the user account edit menu
        /// Returns tuple with completion status and updated username
        public async Task<(bool finished, string currentUser)> EditUserMenu(string currentUser)
        {
            // Validate user is logged in
            if (_currentUser == null)
            {
                ConsoleHelpers.Result(false, "No user logged in!");
                ConsoleHelpers.DelayAndClear();
                return (false, currentUser);
            }

            // Call user edit method (synchronous, but wrapped in Task for consistency)
            var (finished, updatedUser) = _currentUser.EditUser(_userService, currentUser);

            if (finished)
            {
                ConsoleHelpers.Info($"New {updatedUser}.");
                ConsoleHelpers.DelayAndClear();
            }

            return (finished, updatedUser);
        }
    }
}
