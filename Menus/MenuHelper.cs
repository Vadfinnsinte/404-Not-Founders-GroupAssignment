using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    public class MenuHelper
    {
        Character character = new Character();
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
        
        // ----- APPENS START/HUVUDLOOP -----
        public void RunApp()
        {
            bool running = true, loggedIn = false;
            string currentUser = null;
            var users = _userService.Users;

            while (running)
            {
                if (!loggedIn)
                    ShowLoginRegisterMenu(users, out loggedIn, out currentUser, ref running);
                else
                    ShowLoggedInMenu(ref loggedIn, ref currentUser, ref running);
            }

            Info("Thank you for using the app, see you next time");
            Info("Closing down...");
            DelayAndClear();
        }


        // ----- UI-HELPERS OCH GEMENSAM LOGIK -----

        /// Meny med Orange highlight (aktivt) och vita val (inaktivt)
        public static string Menu(string title, params string[] choices) =>
             AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#{MainTitleColor}]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(choices)
                    .UseConverter(choice => $"[white]{choice}[/]")
            );

       
        // Add this method to the MenuHelper class
        public static string ReadBackOrExit()
        {
            var input = Console.ReadLine();
            if (string.Equals(input, "E", StringComparison.OrdinalIgnoreCase))
                return "E";
            if (string.Equals(input, "B", StringComparison.OrdinalIgnoreCase))
                return "B";
            return input;
        }




        /// Skriv ut orange, understruken rubrik (använd alltid för rubriker och viktig feedback)
        public static void Info(string text) => AnsiConsole.MarkupLine($"[underline {MainTitleColor}]{text}[/]");

        /// Skriv ut instruktion till användaren om E och B
        public static void InputInstruction(bool back = false) =>
            AnsiConsole.MarkupLine(back
                ? "[grey italic]Press E to go back or B to return to the previous step[/]"
                : "[grey italic]Press E to go back[/]");

        /// Delay och skärmrens – anropas efter bekräftelse eller fel
        public static void DelayAndClear(int ms = 800) { Thread.Sleep(ms); Console.Clear(); }

        /// Input helpers – AskInput hanterar både secret och vanlig, och alltid "E" för exit
        public static string AskInput(string prompt, bool secret = false)
        {
            var input = secret
                ? AnsiConsole.Prompt(new TextPrompt<string>(prompt).PromptStyle(MainTitleColor).Secret())
                : AnsiConsole.Ask<string>(prompt);
            if (input.Trim().Equals("E", StringComparison.OrdinalIgnoreCase)) return null;
            return input.Trim();
        }

        /// Gemensam feedback – skriver ut resultat med grön/röd + orange underline
        public static void Result(bool success, string text)
        {
            var color = success ? "green" : "red";
            AnsiConsole.MarkupLine($"[underline {MainTitleColor}][bold {color}]{text}[/][/]");
        }

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


