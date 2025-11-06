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

            while (running) // Huvudloop
            {
                if (!loggedIn) // Om användaren inte är inloggad
                {
                    int choice = ShowLoginRegisterMenu(); // Visa inloggnings-/registreringsmeny

                    switch (choice) // Hantera menyval
                    {
                        case 1:
                            string loginUsername = AnsiConsole.Ask<string>("[yellow]Användarnamn:[/]"); // Inloggningslogik
                            string loginPassword = AnsiConsole.Prompt(
                                new TextPrompt<string>("[yellow]Lösenord:[/]") // Lösenordsprompt
                                    .PromptStyle("red")
                                    .Secret());

                            var user = users.Find(u => u.Username == loginUsername); // Hitta användare
                            if (user != null && user.Password == loginPassword) // Kontrollera lösenord
                            {
                                AnsiConsole.MarkupLine("[bold green]Inloggning lyckades![/]");
                                currentUser = loginUsername; // Sätt aktuell användare
                                loggedIn = true; // Sätt inloggningsstatus
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("[bold red]Fel användarnamn eller lösenord.[/]");
                            }
                            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                            Console.ReadKey();
                            break;
                        case 2:
                            User newUser = RegisterMenu(); // Registreringsmeny

                            if (users.Exists(u => u.Username == newUser.Username)) // Kontrollera om användarnamnet redan finns
                            {
                                AnsiConsole.MarkupLine("[bold red]Användarnamnet är redan taget. Försök igen.[/]");
                                AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                                Console.ReadKey();
                            }
                            else // Lägg till ny användare
                            {
                                users.Add(newUser); // Lägg till användare i listan
                                AnsiConsole.MarkupLine("[bold green]Registrering lyckades![/]");
                                AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                                Console.ReadKey();
                            }
                            break;
                        case 0:
                            running = false; // Avsluta appen
                            break;
                        default:
                            AnsiConsole.MarkupLine("[bold red]Ogiltigt val.[/]");
                            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                            Console.ReadKey();
                            break;
                    }
                }
                else // Om användaren är inloggad
                {
                    int choice = ShowLoggedInMenu(currentUser); // Visa meny för inloggad användare
                    switch (choice)
                    {
                        case 1:
                            AnsiConsole.MarkupLine("[blue]Lägger till projekt...[/]");
                            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                            Console.ReadKey();
                            break;
                        case 2:
                            AnsiConsole.MarkupLine("[blue]Visar alla projekt...[/]");
                            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                            Console.ReadKey();
                            break;
                        case 3:
                            AnsiConsole.MarkupLine("[blue]Visar senaste projektet...[/]");
                            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                            Console.ReadKey();
                            break;
                        case 4:
                            AnsiConsole.MarkupLine("[blue]Redigerar ditt konto...[/]");
                            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                            Console.ReadKey();
                            break;
                        case 0:
                            loggedIn = false; // Logga ut användaren
                            currentUser = null; // Rensa aktuell användare
                            break;
                        default:
                            AnsiConsole.MarkupLine("[bold red]Ogiltigt val.[/]");
                            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                            Console.ReadKey();
                            break;
                    }
                }
            }
            AnsiConsole.MarkupLine("[green]Tack för att du använde appen![/]");
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

        public static User RegisterMenu() // Registreringsmeny
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[underline blue]Registrera ny användare[/]"); // Rubrik
            string email = AnsiConsole.Ask<string>("[grey]E-post:[/]"); // Fråga efter e-post
            string username = AnsiConsole.Ask<string>("[grey]Användarnamn:[/]"); // Fråga efter användarnamn
            string password = AnsiConsole.Ask<string>("[grey]Lösenord:[/]"); // Fråga efter lösenord
            return new User // Skapa och returnera ny användare
            {
                Email = email,
                Username = username,
                Password = password,
                CreationDate = DateTime.Now, // Sätt skapelsedatum
                Projects = new List<Project>() // Tom lista över projekt
            };
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
