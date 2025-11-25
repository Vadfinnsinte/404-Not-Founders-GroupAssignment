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

        public User? CurrentUser => _currentUser;

        // ----- HUVUDMENY (login/reg/avsluta) -----
        public async Task<(bool loggedIn, string currentUser, bool running)> ShowLoginRegisterMenu(List<User> users)
        {
            // Variabler för status och nuvarande användare
            bool loggedIn = false;
            string currentUser = null;
            bool running = true;

            // Loopa tills användaren är inloggad eller exit
            while (running && !loggedIn)
            {
                Console.Clear();
                // Visa valmeny
                var choice = MenuChoises.Menu("Choose an option", "Log in", "Sign up", "Exit");

                // Avsluta programmet direkt om användaren väljer Exit
                if (choice == "Exit")
                {
                    running = false;
                    return (loggedIn, currentUser, running);
                }

                // Logga in-funktionen
                if (choice == "Log in")
                {
                    var loginResult = LoginMenu(users); // Tuple-retur här!
                    if (loginResult.success)
                    {
                        loggedIn = true;
                        currentUser = loginResult.loggedInUser;
                        Console.Clear();
                        // Man kan här sätta _currentUser om det används
                        return (loggedIn, currentUser, running);
                    }
                    // Om login misslyckades – loopen fortsätter tills rätt/försök igen.
                }

                // Registrera användare-funktionen
                if (choice == "Sign up")
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


        // ----- INLOGGNING (med stegbaserad backa och återanvändbar vy) -----
        /// Logga in. Returnerar tuple (om inlogg lyckas, och inloggat username)
        public (bool success, string loggedInUser) LoginMenu(List<User> users)
        {
            Console.Clear();
            ConsoleHelpers.Info("[#FFA500]Log in[/]");
            ConsoleHelpers.InputInstruction();

            string username = ConsoleHelpers.AskInput("[#FFA500]Username:[/]");
            if (string.IsNullOrWhiteSpace(username) || username.Trim() == "E")
                return (false, null);

            string password = ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true);
            if (string.IsNullOrWhiteSpace(password) || password.Trim() == "E")
                return (false, null);

            var existingUser = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (existingUser != null)
            {
                ConsoleHelpers.Result(true, $"Welcome back, {username}!");
                ConsoleHelpers.DelayAndClear(800);
                return (true, username);
            }
            else
            {
                ConsoleHelpers.Result(false, "Invalid username or password.");
                ConsoleHelpers.DelayAndClear(1200);
                return (false, null);
            }
        }
    }
}