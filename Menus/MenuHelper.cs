using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace _404_not_founders.Menus
{
    public class MenuHelper
    {
        private const string MainTitleColor = "#FFA500";
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;
       
        public MenuHelper(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));


        }
        
        public void SetCurrentUser(User user) => _currentUser = user;
       
        // ----- APPENS START/HUVUDLOOP -----
        public void RunApp()
        {
            bool running = true, loggedIn = false;
            string currentUser = null;
            var users = _userService.Users;

            // Kör appens huvudflöde tills användaren avslutar
            while (running)
            {
                if (!loggedIn)
                    ShowLoginRegisterMenu(users, out loggedIn, out currentUser, ref running);
                else
                    ShowLoggedInMenu(currentUser, ref loggedIn, ref currentUser, ref running);
            }
            Info("Tack för att du använde appen!");
            Info("Stänger ner...");
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

        /// Skriv ut orange, understruken rubrik (använd alltid för rubriker och viktig feedback)
        public static void Info(string text) => AnsiConsole.MarkupLine($"[underline {MainTitleColor}]{text}[/]");

        /// Skriv ut instruktion till användaren om E och B
        public static void InputInstruction(bool back = false) =>
            AnsiConsole.MarkupLine(back
                ? "[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]"
                : "[grey italic]Skriv E för att gå tillbaka[/]");

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
                var choice = Menu("Välj ett alternativ", "Logga in", "Registrera dig", "Avsluta");
                if (choice == "Avsluta") { running = false; return; }
                if (choice == "Logga in" && LoginMenu(users, out currentUser)) { loggedIn = true; Console.Clear(); break; }
                string newUser = null;
                if (choice == "Registrera dig" && User.RegisterUser(users, out newUser, _userService))
                {
                    loggedIn = true;
                    Console.Clear();
                    break;
                }

            }
        }

        // ----- INLOGGNING (med stegbaserad backa och återanvändbar vy) -----
        public bool LoginMenu(List<User> users, out string loggedInUser)
        {
            loggedInUser = null;
            // Lokala inputfält och step-variabel
            string username = "", password = "";
            int step = 0; // 0 = användarnamn, 1 = lösenord

            while (true)
            {
                Console.Clear();
                Info("Logga in");
                InputInstruction(true);
                // Visa redan ifyllt användarnamn
                if (step >= 1)
                    AnsiConsole.MarkupLine($"[grey]Användarnamn:[/] [#FFA500]{username}[/]");

                // Fråga och hantera input för det aktuella steget
                string value = step == 0
                    ? AskInput("[#FFA500]Användarnamn:[/]")
                    : AskInput("[#FFA500]Lösenord:[/]", true);

                // Avsluta/flöde bakåt?
                if (value == null) return false;
                if (value.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0) { if (step == 1) username = ""; step--; }
                    continue;
                }
                // Spara värde/flytta fram steget
                if (step == 0) { username = value; step++; }
                else if (step == 1) { password = value; step++; }

                // När båda fält är ifyllda – försök logga in
                if (step == 2)
                {
                    var user = users.Find(u => u.Username == username);
                    if (user != null && user.Password == password)
                    {
                        Result(true, "Loggar in...");
                        DelayAndClear();
                        _currentUser = _userService.Users.FirstOrDefault(u => u.Username == username);
                        loggedInUser = username;
                        return true;
                    }
                    Result(false, "Fel användarnamn eller lösenord!");
                    DelayAndClear(1200);
                    // Starta om – nulställ bara password
                    password = ""; step = 1;
                }
            }
        }

        // ----- MENY FÖR INLOGGADE ANVÄNDARE OCH LÄNKAR -----
        public void ShowLoggedInMenu(string username, ref bool loggedIn, ref string currentUser, ref bool running)
        {
            while (running)
            {
                Console.Clear();
                Info($"Huvudmeny (inloggad som {username})");
                var choice = Menu("Vad vill du göra?", "Lägg till projekt", "Visa projekt", "Senaste projekt", "Redigera konto", "Logga ut", "Avsluta");
                Info(choice);

                switch (choice)
                {
                    case "Avsluta":
                        running = false; return;
                    case "Logga ut":
                        Result(true, "Du loggas ut...");
                        DelayAndClear();
                        loggedIn = false;
                        currentUser = null;
                        return;
                    case "Lägg till projekt":
                        // Get the user who is logged in
                        User loggedInUser = _userService.Users.FirstOrDefault(u => u.Username == username);

                        if (loggedInUser != null)
                        {
                            // Create instance for Add
                            Project NewProject = new Project();

                            // Get user input and add project
                            //NewProject.Add(loggedInUser, _userService);
                            NewProject = NewProject.Add(loggedInUser, _userService);

                            // Goes back to Project Menu after adding
                            DelayAndClear();
                            ProjectEditMenu(NewProject);
                        }
                        else
                        {
                            Result(false, "Error: Could not find user data.");
                            DelayAndClear();
                        }
                        break;
                    case "Visa projekt":
                        ShowProjectMenu(); break;
                    case "Senaste projekt":
                        ShowLastProjectMenu(); break;
                    case "Redigera konto":
                        UserMenu(); break;
                }
            }
        }

        // ----- FRAMTIDA UNDERMENYER & PLATSHÅLLARE -----
        public void ShowProjectMenu()
        {
            //             Info("Projektmeny");
            //             DelayAndClear();
            if (_currentUser == null)
            {
                AnsiConsole.MarkupLine("[red]Du måste vara inloggad för att se projekt.[/]");
                Console.WriteLine(_currentUser);
            }// 

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Projekt Meny[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices("Visa alla projekt", "Sök Projekt", "Tillbaka"));

                if (choice == "Tillbaka") break;

                if (choice == "Visa alla projekt")
                {
                    var list = _projectService.GetAll(_currentUser);
                    if (list == null || list.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]Inga projekt ännu.[/]");
                        continue;
                    }

                    var selected = SelectFromList(list, "Välj projekt");
                    if (selected != null)
                        _projectService.SetLastSelected(_currentUser, selected.Id);
                        ProjectEditMenu(selected);
                }
                else if (choice == "Sök Projekt")
                {
                    var term = AnsiConsole.Ask<string>("Sökterm (titel/description):").Trim();
                    var hits = _projectService.Search(_currentUser, term);

                    if (hits == null || hits.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]Inga träffar.[/]");
                        continue;
                    }

                    var selected = SelectFromList(hits, $"Välj från sökresultat för \"{term}\"");
                    if (selected != null)
                        _projectService.SetLastSelected(_currentUser, selected.Id);

                    AnsiConsole.Clear();
                    ProjectEditMenu(selected);
                }

            }
        }

        private Project? SelectFromList(IReadOnlyList<Project> projects, string title)
        {
            if (projects == null || projects.Count == 0) return null;

            var sorted = projects.OrderByDescending(p => p.dateOfCreation).ToList();

            //var table = new Table().Border(TableBorder.Rounded);
            //table.AddColumn("[#FFA500]Title[/]");
            //table.AddColumn("[grey]Date[/]");
            //foreach (var p in sorted)
            //table.AddRow(p.title, p.dateOfCreation.ToString("yyyy-MM-dd"));
            //AnsiConsole.Write(table);

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<Project>()
                    .Title($"[bold]{title}[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(sorted)
                    .UseConverter(p => $"{p.title} ({p.dateOfCreation:yyyy-MM-dd})"));

            AnsiConsole.Clear();
            return selected;

        }


        public static void ProjectEditMenu(Project project)
        {
            Info("Projekt");
            string choises = ProjectEditVisuals.ShowEditMenu(project);

            switch (choises)
            {
                case "Edit/Add Charachters":
                    Console.WriteLine("Coming soon");
                    break;
                case "Edit/Add worlds":
                    Console.WriteLine("Coming soon");
                    break;
                case "Edit/Add Storylines":
                    Console.WriteLine("Coming soon");
                    break;
                case "Show Everything":
                   
                    Console.WriteLine("Coming soon");
                    DelayAndClear();
                    break;
                case "Back to main manu":
                    Console.Clear();
                    return;
                default:
                    Console.WriteLine("Somthing went wrong..going back to menu");
                    return;
            }

            DelayAndClear();
        }
        public static void UserMenu()
        {
            Info("Användarmenyn");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }
        public static void WorldMenu()
        {
            Info("Världsmenyn");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }
        public static void CharacterMenu()
        {
            Info("Karaktärsmenyn");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }
        public static void StorylineMenu()
        {
            Info("Storyline-meny");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }
        public static void ShowLastProjectMenu()
        {
            Info("Senaste projekt");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }
    }
}

