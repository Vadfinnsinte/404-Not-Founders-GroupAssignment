using Spectre.Console;
using _404_not_founders.Models;
using System;
using System.Collections.Generic;
using _404_not_founders.Services;

namespace _404_not_founders.Menus
{
    // Hjälpklass för menyer, UI och navigering.
    // Innehåller all interaktiv konsol-logik, och delegatar datalogik till t.ex. UserService eller User.
    public class MenuHelper
    {
        public const string MainTitleColor = "#FFA500";  // Används som grundfärg i UI för konsistens.
        private readonly UserService _userservice; // Huvudservice för att hämta/spara användardata.

        // Konstruktor: Kräver userservice för datalagring.
        public MenuHelper(UserService userservice) { _userservice = userservice; }

        /// <summary>
        /// Startar och driver programmets huvudflöde. växlar mellan inloggnings- och inloggade menyer.
        /// </summary>
        public void RunApp()
        {
            bool running = true, loggedin = false;
            string currentuser = null;
            var users = _userservice.Users; // Hämtar användarlistan från service.

            // Programloop – växlar beroende på inloggad-status.
            while (running)
            {
                if (!loggedin)
                    ShowLoginRegisterMenu(users, out loggedin, out currentuser, ref running);
                else
                    ShowLoggedInMenu(currentuser, ref loggedin, ref currentuser, ref running);
            }
            // Avslutningsmeddelande och grafisk tydlighet.
            Info("Tack för att du använde appen!");
            Info("Stänger ner...");
            DelayAndClear();
        }

        // ----- UI Helpers: samtliga metoder nedan används både här och externt (User.cs osv) -----

        // Vanlig meny med färgade val, anpassad för Spectre.console. 
        // Använd alltid denna för val/menyer så får ni UI-konsistens.
        public static string Menu(string title, params string[] choices) =>
            AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#{MainTitleColor}]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(choices)
                    .UseConverter(choice => $"[white]{choice}[/]"));

        // Skriver ut underrubrik med appens huvudfärg och fet stil.
        public static void Info(string text) => AnsiConsole.MarkupLine($"[underline {MainTitleColor}]{text}[/]");

        // Resultat-feedback, röd/grön, alltid understruket, för tydlig feedback/konsekvent stil.
        public static void Result(bool success, string text)
        {
            var color = success ? "green" : "red";
            AnsiConsole.MarkupLine($"[underline {MainTitleColor}][bold {color}]{text}[/][/]");
        }

        // Konsolinput – hanterar även "E" (exit/back) för konsekvens i navigationen.
        // Secret-param för t.ex. lösenordsfält.
        public static string AskInput(string prompt, bool secret = false)
        {
            var input = secret
                ? AnsiConsole.Prompt(new TextPrompt<string>(prompt).PromptStyle(MainTitleColor).Secret())
                : AnsiConsole.Ask<string>(prompt);
            if (input.Trim().Equals("E", StringComparison.OrdinalIgnoreCase)) return null;
            return input.Trim();
        }

        // Använd alltid denna efter feedback eller bekräftelse. rensar och skapar mjuka övergångar mellan views.
        public static void DelayAndClear(int ms = 800) { Thread.Sleep(ms); Console.Clear(); }

        /// <summary>
        /// Meny för inloggning, registrering, borttagning och avslut. hanterar loop och växling beroende på inloggningsstatus.
        /// </summary>
        /// <param name="users">Listan av alla användare</param>
        /// <param name="loggedin">Ref till inloggningsflagga</param>
        /// <param name="currentuser">Ref till nuvarande användare</param>
        /// <param name="running">Ref till huvudloopflagga</param>
        public void ShowLoginRegisterMenu(List<User> users, out bool loggedin, out string currentuser, ref bool running)
        {
            loggedin = false; currentuser = null;
            while (running)
            {
                Console.Clear();
                Info("Välkommen till Adventurer's Journal!");
                var choice = Menu("Välj ett alternativ", "Logga in", "Registrera dig", "Ta bort konto", "Avsluta");
                if (choice == "Avsluta") { running = false; return; }
                if (choice == "Logga in" && LoginMenu(users, out currentuser)) { loggedin = true; Console.Clear(); break; }
                if (choice == "Registrera dig")
                {
                    string newuser = null;
                    // Endast logga in om registrering returnerar true och har ett användarnamn.
                    if (User.RegisterMenu(users, out newuser, _userservice))
                    {
                        loggedin = true;
                        currentuser = newuser;
                        Console.Clear();
                        break;
                    }
                }
                if (choice == "Ta bort konto")
                {
                    // Anropar användar-menyhelper i User.cs för borttagning från Login-meny.
                    User.RemoveUserMenu(users, _userservice);
                }
            }
        }

