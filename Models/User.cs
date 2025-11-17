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



        public void Show()
        {
            Console.WriteLine("Coming soon");
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
