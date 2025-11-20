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
        public List<Project> Projects { get; set; } = new List<Project>();
        public Guid? LastSelectedProjectId { get; set; }

        public static bool RegisterUser(List<User> users, out string registeredUser, UserService userService)
        {
            registeredUser = null;
            string email = "", username = "", password = "";
            int step = 0;
            while (true)
            {
                Console.Clear();
                MenuHelper.Info("[underline #FFA500]Register new user[/]");
                MenuHelper.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                if (step >= 1) MenuHelper.Info($"[grey]E-mail:[/] [#FFA500]{email}[/]");
                if (step >= 2) MenuHelper.Info($"[grey]Username:[/] [#FFA500]{username}[/]");

                var value = step switch
                {
                    0 => MenuHelper.AskInput("[#FFA500]E-mail:[/]"),
                    1 => MenuHelper.AskInput("[#FFA500]Username:[/]"),
                    2 => MenuHelper.AskInput("[#FFA500]Password:[/]", true),
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
                        MenuHelper.Result(false, "Email address is already registered."); MenuHelper.DelayAndClear(1200); continue;
                    }
                    email = value; step++;
                }
                else if (step == 1)
                {
                    if (users.Exists(u => u.Username.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        MenuHelper.Result(false, "The username is already taken."); MenuHelper.DelayAndClear(1200); continue;
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
                    var confirm = MenuHelper.Menu("Do you want to register this user?", "Yes", "No", "Exit");
                    if (confirm == "Exit") Environment.Exit(0);
                    if (confirm == "No") { step = 0; continue; }
                    if (confirm == "Yes")
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
                        MenuHelper.Result(true, "Registering user...!");
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
