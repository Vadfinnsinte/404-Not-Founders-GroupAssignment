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
                MenuHelper.Info("[#FFA500]Register new user[/]");
                MenuHelper.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                if (step >= 1) AnsiConsole.MarkupLine($"[grey]E-mail:[/] [#FFA500]{email}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

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

        public bool EditUser(UserService userService, ref string username)
        {
            while (true)
            {
                Console.Clear();
                MenuHelper.Info($"Redigera konto");
                MenuHelper.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                var choice = MenuHelper.Menu("What would you like to change?", "E-mail", "Username", "Password", "Back");
                switch (choice)
                {
                    case "E-mail":
                        string newEmail = MenuHelper.AskInput("[#FFA500]New E-mail:[/]");
                        if (string.IsNullOrWhiteSpace(newEmail) || newEmail.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newEmail.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        var confirmEmail = MenuHelper.Menu($"Confirm change to  \"{newEmail}\"?", "Yes", "No");
                        if (confirmEmail == "Yes")
                        {
                            if (!userService.Users.Exists(u => u.Email == newEmail))
                            {
                                Email = newEmail;
                                userService.SaveUserService();
                                MenuHelper.Result(true, "E-mail updated.");
                            }
                            else
                                MenuHelper.Result(false, "Invalid or already taken E-mail.");
                            MenuHelper.DelayAndClear(1200);
                        }
                        break;

                    case "Username":
                        string newName = MenuHelper.AskInput("[#FFA500]New username:[/]");
                        if (string.IsNullOrWhiteSpace(newName) || newName.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newName.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        var confirmName = MenuHelper.Menu($"Confirm change \"{newName}\"?", "Yes", "No");
                        if (confirmName == "No")
                        {
                            if (!userService.Users.Exists(u => u.Username == newName))
                            {
                                Username = newName;
                                username = newName;
                                userService.SaveUserService();
                                MenuHelper.Result(true, $"Username changed to {newName}.");
                            }
                            else
                                MenuHelper.Result(false, "Invalid or already taken username.");
                            MenuHelper.DelayAndClear(1200);
                        }
                        break;

                    case "Password":
                        string newPassword = MenuHelper.AskInput("[#FFA500]New password:[/]", true);
                        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newPassword.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        var confirmPass = MenuHelper.Menu("Confirm change to new password?", "Yes", "No");
                        if (confirmPass == "Yes")
                        {
                            Password = newPassword;
                            userService.SaveUserService();
                            MenuHelper.Result(true, "Password updated.");
                            MenuHelper.DelayAndClear(1200);
                        }
                        break;

                    case "Back":
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
