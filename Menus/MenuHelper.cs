using Spectre.Console;
using _404_not_founders.Models;
using System;
using System.Collections.Generic;

namespace _404_not_founders.Menus
{
    public class MenuHelper
    {
    public static void RunApp() // Huvudmetod för att köra appen
    {
        bool running = true; // Huvudloop
        bool loggedIn = false; // Inloggningsstatus
        string currentUser = null; // Aktuell användare
        List<User> users = new List<User>(); // Lista över användare

            while (running)
            {
                if (!loggedIn)
                {
                    int choice = ShowLoginRegisterMenu();

                    switch (choice)
                    {
                        case 1:
                            // Login via meny (med "Gå tillbaka"-stöd)
                            if (LoginMenu(users, out string loggedInName))
                            {
                                currentUser = loggedInName;
                                loggedIn = true;
                                Console.Clear();
                            }
                            else
                            {
                                // "Gå tillbaka" - gör inget, börjar om med huvudmeny direkt!
                                Console.Clear();
                            }
                            break;
                        case 2:
                            // Registrera via meny (med "Gå tillbaka"-stöd)
                            while (true)
                            {
                                User newUser = RegisterMenu();
                                if (newUser == null)
                                {
                                    // Användaren valde "Gå tillbaka" - gå till huvudmenyn
                                    Console.Clear();
                                    break;
                                }

                                if (users.Exists(u => u.Username == newUser.Username))
                                {
                                    AnsiConsole.MarkupLine("[bold red]Användarnamnet är redan taget. Försök igen.[/]");
                                    Thread.Sleep(1000);
                                    Console.Clear();
                                }
                                else
                                {
                                    users.Add(newUser);
                                    AnsiConsole.MarkupLine("[bold green]Registrering lyckades![/]");
                                    Thread.Sleep(800);
                                    currentUser = newUser.Username;
                                    loggedIn = true;
                                    Console.Clear();
                                    break; // Gå till logged in menu direkt!
                                }
                            }
                            break;
                        case 0:
                            running = false;
                            break;
                        default:
                            AnsiConsole.MarkupLine("[bold red]Ogiltigt val.[/]");
                            Thread.Sleep(1000);
                            Console.Clear();
                            break;
                    }
                }
                else
                {
                    int menuChoice = ShowLoggedInMenu(currentUser);
                    switch (menuChoice)
                    {
                        case 1:
                            AnsiConsole.MarkupLine("[blue]Lägger till projekt...[/]");
                            Thread.Sleep(800);
                            Console.Clear();
                            break;
                        case 2:
                            AnsiConsole.MarkupLine("[blue]Visar alla projekt...[/]");
                            Thread.Sleep(800);
                            Console.Clear();
                            break;
                        case 3:
                            AnsiConsole.MarkupLine("[blue]Visar senaste projektet...[/]");
                            Thread.Sleep(800);
                            Console.Clear();
                            break;
                        case 4:
                            AnsiConsole.MarkupLine("[blue]Redigerar ditt konto...[/]");
                            Thread.Sleep(800);
                            Console.Clear();
                            break;
                        case 0:
                            if (LogOutMenu())
                            {
                                loggedIn = false;
                                currentUser = null;
                                AnsiConsole.MarkupLine("[green]Du loggas ut...[/]");
                                Thread.Sleep(800);
                                Console.Clear();
                            }
                            else
                            {
                                Console.Clear();
                                // Användaren ångrade sig – gör ingenting, stanna i inloggad meny!
                            }
                            break;

                        default:
                            AnsiConsole.MarkupLine("[bold red]Ogiltigt val.[/]");
                            Thread.Sleep(1000);
                            Console.Clear();
                            break;
                    }
                }
            }
            AnsiConsole.MarkupLine("[green]Tack för att du använde appen![/]");
            AnsiConsole.MarkupLine("[green]Stänger ner...[/]");
            Thread.Sleep(1000);
            Console.Clear();
        }

