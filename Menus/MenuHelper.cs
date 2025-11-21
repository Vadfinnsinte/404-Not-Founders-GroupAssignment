using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI;
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



        // Add this getter so other classes can read the currently logged-in user
        public User? CurrentUser => _currentUser;

        // ----- HUVUDMENY (login/reg/avsluta) -----
        public void ShowLoginRegisterMenu(List<User> users, out bool loggedIn, out string currentUser, ref bool running)
        {
            loggedIn = false; currentUser = null;
            while (running)
            {
                Console.Clear();
                var choice = MenuChoises.Menu("Choose an option", "Log in", "Sign up", "Exit");
                if (choice == "Exit") { running = false; return; }
                if (choice == "Log in" && LoginMenu(users, out currentUser))
                {
                    loggedIn = true;
                    Console.Clear();
                    break;
                }
                string newUser = null;
                if (choice == "Sign up" && User.RegisterUser(users, out newUser, _userService))
                {
                    loggedIn = true;
                    currentUser = newUser;
                    _currentUser = users.FirstOrDefault(u => u.Username == newUser);
                    Console.Clear();
                    break;
                }
            }
        }


        // ----- INLOGGNING (med stegbaserad backa och återanvändbar vy) -----
        public bool LoginMenu(List<User> users, out string loggedInUser)
        {
            loggedInUser = null;
            string username = "", password = ""; int step = 0;
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

                if (value == null) return false;
                if (value.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0) { if (step == 1) username = ""; step--; }
                    continue;
                }
                if (step == 0) { username = value; step++; }
                else if (step == 1) { password = value; step++; }

                if (step == 2)
                {
                    var user = users.Find(u => u.Username == username);
                    if (user != null && user.Password == password)
                    {
                        ConsoleHelpers.Result(true, "Logging in…");
                        ConsoleHelpers.DelayAndClear();
                        loggedInUser = username;
                        _currentUser = user;
                        return true;
                    }
                    ConsoleHelpers.Result(false, "Wrong username or password!");
                    ConsoleHelpers.DelayAndClear(1200);
                    password = ""; step = 1;
                }
            }
        }
    }
}


