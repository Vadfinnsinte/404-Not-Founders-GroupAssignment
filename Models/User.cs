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
                ConsoleHelpers.Info("[#FFA500]Register new user[/]");
                ConsoleHelpers.InputInstruction(true);

                if (step >= 1) AnsiConsole.MarkupLine($"[grey]E-mail:[/] [#FFA500]{email}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

                var value = step switch
                {
                    0 => ConsoleHelpers.AskInput("[#FFA500]E-mail:[/]"),
                    1 => ConsoleHelpers.AskInput("[#FFA500]Username:[/]"),
                    2 => ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true),
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
                        ConsoleHelpers.Result(false, "Email address is already registered."); ConsoleHelpers.DelayAndClear(1200); continue;
                    }
                    email = value; step++;
                }
                else if (step == 1)
                {
                    if (users.Exists(u => u.Username.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        ConsoleHelpers.Result(false, "The username is already taken."); ConsoleHelpers.DelayAndClear(1200); continue;
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
                    var confirm = MenuChoises.Menu("Do you want to register this user?", "Yes", "No");
             
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
                        ConsoleHelpers.Result(true, "Registering user...!");
                        ConsoleHelpers.DelayAndClear(800);
                        registeredUser = username;
                        return true;
                    }
                    step = 0;
                }
            }
        }

        public bool EditUser(UserService userService, ref string username)
        {
            // Work on a temporary copy so changes are visible before applying
            var temp = new User
            {
                Email = this.Email,
                Username = this.Username,
                Password = this.Password,
                CreationDate = this.CreationDate,
                Projects = this.Projects,
                LastSelectedProjectId = this.LastSelectedProjectId
            };

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Redigera konto");
                ConsoleHelpers.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                // Show live preview of temp values above the choice menu
                AnsiConsole.MarkupLine($"[grey]E-mail:[/] [#FFA500]{(string.IsNullOrWhiteSpace(temp.Email) ? "-" : temp.Email)}[/]");
                AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{(string.IsNullOrWhiteSpace(temp.Username) ? "-" : temp.Username)}[/]");
                // Mask password in the preview
                var masked = string.IsNullOrEmpty(temp.Password) ? "-" : new string('*', Math.Min(8, temp.Password.Length));
                AnsiConsole.MarkupLine($"[grey]Password:[/] [#FFA500]{masked}[/]");
                Console.WriteLine();

                var choice = MenuChoises.Menu("What would you like to change?", "E-mail", "Username", "Password", "Done", "Back");
                switch (choice)
                {
                    case "E-mail":
                        {
                            string newEmail = ConsoleHelpers.AskInput("[#FFA500]New E-mail:[/]");
                            if (string.IsNullOrWhiteSpace(newEmail) || newEmail.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                            if (newEmail.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                            var confirmEmail = MenuChoises.Menu($"Confirm change to  \"{newEmail}\"?", "Yes", "No");
                            if (confirmEmail == "Yes")
                            {
                                if (!userService.Users.Exists(u => u.Email == newEmail && u != this))
                                {
                                    temp.Email = newEmail;
                                    ConsoleHelpers.Result(true, "E-mail updated (preview).");
                                }
                                else
                                    ConsoleHelpers.Result(false, "Invalid or already taken E-mail.");
                                ConsoleHelpers.DelayAndClear(1200);
                            }
                        }
                        break;

                    case "Username":
                        {
                            string newName = ConsoleHelpers.AskInput("[#FFA500]New username:[/]");
                            if (string.IsNullOrWhiteSpace(newName) || newName.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                            if (newName.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                            var confirmName = MenuChoises.Menu($"Confirm change \"{newName}\"?", "Yes", "No");
                            if (confirmName == "Yes")
                            {
                                if (!userService.Users.Exists(u => u.Username == newName && u != this))
                                {
                                    temp.Username = newName;
                                    ConsoleHelpers.Result(true, $"Username updated (preview).");
                                }
                                else
                                    ConsoleHelpers.Result(false, "Invalid or already taken username.");
                                ConsoleHelpers.DelayAndClear(1200);
                            }
                        }
                        break;

                    case "Password":
                        {
                            string newPassword = ConsoleHelpers.AskInput("[#FFA500]New password:[/]", true);
                            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Equals("E", StringComparison.OrdinalIgnoreCase)) return false;
                            if (newPassword.Equals("B", StringComparison.OrdinalIgnoreCase)) break;

                            var confirmPass = MenuChoises.Menu("Confirm change to new password?", "Yes", "No");
                            if (confirmPass == "Yes")
                            {
                                temp.Password = newPassword;
                                ConsoleHelpers.Result(true, "Password updated (preview).");
                                ConsoleHelpers.DelayAndClear(1200);
                            }
                        }
                        break;

                    case "Done":
                        {
                            Console.Clear();
                            ConsoleHelpers.Info("Account summary:");
                            AnsiConsole.MarkupLine($"[grey]E-mail:[/] [#FFA500]{temp.Email}[/]");
                            AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{temp.Username}[/]");
                            AnsiConsole.MarkupLine($"[grey]Password:[/] [#FFA500]{(string.IsNullOrEmpty(temp.Password) ? "-" : new string('*', Math.Min(8, temp.Password.Length)))}[/]");

                            Console.WriteLine();
                            var confirmAll = MenuChoises.Menu("Are you happy with these changes?", "Yes", "No (Start over)", "Exit");

                            if (confirmAll == "Exit")
                            {
                                ConsoleHelpers.DelayAndClear();
                                return false;
                            }

                            if (confirmAll == "No (Start over)")
                            {
                                temp.Email = this.Email;
                                temp.Username = this.Username;
                                temp.Password = this.Password;
                                continue;
                            }

                            if (confirmAll == "Yes")
                            {
                                // Apply changes to the real user and persist
                                this.Email = temp.Email;
                                this.Username = temp.Username;
                                this.Password = temp.Password;
                                username = temp.Username; // update external ref
                                userService.SaveUserService();
                                ConsoleHelpers.Result(true, "Account updated.");
                                ConsoleHelpers.DelayAndClear(1200);
                                return true;
                            }
                        }
                        break;

                    case "Back":
                        return true; // keep existing navigation behavior
                }
            }
        }


        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
