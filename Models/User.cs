using _404_not_founders.Services;
using Spectre.Console;
using _404_not_founders.UI.Helpers;
using _404_not_founders.UI.Display;
using System.Text;

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
            int step = 0;

            // Loop to handle registration steps
            while (true)
            {
                // Visa instruktioner och nuvarande innehåll
                Console.Clear();
                ConsoleHelpers.Info("[#FFA500]Register new user[/]");
                ConsoleHelpers.InputInstruction(true);

                // Show current progress
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]E-mail:[/] [#FFA500]{email}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

                // Prompt for input based on the current step
                var value = step switch
                {
                    0 => ConsoleHelpers.AskInput("[#FFA500]E-mail:[/]"),
                    1 => ConsoleHelpers.AskInput("[#FFA500]Username:[/]"),
                    2 => ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true),
                    _ => null
                };

                // Check for exit command
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Equals("E", StringComparison.OrdinalIgnoreCase))
                    return (false, null);

                // Check for back command
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

                // Validate and store input
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

                // Confirms and saves the new user
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
            // Loop to handle editing steps
            while (true)
            {
                // Visa redigeringsmeny
                Console.Clear();
                ConsoleHelpers.Info("Redigera konto");
                ConsoleHelpers.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                // Local helper to render a boxed summary panel (consistent with other Edit* summaries)
                void ShowSummary(User u)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("[underline #FFA500]Account summary:[/]");
                    sb.AppendLine($"[grey]E-mail:[/]    [#FFA500]{(string.IsNullOrWhiteSpace(u.Email) ? "-" : u.Email)}[/]");
                    sb.AppendLine($"[grey]Username:[/] [#FFA500]{(string.IsNullOrWhiteSpace(u.Username) ? "-" : u.Username)}[/]");
                    var masked = string.IsNullOrEmpty(u.Password) ? "-" : new string('*', Math.Min(8, u.Password.Length));
                    sb.AppendLine($"[grey]Password:[/] [#FFA500]{masked}[/]");

                    var panel = new Panel(new Markup(sb.ToString()))
                    {
                        Border = BoxBorder.Rounded,
                        Padding = new Padding(1, 0, 1, 0),
                    };

                    AnsiConsole.Write(panel);
                    Console.WriteLine();
                }

                // Show live preview of current values above the choice menu
                ShowSummary(this);

                var choice = MenuChoises.Menu("What would you like to change?", "E-mail", "Username", "Password", "Back");

                // Handles the selected choice for editing user details
                switch (choice)
                {
                    case "E-mail":
                        // Prompt for new email
                        string newEmail = ConsoleHelpers.AskInput("[#FFA500]New E-mail:[/]");

                        // Handle exit or back commands
                        if (string.IsNullOrWhiteSpace(newEmail) || newEmail.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newEmail.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        // Confirm and apply the email change
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
                        // Ask for new username
                        string newName = ConsoleHelpers.AskInput("[#FFA500]New username:[/]");

                        // Handle exit or back commands
                        if (string.IsNullOrWhiteSpace(newName) || newName.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newName.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        // Confirm and apply the username change
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
                        // Ask for new password
                        string newPassword = ConsoleHelpers.AskInput("[#FFA500]New password:[/]", true);

                        // Handle exit or back commands
                        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                        if (newPassword.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                        // Confirm and apply the password change
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
                        return true; // Exit the edit loop and return to the previous menu
                }
            }
        }
    }
}
