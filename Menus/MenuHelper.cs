using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    /// Helper class for authentication and user management
    /// Handles login and registration workflows
    public class MenuHelper
    {
        public const string MainTitleColor = "#FFA500";
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;

        /// Constructor with dependency injection
        public MenuHelper(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        public void SetCurrentUser(User user) => _currentUser = user;

        public User? CurrentUser => _currentUser;

        /// Shows the initial login/register menu
        /// Returns tuple with login status, username, and running flag
        /// Handles both login and registration workflows
        public (bool loggedIn, string currentUser, bool running) ShowLoginRegisterMenu(List<User> users, bool running)
        {
            bool loggedIn = false;
            string currentUser = null;

            while (running)
            {
                Console.Clear();

                // Present main authentication options
                var choice = MenuChoises.Menu("Choose an option", "Log in", "Sign up", "Exit");

                // Exit application
                if (choice == "Exit")
                {
                    running = false;
                    Console.Clear();
                    break;
                }

                // Registration workflow
                if (choice == "Sign up")
                {
                    var regResult = User.RegisterUser(users, _userService);
                    if (regResult.success)
                    {
                        loggedIn = true;
                        currentUser = regResult.registeredUser;
                        _currentUser = users.FirstOrDefault(u => u.Username == currentUser);
                        Console.Clear();
                        return (loggedIn, currentUser, running);
                    }
                    // If registration failed, loop continues for retry
                }

                // Login workflow
                if (choice == "Log in")
                {
                    var loginResult = LoginMenu(users);
                    if (loginResult.success)
                    {
                        loggedIn = true;
                        currentUser = loginResult.loggedInUser;
                        _currentUser = users.FirstOrDefault(u => u.Username == currentUser);
                        Console.Clear();
                        return (loggedIn, currentUser, running);
                    }
                    // If login failed, loop continues for retry
                }
            }

            return (loggedIn, currentUser, running);
        }

        /// Step-by-step login process with validation
        /// Returns tuple with success status and logged-in username
        /// Supports back navigation between username and password steps
        public (bool success, string loggedInUser) LoginMenu(List<User> users)
        {
            string loggedInUser = null;
            string username = "", password = "";
            int step = 0;

            // Login step loop
            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("Log in");
                ConsoleHelpers.InputInstruction(true);

                // Display username if already entered
                if (step >= 1)
                    AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

                // Prompt for current step (username or password)
                string value = step == 0
                    ? ConsoleHelpers.AskInput("[#FFA500]Username:[/]")
                    : ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true);  // Hide password input

                // Handle exit command
                if (value == null) return (false, null);

                // Handle back navigation
                if (value.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0)
                    {
                        if (step == 1) username = "";  // Clear username when going back
                        step--;
                    }
                    continue;
                }

                // Store input for current step
                if (step == 0)
                {
                    username = value;
                    step++;
                }
                else if (step == 1)
                {
                    password = value;
                    step++;
                }

                // Validation step - check credentials
                if (step == 2)
                {
                    var user = users.Find(u => u.Username == username);

                    // Successful login
                    if (user != null && user.Password == password)
                    {
                        ConsoleHelpers.Result(true, "Logging in…");
                        ConsoleHelpers.DelayAndClear();
                        loggedInUser = username;
                        _currentUser = user;
                        return (true, loggedInUser);
                    }

                    // Invalid credentials - reset to password step
                    ConsoleHelpers.Result(false, "Wrong username or password!");
                    ConsoleHelpers.DelayAndClear(1200);
                    password = "";
                    step = 1;  // Stay on password step, keep username
                }
            }
        }
    }
}
