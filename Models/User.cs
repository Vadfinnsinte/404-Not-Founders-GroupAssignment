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

        // ------- LÄGG IN HÄR -------
        public static (bool success, string registeredUser) RegisterUser(List<User> users, UserService userService)
        {
            string registeredUser = null;
            string email = "", username = "", password = "";
            int step = 0; // Steg i formuläret

            while (true)
            {
                // Visa instruktioner och nuvarande innehåll
                Console.Clear();
                ConsoleHelpers.Info("[#FFA500]Register new user[/]");
                ConsoleHelpers.InputInstruction(true);

                // Visa tidigare ifyllda fält i processen
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]E-mail:[/] [#FFA500]{email}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

                // Select input prompt based on steg
                var value = step switch
                {
                    0 => ConsoleHelpers.AskInput("[#FFA500]E-mail:[/]"),
                    1 => ConsoleHelpers.AskInput("[#FFA500]Username:[/]"),
                    2 => ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true),
                    _ => null
                };

                // Avbryt hela processen
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Equals("E", StringComparison.OrdinalIgnoreCase))
                    return (false, null);

                // Gå bakåt i stegen
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

                // Validera email
                if (step == 0)
                {
                    if (users.Exists(u => u.Email.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        ConsoleHelpers.Result(false, "Email address is already registered.");
                        ConsoleHelpers.DelayAndClear(1200);
                        continue;
                    }
                    email = value; step++;
                }
                // Validera username
                else if (step == 1)
                {
                    if (users.Exists(u => u.Username.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        ConsoleHelpers.Result(false, "The username is already taken.");
                        ConsoleHelpers.DelayAndClear(1200);
                        continue;
                    }
                    username = value; step++;
                }
                // Spara lösenord och gå vidare
                else if (step == 2)
                {
                    password = value; step++;
                }

                // När alla fält är ifyllda: bekräfta
                if (step == 3)
                {
                    var confirm = MenuChoises.Menu("Do you want to register this user?", "Yes", "No");
                    if (confirm == "No") { step = 0; continue; }
                    if (confirm == "Yes")
                    {
                        // Skapa och spara användare
                        users.Add(new User
                        {
                            Email = email,
                            Username = username,
                            Password = password,
                            CreationDate = DateTime.Now,
                            Projects = new List<Project>()
                        });
                        userService.SaveUserService();
                        ConsoleHelpers.Result(true, "Registering user...!");
                        ConsoleHelpers.DelayAndClear(800);
                        registeredUser = username;
                        return (true, registeredUser);
                    }
                    step = 0;
                }
            }
        }
        // ------- SLUT RegisterUser -------


        /// Redigera användare. Returnerar tuple (om edit är färdig, och eventuellt nytt username)
        public (bool finished, string updatedUser) EditUser(UserService userService, string username)
        {
            while (true)
            {
                // Visa redigeringsmeny
                Console.Clear();
                ConsoleHelpers.Info("Redigera konto");
                ConsoleHelpers.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                var choice = MenuChoises.Menu("What would you like to change?", "E-mail", "Username", "Password", "Back");

                switch (choice)
                {
                    case "E-mail":
                        string newEmail = ConsoleHelpers.AskInput("[#FFA500]New E-mail:[/]");
                        if (string.IsNullOrWhiteSpace(newEmail) || newEmail.Equals("E", StringComparison.OrdinalIgnoreCase))
                            return (false, username);
                        if (newEmail.Equals("B", StringComparison.OrdinalIgnoreCase))
                            break;
                        var confirmEmail = MenuChoises.Menu($"Confirm change to  \"{newEmail}\"?", "Yes", "No");
                        if (confirmEmail == "Yes")
                        {
                            if (!userService.Users.Exists(u => u.Email == newEmail))
                            {
                                Email = newEmail;
                                userService.SaveUserService();
                                ConsoleHelpers.Result(true, "E-mail updated.");
                            }
                            else
                            {
                                ConsoleHelpers.Result(false, "Invalid or already taken E-mail.");
                            }
                            ConsoleHelpers.DelayAndClear(1200);
                        }
                        break;

                    case "Username":
                        string newName = ConsoleHelpers.AskInput("[#FFA500]New username:[/]");
                        if (string.IsNullOrWhiteSpace(newName) || newName.Equals("E", StringComparison.OrdinalIgnoreCase))
                            return (false, username);
                        if (newName.Equals("B", StringComparison.OrdinalIgnoreCase))
                            break;
                        var confirmName = MenuChoises.Menu($"Confirm change \"{newName}\"?", "Yes", "No");
                        if (confirmName == "Yes")
                        {
                            if (!userService.Users.Exists(u => u.Username == newName))
                            {
                                Username = newName;
                                userService.SaveUserService();
                                ConsoleHelpers.Result(true, $"Username changed to {newName}.");
                                username = newName; // Uppdatera och returnera nya namnet
                            }
                            else
                            {
                                ConsoleHelpers.Result(false, "Invalid or already taken username.");
                            }
                            ConsoleHelpers.DelayAndClear(1200);
                        }
                        break;

                    case "Password":
                        string newPassword = ConsoleHelpers.AskInput("[#FFA500]New password:[/]", true);
                        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Equals("E", StringComparison.OrdinalIgnoreCase))
                            return (false, username);
                        if (newPassword.Equals("B", StringComparison.OrdinalIgnoreCase))
                            break;
                        var confirmPass = MenuChoises.Menu("Confirm change to new password?", "Yes", "No");
                        if (confirmPass == "Yes")
                        {
                            Password = newPassword;
                            userService.SaveUserService();
                            ConsoleHelpers.Result(true, "Password updated.");
                            ConsoleHelpers.DelayAndClear(1200);
                        }
                        break;

                    case "Back":
                        // Klar med edit, returnera status och username
                        return (true, username);
                }
            }
        }

        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
