using System;
using _404_not_founders.Menus;
using _404_not_founders.Services;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using _404_not_founders.UI.Helpers;
using _404_not_founders.UI.Display;


namespace _404_not_founders.Models
{
    public class Character
    {
        public string Name { get; set; }
        public string Race { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public string OtherInfo { get; set; }


        public string ChracterMenu1(string title, params string[] choices) =>
            AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#{MainTitleColor}]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(choices)
                    .UseConverter(choice => $"[white]{choice}[/]")
            );

        public void menh()
        {

            MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
            if (MenuHelper.ReadBackOrExit() == "E")
            {
                Environment.Exit(0);

            }
                return;
            if (MenuHelper.ReadBackOrExit() == "B")
            {
                ChracterMenu2();
                return;
            }
        }



        public void ChracterMenu2()
        {
            UserService userService = new UserService();
            MenuHelper menuHelper = new MenuHelper(new UserService());
            var choice = ChracterMenu1("Character Menu", "Add Character", "Show Character", "Change Character", "Delete Character", "Back to Main Menu");
            switch (choice)
            {
               
                case "Add Character":

                    Add();
                    ChracterMenu2();
                    break;
                case "Show Character":
                    Show();
                    ChracterMenu2();
                    break;
                case "Change Character":
                    Change();
                    ChracterMenu2();
                    break;
                case "Delete Character":
                    Delete();
                    ChracterMenu2();
                    break;
                case "Back to Main Menu":
                    bool loggedIn = true;
                    string currentUser = Names; 
                    bool running = true;
                    menuHelper.ShowLoggedInMenu(currentUser, ref loggedIn, ref currentUser, ref running);
                    break;
            }
        }



