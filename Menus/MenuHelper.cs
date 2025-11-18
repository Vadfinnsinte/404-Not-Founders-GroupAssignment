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

            while (running)
            {
                if (!loggedIn)
                    ShowLoginRegisterMenu(users, out loggedIn, out currentUser, ref running);
                else
                    ShowLoggedInMenu(ref loggedIn, ref currentUser, ref running);
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
                if (choice == "Logga in" && LoginMenu(users, out currentUser))
                {
                    loggedIn = true;
                    string tempUser = currentUser; // Kopiera ref-värdet!
                    var foundUser = users.FirstOrDefault(u => u.Username == tempUser);
                    _currentUser = foundUser;
                    Console.Clear();
                    break;
                }
                string newUser = null;
                if (choice == "Registrera dig" && User.RegisterUser(users, out newUser, _userService))
                {
                    loggedIn = true;
                    currentUser = newUser;
                    // Sätt rätt user-objekt!
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
                Info("Logga in");
                InputInstruction(true);
                if (step >= 1)
                    AnsiConsole.MarkupLine($"[grey]Användarnamn:[/] [#FFA500]{username}[/]");

                string value = step == 0
                    ? AskInput("[#FFA500]Användarnamn:[/]")
                    : AskInput("[#FFA500]Lösenord:[/]", true);

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
                        Result(true, "Loggar in...");
                        DelayAndClear();
                        loggedInUser = username;
                        _currentUser = user;
                        return true;
                    }
                    Result(false, "Fel användarnamn eller lösenord!");
                    DelayAndClear(1200);
                    password = ""; step = 1;
                }
            }
        }

        // ----- MENY FÖR INLOGGADE ANVÄNDARE OCH LÄNKAR -----
        public void ShowLoggedInMenu(ref bool loggedIn, ref string currentUser, ref bool running)
        {
            while (running)
            {
                if (_currentUser == null)
                {
                    Result(false, "Ingen användare är inloggad!");
                    DelayAndClear();
                    loggedIn = false;
                    currentUser = null;
                    return;
                }

                Console.Clear();
                Info($"Huvudmeny (inloggad som {_currentUser.Username})");
                var choice = Menu("Vad vill du göra?",
                                  "Lägg till projekt",
                                  "Visa projekt",
                                  "Senaste projekt",
                                  "Redigera konto",
                                  "Logga ut",
                                  "Avsluta");

                switch (choice)
                {
                    case "Avsluta":
                        running = false;
                        return;
                    case "Logga ut":
                        Result(true, "Du loggas ut...");
                        DelayAndClear();
                        loggedIn = false;
                        currentUser = null;
                        _currentUser = null;
                        return;
                    case "Lägg till projekt":
                        Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                        var newProject = new Project();
                        var addedProject = newProject.Add(_currentUser, _userService);
                        DelayAndClear();
                        ProjectEditMenu(addedProject);
                        break;
                    case "Visa projekt":
                        ShowProjectMenu();
                        break;
                    case "Senaste projekt":
                        ShowLastProjectMenu();
                        break;
                    case "Redigera konto":
                        // Lägg till redigeringslogik här om behövs.
                        break;
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


        public void ProjectEditMenu(Project project)
        {
            Info("Projekt");
            string choises = ProjectEditVisuals.ShowEditMenu(project);

            switch (choises)
            {
                case "Edit/Add Charachters":
                    Console.WriteLine("Coming soon");
                    break;
                case "Edit/Add worlds":
                    if (_currentUser != null)
                    {
                        WorldMenu(_currentUser, project);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Ingen användare inloggad?![/]");
                        DelayAndClear();
                    }
                    break;
                case "Edit/Add Storylines":
                    StorylineMenu(project);
                    break;
                case "Show Everything":
                   
                    Console.WriteLine("Coming soon");
                    DelayAndClear();
                    break;
                case "Back to main menu":
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
        public static void CharacterMenu()
        {
            Info("Karaktärsmenyn");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }
        public void StorylineMenu(Project project)
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Storylines[/]")
                        .AddChoices("Add Storyline", "Edit Storyline", "Back")
                        .HighlightStyle(Color.Orange1));

                switch (choice)
                {
                    case "Add Storyline":
                        AddStorylineToProject(project);
                        break;

                    case "Edit Storyline":
                        EditStoryline(project);
                        break;

                    case "Back":
                        return;
                }
            }
        }
        private void AddStorylineToProject(Project project)
        {
            Console.Clear();
            Info("Add new storyline");

            var title = AnsiConsole.Ask<string>("[#FFA500]Title:[/]");
            var synopsis = AnsiConsole.Ask<string>("[#FFA500]Synopsis (Short description):[/]");
            var theme = AnsiConsole.Ask<string>("[#FFA500]Theme:[/]");
            var genre = AnsiConsole.Ask<string>("[#FFA500]Genre:[/]");
            var story = AnsiConsole.Ask<string>("[#FFA500]Story:[/]");
            var ideaNotes = AnsiConsole.Ask<string>("[#FFA500]Ideanotes:[/]");
            var otherInfo = AnsiConsole.Ask<string>("[#FFA500]Other info:[/]");

            project.Storylines ??= new List<Storyline>();

            var s = new Storyline
            {
                Title = title,
                Synopsis = synopsis,
                Theme = theme,
                Genre = genre,             
                Story = story,
                IdeaNotes = ideaNotes,
                OtherInfo = otherInfo,
                orderInProject = project.Storylines.Count + 1,
                dateOfLastEdit = DateTime.Now
            };

            project.Storylines.Add(s);
            _userService.SaveUserService();

            Info("Storyline created!");
            DelayAndClear();
        }
        private void EditStoryline(Project project)
        {
            var s = SelectStoryline(project, "Choose storyline to edit");
            if (s == null) return;

            while (true)
            {
                Console.Clear();
                Info($"Edit storyline: [#FFA500]{s.Title}[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to edit?")
                        .AddChoices(
                            "Title",
                            "Synopsis",
                            "Theme",
                            "Genre",
                            "Story",
                            "Ideanotes",
                            "Other info",
                            "Done")
                        .HighlightStyle(Color.Orange1));

                if (choice == "Done")
                {
                    s.dateOfLastEdit = DateTime.Now;
                    _userService.SaveUserService();
                    Info("Storyline uppdated!");
                    DelayAndClear();
                    return;
                }

                string PromptNonEmpty(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (!string.IsNullOrWhiteSpace(value))
                            return value;

                        AnsiConsole.MarkupLine("[red]Value must not be empty.[/]");
                    }
                }

                switch (choice)
                {
                    case "Title":
                        s.Title = PromptNonEmpty("[#FFA500]New title:[/]");
                        break;
                    case "Synopsis":
                        s.Synopsis = PromptNonEmpty("[#FFA500]New synopsis:[/]");
                        break;
                    case "Theme":
                        s.Theme = PromptNonEmpty("[#FFA500]New theme:[/]");
                        break;
                    case "Genre":
                        s.Genre = PromptNonEmpty("[#FFA500]New genre:[/]");
                        break;
                    case "Story":
                        s.Story = PromptNonEmpty("[#FFA500]New story:[/]");
                        break;
                    case "Ideanotes":
                        s.IdeaNotes = PromptNonEmpty("[#FFA500]New ideanotes:[/]");
                        break;
                    case "Other info":
                        s.OtherInfo = PromptNonEmpty("[#FFA500]New other info:[/]");
                        break;
                }
            }
        }
        private Storyline? SelectStoryline(Project project, string title)
        {
            if (project.Storylines == null || project.Storylines.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No storylines yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            var sorted = project.Storylines
                .OrderBy(s => s.orderInProject)
                .ToList();

            return AnsiConsole.Prompt(
                new SelectionPrompt<Storyline>()
                    .Title($"[bold]{title}[/]")
                    .AddChoices(sorted)
                    .UseConverter(s => $"{s.orderInProject}. {s.Title}"));
        }

        public static void ShowLastProjectMenu()
        {
            Info("Senaste projekt");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }

        // ----- WORLD MENU -----

        public void WorldMenu(User loggedInUser, Project currentProject)
        {

            while (true)
            {
                Console.Clear();
                Info("World Menu");
                var choice = Menu("", 
                    "Add World", 
                    "Show Worlds", 
                    "Edit World", 
                    "Remove World", 
                    "Back");

                switch (choice)
                {
                    case "Add World":
                        World newWorld = new World();
                        newWorld.Add(loggedInUser, currentProject, _userService);
                        break;

                    case "Show Worlds":
                        Console.WriteLine("Coming soon");
                        break;
                    case "Edit World":
                        Console.WriteLine("Coming soon");
                        break;
                    case "Remove World":
                        Console.WriteLine("Coming soon");
                        break;
                    case "Back":
                        return;
                }
            }
        }
    }
}

