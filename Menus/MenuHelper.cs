using Spectre.Console;
using _404_not_founders.Models;
using System;
using System.Collections.Generic;
using _404_not_founders.Services;

namespace _404_not_founders.Menus
{
    public class MenuHelper
    {
        private const string MainTitleColor = "#FFA500";
        private readonly UserService _userService;
        private readonly UserService _svc;
        private readonly User _currentUser;
        private readonly ProjectService _projectService;



        public MenuHelper(UserService userService)
        {
            _userService = userService;
            _projectService = new ProjectService(userService);
        }
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
        private static string Menu(string title, params string[] choices) =>
            AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#{MainTitleColor}]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(choices)
                    .UseConverter(choice => $"[white]{choice}[/]")
            );

        /// Skriv ut orange, understruken rubrik (använd alltid för rubriker och viktig feedback)
        private static void Info(string text) => AnsiConsole.MarkupLine($"[underline {MainTitleColor}]{text}[/]");

        /// Skriv ut instruktion till användaren om E och B
        private static void InputInstruction(bool back = false) =>
            AnsiConsole.MarkupLine(back
                ? "[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]"
                : "[grey italic]Skriv E för att gå tillbaka[/]");

        /// Delay och skärmrens – anropas efter bekräftelse eller fel
        private static void DelayAndClear(int ms = 800) { Thread.Sleep(ms); Console.Clear(); }

        /// Input helpers – AskInput hanterar både secret och vanlig, och alltid "E" för exit
        private static string AskInput(string prompt, bool secret = false)
        {
            var input = secret
                ? AnsiConsole.Prompt(new TextPrompt<string>(prompt).PromptStyle(MainTitleColor).Secret())
                : AnsiConsole.Ask<string>(prompt);
            if (input.Trim().Equals("E", StringComparison.OrdinalIgnoreCase)) return null;
            return input.Trim();
        }