        public void Add()
        {
            string name = "", race = "", description = "", gender = "", @class = "", otherInfo = "";
            int age = 0, level = 0;
            int step = 0; // 0 = name, 1 = race, 2 = description, 3 = gender, 4 = age, 5 = level, 6 = class, 7 = otherInfo, 8 = confirm

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("Create New Character");
                ConsoleHelpers.InputInstruction(true);

                // Show already filled values for context
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{name}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Race:[/] [#FFA500]{race}[/]");
                if (step >= 3) AnsiConsole.MarkupLine($"[grey]Description:[/] [#FFA500]{description}[/]");
                if (step >= 4) AnsiConsole.MarkupLine($"[grey]Gender:[/] [#FFA500]{gender}[/]");
                if (step >= 5) AnsiConsole.MarkupLine($"[grey]Age:[/] [#FFA500]{age}[/]");
                if (step >= 6) AnsiConsole.MarkupLine($"[grey]Level:[/] [#FFA500]{level}[/]");
                if (step >= 7) AnsiConsole.MarkupLine($"[grey]Class:[/] [#FFA500]{@class}[/]");
                if (step >= 8) AnsiConsole.MarkupLine($"[grey]Other info:[/] [#FFA500]{otherInfo}[/]");

                string input;

                switch (step)
                {
                    case 0:
                        Console.Write("Name: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 1:
                        Console.Write("Race: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 2:
                        Console.Write("Description: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 3:
                        Console.Write("Gender: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 4:
                        // Age — allow empty for 0, validate integer
                        Console.Write("Age (leave empty for 0): ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        if (input != "E" && input != "B")
                        {
                            if (string.IsNullOrWhiteSpace(input)) { age = 0; step++; continue; }
                            if (!int.TryParse(input.Trim(), out age))
                            {
                                Console.WriteLine("Invalid number — please enter an integer or leave empty.");
                     
                                continue;
                            }
                        }
                        break;
                    case 5:
                        Console.Write("Level (leave empty for 0): ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        if (input != "E" && input != "B")
                        {
                            if (string.IsNullOrWhiteSpace(input)) { level = 0; step++; continue; }
                            if (!int.TryParse(input.Trim(), out level))
                            {
                                Console.WriteLine("Invalid number — please enter an integer or leave empty.");
                         ;
                                continue;
                            }
                        }
                        break;
                    case 6:
                        Console.Write("Class: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 7:
                        Console.Write("Other info: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 8:
                        // Confirm
                        var confirm = ChracterMenu1("Confirm character creation", "Ja", "Nej", "Avsluta");
                        if (confirm == "Avsluta") Environment.Exit(0);
                        if (confirm == "Nej") { step = 0; continue; }
                        if (confirm == "Ja")
                        {
                            // assign to properties and finish
                            Names = name;
                            Race = race;
                            Description = description;
                            Gender = gender;
                            Age = age;
                            Level = level;
                            Class = @class;
                            OtherInfo = otherInfo;

                            Console.WriteLine();
                            Console.WriteLine($"Character '{Names}' created.");
                            Console.WriteLine("Press any key to continue...");
                            MenuHelper.DelayAndClear();



                            return;
                        }
                        // if somehow other result, loop
                        
                        continue;
                    default:
                        return;
                }

                // Handle Back/Exit inputs for non-age/level steps
                if (input == "E")
                {
                    Console.Clear();
                    ChracterMenu2();
                }
                
                if (input == "B")
                {
                    if (step > 0) step--;
                    continue;
                }

                // Store input to the right variable for steps that use string input
                switch (step)
                {
                    case 0: name = input?.Trim() ?? ""; break;
                    case 1: race = input?.Trim() ?? ""; break;
                    case 2: description = input?.Trim() ?? ""; break;
                    case 3: gender = input?.Trim() ?? ""; break;
                    case 6: @class = input?.Trim() ?? ""; break;
                    case 7: otherInfo = input?.Trim() ?? ""; break;
                }

                step++;
            }
        }


        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }

        public void EditCharacter(Project project, UserService userService)
        {
            var original = SelectCharacter(project, "Choose character to edit");
            if (original == null) return;

            var temp = new Character
            {
                Name = original.Name,
                Race = original.Race,
                Description = original.Description,
                Gender = original.Gender,
                Age = original.Age,
                Level = original.Level,
                Class = original.Class,
                OtherInfo = original.OtherInfo
            };

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit character: [#FFA500]{temp.Name}[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to change?")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices(
                            "Name",
                            "Race",
                            "Description",
                            "Gender",
                            "Age",
                            "Level",
                            "Class",
                            "Other info",
                            "Done")
                        .HighlightStyle(Color.Orange1));

                string PromptNonEmpty(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (!string.IsNullOrWhiteSpace(value))
                            return value;

                        AnsiConsole.MarkupLine("[red]Value cannot be empty.[/]");
                    }
                }

                int PromptInt(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (int.TryParse(value, out int number))
                            return number;

                        AnsiConsole.MarkupLine("[red]You must enter a number.[/]");
                    }
                }

                if (choice == "Done")
                {
                    Console.Clear();
                    ConsoleHelpers.Info("Character summary:");
                    AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{temp.Name}[/]");
                    AnsiConsole.MarkupLine($"[grey]Race:[/] {temp.Race}");
                    AnsiConsole.MarkupLine($"[grey]Description:[/] {temp.Description}");
                    AnsiConsole.MarkupLine($"[grey]Gender:[/] {temp.Gender}");
                    AnsiConsole.MarkupLine($"[grey]Age:[/] {temp.Age}");
                    AnsiConsole.MarkupLine($"[grey]Level:[/] {temp.Level}");
                    AnsiConsole.MarkupLine($"[grey]Class:[/] {temp.Class}");
                    AnsiConsole.MarkupLine($"[grey]Other info:[/] {temp.OtherInfo}");

                    Console.WriteLine();
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with these changes?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    if (confirm == "Exit")
                    {
                        ConsoleHelpers.DelayAndClear();
                        return;
                    }

                    if (confirm == "No (Start over)")
                    {
                        temp.Name = original.Name;
                        temp.Race = original.Race;
                        temp.Description = original.Description;
                        temp.Gender = original.Gender;
                        temp.Age = original.Age;
                        temp.Level = original.Level;
                        temp.Class = original.Class;
                        temp.OtherInfo = original.OtherInfo;
                        continue;
                    }

                    if (confirm == "Yes")
                    {
                        original.Name = temp.Name;
                        original.Race = temp.Race;
                        original.Description = temp.Description;
                        original.Gender = temp.Gender;
                        original.Age = temp.Age;
                        original.Level = temp.Level;
                        original.Class = temp.Class;
                        original.OtherInfo = temp.OtherInfo;

                        userService.SaveUserService();
                        ConsoleHelpers.Info("Character updated!");
                        ConsoleHelpers.DelayAndClear();
                        return;
                    }
                }

                switch (choice)
                {
                    case "Name":
                        temp.Name = PromptNonEmpty("[#FFA500]New name:[/]");
                        break;
                    case "Race":
                        temp.Race = PromptNonEmpty("[#FFA500]New race:[/]");
                        break;
                    case "Description":
                        temp.Description = PromptNonEmpty("[#FFA500]New description:[/]");
                        break;
                    case "Gender":
                        temp.Gender = PromptNonEmpty("[#FFA500]New gender:[/]");
                        break;
                    case "Age":
                        temp.Age = PromptInt("[#FFA500]New age:[/]");
                        break;
                    case "Level":
                        temp.Level = PromptInt("[#FFA500]New level:[/]");
                        break;
                    case "Class":
                        temp.Class = PromptNonEmpty("[#FFA500]New class:[/]");
                        break;
                    case "Other info":
                        temp.OtherInfo = PromptNonEmpty("[#FFA500]New other info:[/]");
                        break;
                }
            }
        }
        private Character? SelectCharacter(Project project, string title)
        {
            if (project.Characters == null || project.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No characters yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            return AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(project.Characters)
                    .UseConverter(c => $"{c.Name} ({c.Race})"));
        }
        public void DeleteCharacter(Project project, UserService userService)
        {
            Console.Clear();

            // Ask for confirmation
            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{this.Name}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                // Remove the character from the project's character list
                if (project.Characters.Contains(this))
                {
                    project.Characters.Remove(this);

                    // Save changes
                    userService.SaveUserService();

                    AnsiConsole.MarkupLine($"The character '[orange1]{this.Name}[/]' has been deleted!");
                    Thread.Sleep(1200);
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]Error: Character not found.[/]");
                    Thread.Sleep(1200);
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
                Thread.Sleep(1200);
            }
        }
    }
};
