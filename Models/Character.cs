using System;
using System.Text;
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


        public void Add(User currentUser, ProjectService projectService, UserService userService)
        {

            string name = "", race = "", description = "", gender = "", characterClass = "", otherInfo = "";
            int age = 0, level = 0;

            int step = 0;
            bool addingCharacter = true;

            // Loop for adding character using step
            while (addingCharacter)
            {
                Console.Clear();
                ConsoleHelpers.Info("Create New Character");
                ConsoleHelpers.InputInstruction(true);

                // Show current inputs based on step
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{name}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Race:[/] [#FFA500]{race}[/]");
                if (step >= 3) AnsiConsole.MarkupLine($"[grey]Description:[/] [#FFA500]{description}[/]");
                if (step >= 4) AnsiConsole.MarkupLine($"[grey]Gender:[/] [#FFA500]{gender}[/]");
                if (step >= 5) AnsiConsole.MarkupLine($"[grey]Age:[/] [#FFA500]{age}[/]");
                if (step >= 6) AnsiConsole.MarkupLine($"[grey]Level:[/] [#FFA500]{level}[/]");
                if (step >= 7) AnsiConsole.MarkupLine($"[grey]Class:[/] [#FFA500]{characterClass}[/]");
                if (step >= 8) AnsiConsole.MarkupLine($"[grey]Other info:[/] [#FFA500]{otherInfo}[/]");

                string input;

                // Handle each step input
                switch (step)
                {
                    case 0:
                        // Name: textinput
                        Console.Write("Name: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 1:
                        // Race: textinput
                        Console.Write("Race: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 2:
                        // Description: long textinput
                        Console.Write("Description: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 3:
                        // Gender: textinput
                        Console.Write("Gender: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 4:
                        // Age: number, empty = 0
                        Console.Write("Age (leave empty for 0): ");
                        input = ConsoleHelpers.ReadBackOrExit();

                        // if not exit or back, try parse
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
                        // Level: same logic as age
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
                        // Class: textinput
                        Console.Write("Class: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 7:
                        // Other info: textinput
                        Console.Write("Other info: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 8:
                        // Confirmation step
                        Project project = null;
                        var confirm = MenuChoises.Menu("Confirm character creation", "Yes", "No");
                        if (confirm == "No") { step = 0; continue; }
                        if (confirm == "Yes")
                        {
                            // Get current project to add character to
                            if (currentUser != null)
                            {
                                if (currentUser.Projects != null && currentUser.LastSelectedProjectId.HasValue)
                                {
                                    project = currentUser.Projects.FirstOrDefault(p => p.Id == currentUser.LastSelectedProjectId.Value);
                                }
                                if (project == null && currentUser.Projects != null)
                                {
                                    project = currentUser.Projects.FirstOrDefault();
                                }
                            }

                            // Handla null project
                            if (project == null)
                            {

                                Console.WriteLine("No project found. Create or select a project before adding characters.");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey(true);
                                return;
                            }

                            // Create new character object for adding
                            var newCharacter = new Character
                            {
                                Name = name,
                                Race = race,
                                Description = description,
                                Gender = gender,
                                Age = age,
                                Level = level,
                                Class = characterClass,
                                OtherInfo = otherInfo,
                            };

                            try
                            {
                                // Try to add character to project
                                project.AddCharacter(newCharacter, userService);
                            }
                            catch (InvalidOperationException ex)
                            {
                                // If error occurs, shows message and restart
                                Console.WriteLine(ex.Message);
                                Console.WriteLine("Press any key to try again...");
                                Console.ReadKey(true);
                                step = 0;
                                continue;
                            }

                            Console.WriteLine();
                            Console.WriteLine($"Character '{name}' created.");

                            ConsoleHelpers.DelayAndClear();

                            return;
                        }
                        // Should not reach here, but just in case
                        continue;
                    default:
                        return;
                }

                // Handle special commands "E" and "B"
                if (input == "E")
                {
                    
                    addingCharacter = false;
                    Console.Clear();
                    return;
                }

                if (input == "B")
                {

                    if (step > 0) step--;
                    continue;
                }

                // Store input based on step
                switch (step)
                {
                    case 0: name = input?.Trim() ?? ""; break;
                    case 1: race = input?.Trim() ?? ""; break;
                    case 2: description = input?.Trim() ?? ""; break;
                    case 3: gender = input?.Trim() ?? ""; break;
                    case 6: characterClass = input?.Trim() ?? ""; break;
                    case 7: otherInfo = input?.Trim() ?? ""; break;
                }

                // Advance to next step
                step++;
            }
        }



        public void ShowCharacters(Project project)
        {
            // Check that project is not null
            if (project == null)
            {
                AnsiConsole.MarkupLine("[red]No project provided.[/]");
                ConsoleHelpers.DelayAndClear();
                return;
            }

            // Ensure there are characters to show
            if (project.Characters == null || project.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No characters in this project.[/]");
                ConsoleHelpers.DelayAndClear();
                return;
            }

            // Give user a selection of characters to choose from
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title($"[#FFA500]Select character to show[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .PageSize(10)
                    .AddChoices(project.Characters)
                    .UseConverter(c => string.IsNullOrWhiteSpace(c.Name) ? "(unnamed)" : c.Name)
            );

            Console.Clear();
            ShowInfoCard.ShowObject(selected);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            ConsoleHelpers.DelayAndClear();
        }
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }

        public void EditCharacter(Project project, UserService userService)
        {
            var original = SelectCharacter(project, "Choose character to edit");
            if (original == null) return;

            // Create a temporary copy to edit
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

            void ShowSummary(Character c)
            {
                // Build a multiline markup string for the panel so the summary is visible above the prompt
                var sb = new StringBuilder();
                sb.AppendLine("[underline #FFA500]Character summary:[/]");
                sb.AppendLine($"[grey]Name:[/]       [#FFA500]{(string.IsNullOrWhiteSpace(c.Name) ? "(unnamed)" : c.Name)}[/]");
                sb.AppendLine($"[grey]Race:[/]       [#FFA500]{(string.IsNullOrWhiteSpace(c.Race) ? "-" : c.Race)}[/]");
                sb.AppendLine($"[grey]Description:[/] [#FFA500]{(string.IsNullOrWhiteSpace(c.Description) ? "-" : c.Description)}[/]");
                sb.AppendLine($"[grey]Gender:[/]     [#FFA500]{(string.IsNullOrWhiteSpace(c.Gender) ? "-" : c.Gender)}[/]");
                sb.AppendLine($"[grey]Age:[/]        [#FFA500]{c.Age}[/]");
                sb.AppendLine($"[grey]Level:[/]      [#FFA500]{c.Level}[/]");
                sb.AppendLine($"[grey]Class:[/]      [#FFA500]{(string.IsNullOrWhiteSpace(c.Class) ? "-" : c.Class)}[/]");
                sb.AppendLine($"[grey]Other info:[/] [#FFA500]{(string.IsNullOrWhiteSpace(c.OtherInfo) ? "-" : c.OtherInfo)}[/]");

                var panel = new Panel(new Markup(sb.ToString()))
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 0, 1, 0),
                };

                AnsiConsole.Write(panel);
                Console.WriteLine();
            }

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit character: [#FFA500]{temp.Name}[/]");

                ShowSummary(temp);

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

                // Helper methods for input validation
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

                // Prompt for integer with validation
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

                // If done, show summary and confirmation options
                if (choice == "Done")
                {
                    Console.Clear();
                    ConsoleHelpers.Info("Character summary:");
                    ShowSummary(temp);

                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with these changes?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    // If exit, return without saving
                    if (confirm == "Exit")
                    {
                        ConsoleHelpers.DelayAndClear();
                        return;
                    }

                    // If start over, reset temp to original and continue editing
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

                    // If yes, save changes to original and exit
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

                // Handle editing each field
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
            // Ensure there are characters to select from
            if (project.Characters == null || project.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No characters yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            // Prompt user to select a character
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