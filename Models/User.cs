using _404_not_founders.Services;
using _404_not_founders.Menus;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace _404_not_founders.Models
{
    /// <summary>
    /// User-modellen beskriver en användare, dess kontodata och projekt.
    /// Här finns även alla interaktiva menyer kopplade till en användare.
    /// </summary>
    public class User
    {
        // --- DATAFÄLT ---
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Project> Projects { get; set; } = new();

        /// <summary>
        /// Registreringsmeny för ny användare.
        /// Sköter stegbaserad input och validering av fält mot redan existerande användare.
        /// E för exit, B för back fungerar under hela processen.
        /// </summary>
        public static bool RegisterMenu(List<User> users, out string registeredUser, UserService userService)
        {
            registeredUser = null;
            string email = "", username = "", password = "";
            int step = 0;
            while (true)
            {
                Console.Clear();
                MenuHelper.Info("[underline #FFA500]Registrera ny användare[/]");
                MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");

                if (step >= 1) MenuHelper.Info($"[grey]E-post:[/] [#FFA500]{email}[/]");
                if (step >= 2) MenuHelper.Info($"[grey]Användarnamn:[/] [#FFA500]{username}[/]");

                var value = step switch
                {
                    0 => MenuHelper.AskInput("[#FFA500]E-post:[/]"),
                    1 => MenuHelper.AskInput("[#FFA500]Användarnamn:[/]"),
                    2 => MenuHelper.AskInput("[#FFA500]Lösenord:[/]", true),
                    _ => null
                };

                // E = Exit/Avbryt hela registreringen (går tillbaka till login/register)
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Equals("E", StringComparison.OrdinalIgnoreCase))
                    return false;

                // B = Backa till föregående steg (eller avbryter om möjligt)
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

                // Fältvalidering
                if (step == 0)
                {
                    if (users.Exists(u => u.Email.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        MenuHelper.Result(false, "E-postadressen är redan registrerad."); MenuHelper.DelayAndClear(1200); continue;
                    }
                    email = value; step++;
                }
                else if (step == 1)
                {
                    if (users.Exists(u => u.Username.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        MenuHelper.Result(false, "Användarnamnet är redan taget."); MenuHelper.DelayAndClear(1200); continue;
                    }
                    username = value; step++;
                }
                else if (step == 2)
                {
                    password = value; step++;
                }

                // Bekräfta och skapa användaren
                if (step == 3)
                {
                    var confirm = MenuHelper.Menu("Vill du registrera denna användare?", "Ja", "Nej", "Avsluta");
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
                        userService.SaveUserService();
                        MenuHelper.Result(true, "Registreras...!");
                        MenuHelper.DelayAndClear(800);
                        registeredUser = username;
                        return true;
                    }
                    step = 0;
                }
            }
        }

        /// <summary>
        /// Skriver ut all info om denna user.
        /// </summary>
        public void Show()
        {
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Username: {Username}");
            Console.WriteLine($"Skapad: {CreationDate}");
            Console.WriteLine($"Projekt: {Projects.Count}");
        }

        /// <summary>
        /// Redigeringsmeny för nuvarande användare – kan ändra mail, namn, lösenord.
        /// E och B fungerar i varje prompt: E avslutar allt, B går bara ur pågående fältbyte utan att ändra.
        /// </summary>
        public void EditCurrentUserMenu(UserService userService, ref string username)
        {
            while (true)
            {
                Console.Clear();
                MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                MenuHelper.Info($"Redigera konto ({Username})");

                var choice = MenuHelper.Menu("Vad vill du ändra?", "E-post", "Användarnamn", "Lösenord", "Tillbaka");
                switch (choice)
                {
                    case "E-post":
                        MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                        string newEmail = MenuHelper.AskInput("Ny e-post:");
                        // E = gå ur redigering helt (tillbaka till huvudmenyn för inloggad)
                        if (string.IsNullOrWhiteSpace(newEmail) || newEmail.Equals("E", StringComparison.OrdinalIgnoreCase)) return;
                        // B = backa/avbryt e-postbyte, gå till huvudmeny för redigering
                        if (newEmail.Equals("B", StringComparison.OrdinalIgnoreCase)) break;
                        if (!userService.Users.Exists(u => u.Email == newEmail))
                        {
                            Email = newEmail;
                            userService.SaveUserService();
                            MenuHelper.Result(true, "E-post uppdaterad.");
                        }
                        else
                            MenuHelper.Result(false, "Ogiltig eller redan upptagen e-post.");
                        MenuHelper.DelayAndClear(1200);
                        break;

                    case "Användarnamn":
                        MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                        string newName = MenuHelper.AskInput("Nytt användarnamn:");
                        if (string.IsNullOrWhiteSpace(newName) || newName.Equals("E", StringComparison.OrdinalIgnoreCase)) return;
                        if (newName.Equals("B", StringComparison.OrdinalIgnoreCase)) break;
                        if (!userService.Users.Exists(u => u.Username == newName))
                        {
                            Username = newName;
                            username = newName;
                            userService.SaveUserService();
                            MenuHelper.Result(true, "Användarnamn uppdaterat.");
                        }
                        else
                            MenuHelper.Result(false, "Ogiltigt eller redan upptaget användarnamn.");
                        MenuHelper.DelayAndClear(1200);
                        break;

                    case "Lösenord":
                        MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                        string newPassword = MenuHelper.AskInput("Nytt lösenord:", true);
                        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Equals("E", StringComparison.OrdinalIgnoreCase)) return;
                        if (newPassword.Equals("B", StringComparison.OrdinalIgnoreCase)) break;
                        Password = newPassword;
                        userService.SaveUserService();
                        MenuHelper.Result(true, "Lösenord uppdaterat.");
                        MenuHelper.DelayAndClear(1200);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }

        /// <summary>
        /// Meny för att ta bort det nuvarande (inloggade) användarkontot.
        /// Bekräftar alltid lösenord och hanterar E och B för back/exit.
        /// </summary>
        public void RemoveCurrentUserMenu(UserService userService, ref bool loggedIn, ref string currentUser)
        {
            while (true)
            {
                Console.Clear();
                MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                var confirm = MenuHelper.Menu(
                    $"Är du säker på att du vill ta bort ditt konto '{Username}'?",
                    "Ja", "Nej", "Tillbaka");
                // Både "Nej" och "Tillbaka" ger tidig return
                if (confirm == "Nej" || confirm == "Tillbaka") return;
                if (confirm == "Ja")
                {
                    MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                    string password = MenuHelper.AskInput($"Ange lösenord för att ta bort kontot '{Username}':", true);
                    // E = avbryt hela borttagningsprocessen direkt
                    if (string.IsNullOrWhiteSpace(password) || password.Equals("E", StringComparison.OrdinalIgnoreCase)) return;
                    // B = fråga om man verkligen vill ta bort igen (dvs börja om confirmation-loopen)
                    if (password.Equals("B", StringComparison.OrdinalIgnoreCase)) continue;
                    // Fel lösenord = avbryt
                    if (password != Password)
                    {
                        MenuHelper.Result(false, "Fel lösenord! Konto tas inte bort.");
                        MenuHelper.DelayAndClear(1200);
                        return;
                    }
                    // Ta bort konto och logga ut
                    userService.Users.RemoveAll(u => u.Username == Username);
                    userService.SaveUserService();
                    MenuHelper.Result(true, "Ditt konto har tagits bort.");
                    MenuHelper.DelayAndClear(800);
                    loggedIn = false;
                    currentUser = null; // Detta gör att menyflödet leder dig till login/register
                    return;
                }
            }
        }

        /// <summary>
        /// Meny för att radera valfritt användarkonto från Login-menyn.
        /// Fungerar med E och B-backning på varje prompt!
        /// </summary>
        public static void RemoveUserMenu(List<User> users, UserService userService)
        {
            while (true)
            {
                Console.Clear();
                MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                MenuHelper.Info("[underline #FFA500]Existerande konton:[/]");
                foreach (var user in users)
                    MenuHelper.Info($"[white]- {user.Username}[/]");

                string delUsername = MenuHelper.AskInput("[#FFA500]Ange användarnamn på kontot som ska tas bort:[/]");
                if (string.IsNullOrWhiteSpace(delUsername) || delUsername.Equals("E", StringComparison.OrdinalIgnoreCase) || delUsername.Equals("B", StringComparison.OrdinalIgnoreCase)) return;
                var delUser = users.Find(u => u.Username == delUsername);
                if (delUser == null)
                {
                    MenuHelper.Result(false, "Användaren hittades inte."); MenuHelper.DelayAndClear(1200); return;
                }
                MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                string password = MenuHelper.AskInput($"Ange lösenord för konto '{delUser.Username}':", true);
                if (string.IsNullOrWhiteSpace(password) || password.Equals("E", StringComparison.OrdinalIgnoreCase) || password.Equals("B", StringComparison.OrdinalIgnoreCase)) return;
                if (password != delUser.Password)
                {
                    MenuHelper.Result(false, "Fel lösenord! Konto tas inte bort."); MenuHelper.DelayAndClear(1200); return;
                }
                var confirm = MenuHelper.Menu($"Bekräfta att du vill ta bort kontot '{delUsername}'?", "Ja", "Nej");
                if (confirm == "Ja")
                {
                    users.RemoveAll(u => u.Username == delUsername);
                    userService.SaveUserService();
                    MenuHelper.Result(true, $"Användare {delUsername} borttagen.");
                    MenuHelper.DelayAndClear(1200);
                }
                return;
            }
        }
    }
}
