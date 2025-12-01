using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    public class MenuHelper
    {
        public const string MainTitleColor = "#FFA500";
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;

        public MenuHelper(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        public void SetCurrentUser(User user) => _currentUser = user;

        public User? CurrentUser => _currentUser;

        public (bool loggedIn, string currentUser, bool running) ShowLoginRegisterMenu(List<User> users, bool running)
        {
            bool loggedIn = false;
            string currentUser = null;

            while (running)
            {
                Console.Clear();
                var choice = MenuChoises.Menu("Choose an option", "Log in", "Sign up", "Exit");

                if (choice == "Exit")
                {
                    running = false;
                    Console.Clear();
                    break;
                }

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
                }

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
                }
            }

            return (loggedIn, currentUser, running);
        }

        public (bool success, string loggedInUser) LoginMenu(List<User> users)
        {
            string loggedInUser = null;
            string username = "", password = "";
            int step = 0;

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("Log in");
                ConsoleHelpers.InputInstruction(true);
                if (step >= 1)
                    AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

                string value = step == 0
                    ? ConsoleHelpers.AskInput("[#FFA500]Username:[/]")
                    : ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true);

                if (value == null) return (false, null);
                if (value.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0)
                    {
                        if (step == 1) username = "";
                        step--;
                    }
                    continue;
                }

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

                if (step == 2)
                {
                    var user = users.Find(u => u.Username == username);
                    if (user != null && user.Password == password)
                    {
                        ConsoleHelpers.Result(true, "Logging in…");
                        ConsoleHelpers.DelayAndClear();
                        loggedInUser = username;
                        _currentUser = user;
                        return (true, loggedInUser);
                    }

                    ConsoleHelpers.Result(false, "Wrong username or password!");
                    ConsoleHelpers.DelayAndClear(1200);
                    password = "";
                    step = 1;
                }
            }
        }
    }
}