        /// Gemensam feedback – skriver ut resultat med grön/röd + orange underline
        private static void Result(bool success, string text)
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
                if (choice == "Registrera dig" && RegisterMenu(users, out currentUser))
                {
                    loggedIn = true;
                    Console.Clear();
                    break;
                }
                
            }
        }

        // ----- INLOGGNING (med stegbaserad backa och återanvändbar vy) -----
        public static bool LoginMenu(List<User> users, out string loggedInUser)
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

        // ----- REGISTRERING, direktvalidering per prompt, stegbaserad vy och backa (B/E) support -----
        public bool RegisterMenu(List<User> users, out string registeredUser)
        {
            registeredUser = null;
            string email = "", username = "", password = "";
            int step = 0; // 0 = epost, 1 = användarnamn, 2 = lösenord
            
            while (true)
            {
                Console.Clear();
                Info("Registrera ny användare");
                InputInstruction(true);

                // Visa redan ifyllda/validerade fält
                if (step >= 1)
                    AnsiConsole.MarkupLine($"[grey]E-post:[/] [#FFA500]{email}[/]");
                if (step >= 2)
                    AnsiConsole.MarkupLine($"[grey]Användarnamn:[/] [#FFA500]{username}[/]");

                // Fråga för aktuellt steg – byter vy i loopen, reset vid back
                var value = step switch
                {
                    0 => AskInput("[#FFA500]E-post:[/]"),
                    1 => AskInput("[#FFA500]Användarnamn:[/]"),
                    2 => AskInput("[#FFA500]Lösenord:[/]", true),
                    _ => null
                };

                // Avbrott – tillbaka till huvudmeny
                if (value == null) return false;
                // Backa till tidigare prompt
                if (value.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0)
                    {
                        if (step == 1) email = "";
                        if (step == 2) username = "";
                        step--;
                    }
                    continue;
                }
                // Direktvalidera per steg och lägg in/sätt fram
                if (step == 0)
                {
                    if (users.Exists(u => u.Email.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        Result(false, "E-postadressen är redan registrerad."); DelayAndClear(1200); continue;
                    }
                    email = value; step++;
                }
                else if (step == 1)
                {
                    if (users.Exists(u => u.Username.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        Result(false, "Användarnamnet är redan taget."); DelayAndClear(1200); continue;
                    }
                    username = value; step++;
                }
                else if (step == 2)
                {
                    password = value; step++;
                }

                // Alla fält validerade – bekräfta!
                if (step == 3)
                {
                    var confirm = Menu("Vill du registrera denna användare?", "Ja", "Nej", "Avsluta");
                    if (confirm == "Avsluta") Environment.Exit(0);
                    if (confirm == "Nej") { step = 0; continue; }
                    if (confirm == "Ja")
                    {
                        users.Add(new User
                        {
                            Email = email,
                            Username = username,
                            Password = password,
                            CreationDate = DateTime.Now,
                            Projects = new List<Project>()
                        });
                        _userService.SaveUserService();
                        Result(true, "Registreras...!");
                        DelayAndClear();
                        registeredUser = username;
                        return true;
                        
                    }
                    step = 0;
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
                            NewProject.Add(loggedInUser, _userService);

                            // Goes back to Project Menu after adding
                            ProjectMenu();
                        }
                        else
                        {
                            Result(false, "Error: Could not find user data.");
                            DelayAndClear();
                        }
                        break;
                    case "Visa projekt":
                        var user = _userService.Users.FirstOrDefault(u => u.Username == username);
                        if (user != null)
                            ShowProjectMenu(user);
                        else
                        {
                            Result(false, "Error: Could not find user data.");
                            DelayAndClear();
                        }
                        break;
                    case "Senaste projekt":
                        ShowLastProjectMenu(); break;
                    case "Redigera konto":
                        UserMenu(); break;
                }
            }
        }

        // ----- FRAMTIDA UNDERMENYER & PLATSHÅLLARE -----
        public void ShowProjectMenu(User currentUser)
        {
//             Info("Projektmeny");
//             DelayAndClear();
            
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
                    var list = _projectService.GetAll(currentUser);
                    var selected = SelectFromList(list, "Välj projekt");
                    if (selected != null)
                        _projectService.SetLastSelected(currentUser, selected.Id);
                    ShowProjectDetailMenu(currentUser, selected);
                }
                else if (choice == "Sök Projekt")
                {
                    var term = AnsiConsole.Ask<string>("Sökterm (titel/description):").Trim();
                    var hits = _projectService.Search(currentUser, term);

                    if (hits.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]Inga träffar.[/]");
                        continue;
                    }

                    var selected = SelectFromList(hits, $"Välj från sökresultat för \"{term}\"");
                    if (selected != null)
                        _projectService.SetLastSelected(currentUser, selected.Id);
                    ShowProjectDetailMenu(currentUser, selected);
                }
            }
        }
        private Project? SelectFromList(IReadOnlyList<Project> projects, string title)
        {
            var sorted = projects.OrderByDescending(p => p.dateOfCreation).ToList();

            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("[#FFA500]Title[/]");
            table.AddColumn("[grey]Date[/]");
            foreach (var p in sorted)
                table.AddRow(p.title, p.dateOfCreation.ToString("yyyy-MM-dd"));
            AnsiConsole.Write(table);

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<Project>()
                    .Title($"[bold]{title}[/]")
                    .PageSize(10)
                    .AddChoices(sorted)
                    .UseConverter(p => $"{p.title} ({p.dateOfCreation:yyyy-MM-dd})"));

            AnsiConsole.MarkupLine($"[green]Valt:[/] {selected.title}");
            return selected;
        }


        public static void ProjectMenu()
        {
            Info("Projekt");
            Console.WriteLine("Coming Soon");
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
        public void ShowProjectDetailMenu(User currentUser, Project project)
        {
            while (true)
            {
                Console.Clear();
                Info($"Projekt: [#FFA500]{project.title}[/]");
                AnsiConsole.MarkupLine($"[grey]{project.description}[/]");
                AnsiConsole.MarkupLine($"[grey]Skapad:[/] {project.dateOfCreation:yyyy-MM-dd}");
                AnsiConsole.WriteLine();

                var choice = Menu(
                    "Vad vill du göra i detta projekt?",
                    "Redigera storylines",
                    "Redigera karaktärer",
                    "Redigera världar",
                    "Visa allt",
                    "Tillbaka");

                switch (choice)
                {
                    case "Redigera storylines":
                       
                        StorylineMenu(currentUser, project);
                        break;

                    case "Redigera karaktärer":
                        CharacterMenu(); 
                        break;

                    case "Redigera världar":
                        WorldMenu();     
                        break;

                    case "Visa allt":
                        AnsiConsole.MarkupLine("[grey]Visa allt kommer senare.[/]");
                        Console.ReadKey(true);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }
        public void StorylineMenu(User currentUser, Project project)
        {

            while (true)
            {
                Console.Clear();
                Info($"Storylines i projektet [#FFA500]{project.title}[/]");

                var choice = Menu("Storyline-meny",
                    "Lägg till storyline",      
                    "Visa befintliga",          
                    "Ändra storyline",          
                    "Ta bort storyline",        
                    "Tillbaka till projekt");

                switch (choice)
                {
                    case "Lägg till storyline":
                        AddStorylineToProject(project);
                        break;

                    case "Visa befintliga":
                        ShowStorylines(project);
                        break;

                    case "Ändra storyline":
                        EditStoryline(project);
                        break;

                    case "Ta bort storyline":
                        DeleteStoryline(project);
                        break;

                    case "Tillbaka till projekt":
                        return;
                }
            }
        }
        private void AddStorylineToProject(Project project)
        {
            Console.Clear();
            Info("Skapa ny storyline");

            var title = AnsiConsole.Ask<string>("[#FFA500]Titel:[/]");
            var synopsis = AnsiConsole.Ask<string>("[#FFA500]Synopsis (kort beskrivning):[/]");
            var theme = AnsiConsole.Ask<string>("[#FFA500]Tema:[/]");
            var genre = AnsiConsole.Ask<string>("[#FFA500]Genre:[/]");
            var story = AnsiConsole.Ask<string>("[#FFA500]Själva storyn (kort):[/]");
            var ideaNotes = AnsiConsole.Ask<string>("[#FFA500]Idéanteckningar:[/]");
            var otherInfo = AnsiConsole.Ask<string>("[#FFA500]Övrig info:[/]");

            project.Storylines ??= new List<Storyline>();

            var storyline = new Storyline
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

            project.Storylines.Add(storyline);
            _userService.SaveUserService();

            Result(true, "Storyline sparad!");
            DelayAndClear();
        }
        private void ShowStorylines(Project project)
        {
            Console.Clear();
            Info($"Storylines i projektet {project.title}");

            if (project.Storylines == null || project.Storylines.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]Inga storylines ännu.[/]");
                DelayAndClear();
                return;
            }

            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("[#FFA500]Ordning[/]");
            table.AddColumn("[#FFA500]Titel[/]");
            table.AddColumn("[grey]Synopsis[/]");

            foreach (var s in project.Storylines.OrderBy(s => s.orderInProject))
            {
                table.AddRow(
                    s.orderInProject.ToString(),
                    s.Title ?? "",
                    s.Synopsis ?? "");
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("[grey italic]Tryck valfri tangent för att gå tillbaka...[/]");
            Console.ReadKey(true);
        }
        private void EditStoryline(Project project)
        {
            var s = SelectStoryline(project, "Välj storyline att ändra");
            if (s == null) return;

            Console.Clear();
            Info($"Ändra storyline: [#FFA500]{s.Title}[/]");

            string title = AnsiConsole.Ask<string>(
                $"[#FFA500]Titel[/] ([grey]{s.Title}[/], lämna tom för att behålla):");
            if (!string.IsNullOrWhiteSpace(title)) s.Title = title;

            string synopsis = AnsiConsole.Ask<string>(
                $"[#FFA500]Synopsis[/] ([grey]{s.Synopsis}[/], lämna tom för att behålla):");
            if (!string.IsNullOrWhiteSpace(synopsis)) s.Synopsis = synopsis;

            string theme = AnsiConsole.Ask<string>(
                $"[#FFA500]Tema[/] ([grey]{s.Theme}[/], lämna tom för att behålla):");
            if (!string.IsNullOrWhiteSpace(theme)) s.Theme = theme;

            string genre = AnsiConsole.Ask<string>(
                $"[#FFA500]Genre[/] ([grey]{s.Genre}[/], lämna tom för att behålla):");
            if (!string.IsNullOrWhiteSpace(genre)) s.Genre = genre;

            string story = AnsiConsole.Ask<string>(
                $"[#FFA500]Story[/] ([grey]{s.Story}[/], lämna tom för att behålla):");
            if (!string.IsNullOrWhiteSpace(story)) s.Story = story;

            string notes = AnsiConsole.Ask<string>(
                $"[#FFA500]Idéanteckningar[/] ([grey]{s.IdeaNotes}[/], lämna tom för att behålla):");
            if (!string.IsNullOrWhiteSpace(notes)) s.IdeaNotes = notes;

            string other = AnsiConsole.Ask<string>(
                $"[#FFA500]Övrig info[/] ([grey]{s.OtherInfo}[/], lämna tom för att behålla):");
            if (!string.IsNullOrWhiteSpace(other)) s.OtherInfo = other;

            s.dateOfLastEdit = DateTime.Now;
            _userService.SaveUserService();

            Result(true, "Storyline uppdaterad!");
            DelayAndClear();
        }
        private void DeleteStoryline(Project project)
        {
            var s = SelectStoryline(project, "Välj storyline att ta bort");
            if (s == null) return;

            var confirm = Menu($"Ta bort \"{s.Title}\"?", "Ja", "Nej");
            if (confirm != "Ja") return;

            project.Storylines.Remove(s);

            int ord = 1;
            foreach (var sl in project.Storylines.OrderBy(x => x.orderInProject))
                sl.orderInProject = ord++;

            _userService.SaveUserService();

            Result(true, "Storyline borttagen.");
            DelayAndClear();
        }
        private Storyline? SelectStoryline(Project project, string title)
        {
            if (project.Storylines == null || project.Storylines.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]Inga storylines ännu.[/]");
                DelayAndClear();
                return null;
            }

            var sorted = project.Storylines
                .OrderBy(s => s.orderInProject)
                .ToList();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<Storyline>()
                    .Title($"[bold]{title}[/]")
                    .PageSize(10)
                    .AddChoices(sorted)
                    .UseConverter(s => $"{s.orderInProject}. {s.Title}"));

            return choice;
        }
        public static void ShowLastProjectMenu()
        {
            Info("Senaste projekt");
            Console.WriteLine("Coming Soon");
            DelayAndClear();
        }
    }
}

