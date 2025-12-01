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

        // Constructor with dependency injection
        public MenuHelper(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        public void SetCurrentUser(User user) => _currentUser = user;

        public User? CurrentUser => _currentUser;

        // ----- MAIN MENU (login/reg/exit) -----
        public void ShowLoginRegisterMenu(List<User> users, out bool loggedIn, out string currentUser, ref bool running)
        {
            loggedIn = false; currentUser = null;
            // Login/Register Menu Loop
            while (running)
            {
                Console.Clear();
                // Displays the main menu choices and handles user selection
                var choice = MenuChoises.Menu("Choose an option", "Log in", "Sign up", "Exit");

                // Avsluta programmet direkt om användaren väljer Exit
                if (choice == "Exit")
                {
                    loggedIn = true;
                    Console.Clear();
                    break;
                }
                // create new user and adds to the users list
                string newUser = null;
                if (choice == "Sign up" && User.RegisterUser(users, out newUser, _userService))
                {
                    var regResult = User.RegisterUser(users, _userService); // Tuple-retur här!
                    if (regResult.success)
                    {
                        loggedIn = true;
                        currentUser = regResult.registeredUser;
                        _currentUser = users.FirstOrDefault(u => u.Username == currentUser); // Sätt nuvarande användare om du vill
                        Console.Clear();
                        return (loggedIn, currentUser, running);
                    }
                    // Vid misslyckad registrering – fortsätt loopen så man kan försöka igen
                }
                // Loopen fortsätter tills logga in eller exit.
            }
            // Returnera status om vi brutit loopen utan inlogg/registrering
            return (loggedIn, currentUser, running);
        }


        // ----- LOGIN MENU (using steps for the login process) -----
        public bool LoginMenu(List<User> users, out string loggedInUser)
        {
            loggedInUser = null;
            string username = "", password = ""; int step = 0;
            // Login Menu Loop
            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("Log in");
                ConsoleHelpers.InputInstruction(true);
                if (step >= 1)
                    AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

                // Ask for username or password based on the current step
                string value = step == 0
                    ? ConsoleHelpers.AskInput("[#FFA500]Username:[/]")
                    : ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true);

                // Handle back and exit commands
                if (value == null) return false;
                if (value.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0) { if (step == 1) username = ""; step--; }
                    continue;
                }

                // Handle input based on the current step, step 0 = username, step 1 = password
                if (step == 0) { username = value; step++; }
                else if (step == 1) { password = value; step++; }

                // Step 2 validates the credentials and logs in the user if valid
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
                    // Invalid credentials, reset to step 1
                    ConsoleHelpers.Result(false, "Wrong username or password!");
                    ConsoleHelpers.DelayAndClear(1200);
                    password = ""; step = 1;
                }
            }
        }
    }
}

