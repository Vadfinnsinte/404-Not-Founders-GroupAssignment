using _404_not_founders.Services;
using _404_not_founders.Menus;
using _404_not_founders.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace _404_not_founders.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Project> Projects { get; set; }
        public Guid? LastSelectedProjectId { get; set; }

        public static bool RegisterUser(List<User> users, out string registeredUser, UserService userService)
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

        public bool EditUser(UserService userService, ref string username)
        {
            while (true)
            {
                Console.Clear();
                MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
                AnsiConsole.MarkupLine($"Redigera konto ({Username})");

                var choice = MenuHelper.Menu("Vad vill du ändra?", "E-post", "Användarnamn", "Lösenord", "Tillbaka");
                switch (choice)
                {
                    case "E-post":
                        string newEmail = MenuHelper.AskInput("[#FFA500]Ny E-post:[/]");
                        if (string.IsNullOrWhiteSpace(newEmail) || newEmail.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newEmail.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        var confirmEmail = MenuHelper.Menu($"Bekräfta ändring till \"{newEmail}\"?", "Ja", "Nej");
                        if (confirmEmail == "Ja")
                        {
                            if (!userService.Users.Exists(u => u.Email == newEmail))
                            {
                                Email = newEmail;
                                userService.SaveUserService();
                                MenuHelper.Result(true, "E-post uppdaterad.");
                            }
                            else
                                MenuHelper.Result(false, "Ogiltig eller redan upptagen e-post.");
                            MenuHelper.DelayAndClear(1200);
                        }
                        break;

                    case "Användarnamn":
                        string newName = MenuHelper.AskInput("[#FFA500]Nytt användarnamn:[/]");
                        if (string.IsNullOrWhiteSpace(newName) || newName.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newName.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        var confirmName = MenuHelper.Menu($"Bekräfta ändring till \"{newName}\"?", "Ja", "Nej");
                        if (confirmName == "Ja")
                        {
                            if (!userService.Users.Exists(u => u.Username == newName))
                            {
                                Username = newName;
                                username = newName;
                                userService.SaveUserService();
                                MenuHelper.Result(true, $"Användarnamn uppdaterat till {newName}.");
                            }
                            else
                                MenuHelper.Result(false, "Ogiltigt eller redan upptaget användarnamn.");
                            MenuHelper.DelayAndClear(1200);
                        }
                        break;

                    case "Lösenord":
                        string newPassword = MenuHelper.AskInput("[#FFA500]Nytt lösenord:[/]", true);
                        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newPassword.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        var confirmPass = MenuHelper.Menu("Bekräfta ändring till nytt lösenord?", "Ja", "Nej");
                        if (confirmPass == "Ja")
                        {
                            Password = newPassword;
                            userService.SaveUserService();
                            MenuHelper.Result(true, "Lösenord uppdaterat.");
                            MenuHelper.DelayAndClear(1200);
                        }
                        break;

                    case "Tillbaka":
                        return true; // Bara här visas inloggningsfeedback!
                }
            }
        }


        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
