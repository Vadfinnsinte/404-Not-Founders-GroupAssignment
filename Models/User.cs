using _404_not_founders.Services;
using Spectre.Console;
using _404_not_founders.UI.Helpers;
using _404_not_founders.UI.Display;
using System.Text;

namespace _404_not_founders.Models
{
    /// Represents an application user with credentials, projects and metadata.
    /// Handles registration and account editing logic.
    public class User
    {
        // Basic user data and relationships
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Project> Projects { get; set; } = new List<Project>();
        public Guid? LastSelectedProjectId { get; set; }

        /// Step-by-step registration flow for a new user.
        /// Validates uniqueness of e-mail and username and saves via UserService.
        /// Returns a tuple (success, registeredUserName).
        public static (bool success, string registeredUser) RegisterUser(List<User> users, UserService userService)
        {
            string registeredUser = null;
            string email = "", username = "", password = "";
            int step = 0;

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("[#FFA500]Register new user[/]");
                ConsoleHelpers.InputInstruction(true);

                // Show already entered values
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]E-mail:[/] [#FFA500]{Markup.Escape(email)}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{Markup.Escape(username)}[/]");

                // Ask for current step input
                var value = step switch
                {
                    0 => ConsoleHelpers.AskInput("[#FFA500]E-mail:[/]"),
                    1 => ConsoleHelpers.AskInput("[#FFA500]Username:[/]"),
                    2 => ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true),
                    _ => null
                };

                // Exit on empty or 'E'
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Equals("E", StringComparison.OrdinalIgnoreCase))
                    return (false, null);

                // Go one step back on 'B'
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

                if (step == 0)
                {
                    // Check if e-mail already exists
                    if (users.Exists(u => u.Email.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        ConsoleHelpers.Result(false, "Email address is already registered.");
                        ConsoleHelpers.DelayAndClear(1200);
                        continue;
                    }
                    email = value;
                    step++;
                }
                else if (step == 1)
                {
                    // Check if username already exists
                    if (users.Exists(u => u.Username.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        ConsoleHelpers.Result(false, "The username is already taken.");
                        ConsoleHelpers.DelayAndClear(1200);
                        continue;
                    }
                    username = value;
                    step++;
                }
                else if (step == 2)
                {
                    password = value;
                    step++;
                }

                // Confirmation step after all inputs are collected
                if (step == 3)
                {
                    var confirm = MenuChoises.Menu("Do you want to register this user?", "Yes", "No");

                    if (confirm == "No")
                    {
                        // Start over from step 0
                        step = 0;
                        email = "";
                        username = "";
                        password = "";
                        continue;
                    }

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
                        ConsoleHelpers.Result(true, "Registering user...!");
                        ConsoleHelpers.DelayAndClear(800);
                        registeredUser = username;
                        return (true, registeredUser);
                    }

                    step = 0;
                }
            }
        }

        /// Interactive menu to edit the current user's e-mail, username or password.
        /// Returns (finished, updatedUserName) where finished indicates if the user left via Back.
        public (bool finished, string updatedUser) EditUser(UserService userService, string username)
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("Redigera konto");
                ConsoleHelpers.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                // Local function to display current account summary
                void ShowSummary(User u)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("[underline #FFA500]Account summary:[/]");
                    sb.AppendLine($"[grey]E-mail:[/]    [#FFA500]{(string.IsNullOrWhiteSpace(u.Email) ? "-" : Markup.Escape(u.Email))}[/]");
                    sb.AppendLine($"[grey]Username:[/] [#FFA500]{(string.IsNullOrWhiteSpace(u.Username) ? "-" : Markup.Escape(u.Username))}[/]");

                    var masked = string.IsNullOrEmpty(u.Password)
                        ? "-"
                        : new string('*', Math.Min(8, u.Password.Length));

                    sb.AppendLine($"[grey]Password:[/] [#FFA500]{masked}[/]");

                    var panel = new Panel(new Markup(sb.ToString()))
                    {
                        Border = BoxBorder.Rounded,
                        Padding = new Padding(1, 0, 1, 0),
                    };

                    AnsiConsole.Write(panel);
                    Console.WriteLine();
                }

                // Show current data before each choice
                ShowSummary(this);

                var choice = MenuChoises.Menu(
                    "What would you like to change?",
                    "E-mail",
                    "Username",
                    "Password",
                    "Back");

                switch (choice)
                {
                    case "E-mail":
                        {
                            string newEmail = ConsoleHelpers.AskInput("[#FFA500]New E-mail:[/]");

                            // Exit without changes
                            if (string.IsNullOrWhiteSpace(newEmail) ||
                                newEmail.Equals("E", StringComparison.OrdinalIgnoreCase))
                                return (false, username);

                            // Cancel this change only
                            if (newEmail.Equals("B", StringComparison.OrdinalIgnoreCase))
                                break;

                            var confirmEmail = MenuChoises.Menu($"Confirm change to \"{newEmail}\"?", "Yes", "No");
                            if (confirmEmail == "Yes")
                            {
                                // Check uniqueness of new e-mail
                                if (!userService.Users.Exists(u => u.Email.Equals(newEmail, StringComparison.OrdinalIgnoreCase)))
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
                        }

                    case "Username":
                        {
                            string newName = ConsoleHelpers.AskInput("[#FFA500]New username:[/]");

                            if (string.IsNullOrWhiteSpace(newName) ||
                                newName.Equals("E", StringComparison.OrdinalIgnoreCase))
                                return (false, username);

                            if (newName.Equals("B", StringComparison.OrdinalIgnoreCase))
                                break;

                            var confirmName = MenuChoises.Menu($"Confirm change \"{newName}\"?", "Yes", "No");
                            if (confirmName == "Yes")
                            {
                                // Check uniqueness of new username
                                if (!userService.Users.Exists(u => u.Username.Equals(newName, StringComparison.OrdinalIgnoreCase)))
                                {
                                    Username = newName;
                                    userService.SaveUserService();
                                    ConsoleHelpers.Result(true, $"Username changed to {newName}.");
                                    username = newName;
                                }
                                else
                                {
                                    ConsoleHelpers.Result(false, "Invalid or already taken username.");
                                }
                                ConsoleHelpers.DelayAndClear(1200);
                            }
                            break;
                        }

                    case "Password":
                        {
                            string newPassword = ConsoleHelpers.AskInput("[#FFA500]New password:[/]", true);

                            if (string.IsNullOrWhiteSpace(newPassword) ||
                                newPassword.Equals("E", StringComparison.OrdinalIgnoreCase))
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
                        }

                    case "Back":
                        // User leaves the edit menu, return current username (may have changed)
                        return (true, username);
                }
            }
        }
    }
}