        public static int ShowLoginRegisterMenu() // Visa inloggnings-/registreringsmeny
        {
            AnsiConsole.Clear();
            var menuChoice = AnsiConsole.Prompt( // Menyval
                new SelectionPrompt<string>() // Skapa meny
                    .Title("[green]Välj ett alternativ[/]")
                    .PageSize(3)
                    .AddChoices(new[] { "Logga in", "Registrera dig", "Avsluta" })
            );
            if (menuChoice == "Logga in") return 1;
            if (menuChoice == "Registrera dig") return 2;
            if (menuChoice == "Avsluta") return 0;
            return -1;
        }

        public static bool LoginMenu(List<User> users, out string loggedInUser)
        {
            loggedInUser = null;

            while (true)
            {
                AnsiConsole.Clear();
                var menuChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Välj ett alternativ:")
                        .AddChoices(new[] { "Logga in", "Gå tillbaka" })
                );
                if (menuChoice == "Gå tillbaka")
                    return false;

                // Fortsätt med login
                string username = AnsiConsole.Ask<string>("[yellow]Användarnamn:[/]");
                string password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]Lösenord:[/]")
                        .PromptStyle("red")
                        .Secret());

                var user = users.Find(u => u.Username == username);
                if (user != null && user.Password == password)
                {
                    AnsiConsole.MarkupLine("[bold green]Loggar in...[/]");
                    Thread.Sleep(800);
                    loggedInUser = username;
                    return true;
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]Fel användarnamn eller lösenord![/]");
                    Thread.Sleep(1200);
                }
            }
        }

        public static User RegisterMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[underline blue]Registrera ny användare[/]");
                var menuChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Välj ett alternativ:")
                        .AddChoices(new[] { "Registrera", "Gå tillbaka" })
                );
                if (menuChoice == "Gå tillbaka")
                    return null;

                // Fortsätt med registrering
                string email = AnsiConsole.Ask<string>("[grey]E-post:[/]");
                string username = AnsiConsole.Ask<string>("[grey]Användarnamn:[/]");
                string password = AnsiConsole.Ask<string>("[grey]Lösenord:[/]");

                // Bekräfta registrering
                var confirm = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Vill du registrera denna användare?")
                        .AddChoices(new[] { "Ja", "Nej, börja om" })
                );
                if (confirm == "Ja")
                {
                    return new User
                    {
                        Email = email,
                        Username = username,
                        Password = password,
                        CreationDate = DateTime.Now,
                        Projects = new List<Project>()
                    };
                }
                // Om "Nej, börja om", loopen startar om!
            }
        }

        public static int ShowLoggedInMenu(string username) // Meny för inloggad användare
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold green]Inloggad som {username}[/]"); // Visa aktuell användare
            var menuChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>() // Menyval
                    .Title("Vad vill du göra nu?")
                    .AddChoices(new[] {
                        "Lägg till projekt",
                        "Visa projekt",
                        "Senaste projekt",
                        "Redigera konto",
                        "Logga ut"
                    }));

            if (menuChoice == "Lägg till projekt") return 1; // Hantera menyval
            if (menuChoice == "Visa projekt") return 2; // Hantera menyval
            if (menuChoice == "Senaste projekt") return 3; // Hantera menyval
            if (menuChoice == "Redigera konto") return 4; // Hantera menyval
            if (menuChoice == "Logga ut") return 0; // Hantera menyval
            return -1; // Ogiltigt val
        }
        public static bool LogOutMenu()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Vill du logga ut?[/]")
                    .AddChoices(new[] { "Ja, logga ut", "Nej, gå tillbaka" })
            );
            return choice == "Ja, logga ut";
        }

        public void ShowProjectMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void ProjectMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void UserMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void WorldMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void CharacterMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void StorylineMenu()
        {
            Console.WriteLine("Coming Soon");
        }
    }
}