        /// <summary>
        /// Inloggningsmeny, validerar Användarnamn+Lösen. 
        /// Stegbaserad input med back-funktion ("B"), exit ("E") och tydlig feedback.
        /// </summary>
        public static bool LoginMenu(List<User> users, out string loggedinuser)
        {
            loggedinuser = null;
            string username = "", password = ""; int step = 0;
            while (true)
            {
                Console.Clear();
                Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                Info("Logga in");
                if (step >= 1)
                    Info($"[grey]Användarnamn:[/] [#FFA500]{username}[/]");
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
                        loggedinuser = username;
                        return true;
                    }
                    Result(false, "Fel användarnamn eller lösenord!");
                    DelayAndClear(1200);
                    password = ""; step = 1;
                }
            }
        }

        /// <summary>
        /// Meny för inloggade – visar alla möjliga handlingar man som inloggad användare kan göra.
        /// Alla datamanipulationer är delegat till User.cs/metoder för separation och enkel testbarhet.
        /// </summary>
        public void ShowLoggedInMenu(string username, ref bool loggedin, ref string currentuser, ref bool running)
        {
            var users = _userservice.Users;
            var currentUserObj = users.Find(u => u.Username == username);

            while (running)
            {
                Console.Clear();
                Info($"[grey]Användarnamn:[/] [#FFA500]{username})[/]");
                var choice = Menu("Vad vill du göra?",
                    "Lägg till projekt", "Visa projekt", "Senaste projekt",
                    "Redigera konto", "Ta bort konto", "Logga ut", "Avsluta");

                switch (choice)
                {
                    case "Avsluta":
                        running = false; return;
                    case "Logga ut":
                        Result(true, "Du loggas ut...");
                        DelayAndClear();
                        loggedin = false;
                        currentuser = null;
                        return;
                    case "Lägg till projekt":
                        MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                        AddProject(currentUserObj, _userservice);
                        break;
                    case "Visa projekt":
                        ShowProjects(currentUserObj);
                        DelayAndClear();
                        break;
                    case "Senaste projekt":
                        ShowLastProject(currentUserObj);
                        DelayAndClear();
                        break;
                    case "Redigera konto":
                        currentUserObj?.EditCurrentUserMenu(_userservice, ref currentuser);
                        break;
                    case "Ta bort konto":
                        // Anrop till User-metoden. den nollställer LoggedIn och CurrentUser vid borttag.
                        currentUserObj?.RemoveCurrentUserMenu(_userservice, ref loggedin, ref currentuser);
                        // Om User är borta/loggad ut, gå tillbaka till login-menyn
                        if (!loggedin || currentuser == null)
                            Info("Du loggas nu ut...");
                        DelayAndClear();
                        return;
                        break;
                }
            }
        }



        // --- Projektfunktioner ---

        /// <summary>
        /// Adderar ett projekt till en användare. Har E och B-backning för användarvänlighet.
        /// </summary>
        public void AddProject(User user, UserService userService)
        {
            while (true)
            {
                Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                string title = AskInput("[#FFA500]Projektnamn:[/]");
                // E = exit/avbryt, tillbaka direkt
                if (string.IsNullOrWhiteSpace(title) || title.Equals("E", StringComparison.OrdinalIgnoreCase)) return;
                // B = backa, i detta enkla läge också exit, men kan bli step-logik
                if (title.Equals("B", StringComparison.OrdinalIgnoreCase)) return;
                user.Projects.Add(new Project { title = title, dateOfCreation = DateTime.Now });
                userService.SaveUserService();
                Result(true, $"Projekt '{title}' tillagt.");
                DelayAndClear(800);
                return;
            }
        }

        /// <summary>
        /// Visar alla projekt för en användare.
        /// </summary>
        public static void ShowProjects(User user)
        {
            Info($"Projekt för användare {user.Username}:");
            foreach (var project in user.Projects)
                Info($"- {project.title}");
            if (user.Projects.Count == 0)
                Info("Inga projekt inlagda.");
        }

        /// <summary>
        /// Visar senaste projektet.
        /// </summary>
        public static void ShowLastProject(User user)
        {
            if (user.Projects.Count == 0)
                Info("Inga projekt.");
            else
                Info($"Senaste projektet: {user.Projects[^1].title}");
        }


        // ---- Platshållare/under-menyer – för gruppexpansion, kan förfinas/utökas med moduler. ----
        public static void WorldMenu() { Info("Världsmenyn"); Console.WriteLine("Coming soon"); DelayAndClear(); }
        public static void CharacterMenu() { Info("Karaktärsmenyn"); Console.WriteLine("Coming soon"); DelayAndClear(); }
        public static void StoryineMenu() { Info("Storyline-Meny"); Console.WriteLine("Coming soon"); DelayAndClear(); }
        public static void ShowLastProjectMenu() { Info("Senaste Projekt"); Console.WriteLine("Coming soon"); DelayAndClear(); }
    }
}