using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Text;

namespace _404_not_founders.Models
{
    /// Represents a character in the D&D game project.
    /// Contains all character properties and methods for CRUD operations and AI generation.
    public class Character
    {
        // Character properties
        public string Name { get; set; }
        public string Race { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public string OtherInfo { get; set; }

        /// Interactive step-by-step process to create a new character.
        /// Allows user to go back (B) or exit (E) at any step.
        public void Add(User user, Project project, UserService userService)
        {
            string name = "", race = "", description = "", gender = "", characterClass = "", otherInfo = "";
            int age = 0, level = 0;
            int step = 0;
            bool addingCharacter = true;

            while (addingCharacter)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[underline #FFA500]Create New Character[/]");
                AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");
                Console.WriteLine();

                // Display already filled fields
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{name}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Race:[/] [#FFA500]{race}[/]");
                if (step >= 3) AnsiConsole.MarkupLine($"[grey]Description:[/] [#FFA500]{description}[/]");
                if (step >= 4) AnsiConsole.MarkupLine($"[grey]Gender:[/] [#FFA500]{gender}[/]");
                if (step >= 5) AnsiConsole.MarkupLine($"[grey]Age:[/] [#FFA500]{age}[/]");
                if (step >= 6) AnsiConsole.MarkupLine($"[grey]Level:[/] [#FFA500]{level}[/]");
                if (step >= 7) AnsiConsole.MarkupLine($"[grey]Class:[/] [#FFA500]{characterClass}[/]");
                if (step >= 8) AnsiConsole.MarkupLine($"[grey]Other info:[/] [#FFA500]{otherInfo}[/]");

                string input;

                // Handle each step of character creation
                switch (step)
                {
                    case 0: // Name
                        Console.Write("Name: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 1: // Race
                        Console.Write("Race: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 2: // Description
                        Console.Write("Description: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 3: // Gender
                        Console.Write("Gender: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 4: // Age (optional, defaults to 0)
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
                    case 5: // Level (optional, defaults to 0)
                        Console.Write("Level (leave empty for 0): ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        if (input != "E" && input != "B")
                        {
                            if (string.IsNullOrWhiteSpace(input)) { level = 0; step++; continue; }
                            if (!int.TryParse(input.Trim(), out level))
                            {
                                Console.WriteLine("Invalid number — please enter an integer or leave empty.");
                                continue;
                            }
                        }
                        break;
                    case 6: // Class
                        Console.Write("Class: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 7: // Other info
                        Console.Write("Other info: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 8: // Confirmation step
                        var confirm = MenuChoises.Menu("Confirm character creation", "Yes", "No");
                        if (confirm == "No") { step = 0; continue; }
                        if (confirm == "Yes")
                        {
                            if (project == null)
                            {
                                Console.WriteLine("No project found. Create or select a project before adding characters.");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey(true);
                                return;
                            }

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
                                project.AddCharacter(newCharacter, userService);
                            }
                            catch (InvalidOperationException ex)
                            {
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
                        continue;
                    default:
                        return;
                }

                // Handle exit command
                if (input == "E")
                {
                    addingCharacter = false;
                    Console.Clear();
                    return;
                }

                // Handle back command
                if (input == "B")
                {
                    if (step > 0) step--;
                    continue;
                }

                // Store input
                switch (step)
                {
                    case 0: name = input; break;
                    case 1: race = input; break;
                    case 2: description = input; break;
                    case 3: gender = input; break;
                    case 6: characterClass = input; break;
                    case 7: otherInfo = input; break;
                }

                step++;
            }
        }

        /// Displays the character information in a formatted card.
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }

        /// Interactive menu to edit all character properties.
        /// Shows live summary of changes before confirming.
        public void EditCharacter(Project project, UserService userService)
        {
            var temp = new Character
            {
                Name = this.Name,
                Race = this.Race,
                Description = this.Description,
                Gender = this.Gender,
                Age = this.Age,
                Level = this.Level,
                Class = this.Class,
                OtherInfo = this.OtherInfo
            };

            void ShowSummary(Character c)
            {
                var sb = new StringBuilder();
                sb.AppendLine("[underline #FFA500]Character summary:[/]");
                sb.AppendLine($"[grey]Name:[/]        [#FFA500]{(string.IsNullOrWhiteSpace(c.Name) ? "(unnamed)" : Markup.Escape(c.Name))}[/]");
                sb.AppendLine($"[grey]Race:[/]        [#FFA500]{(string.IsNullOrWhiteSpace(c.Race) ? "-" : Markup.Escape(c.Race))}[/]");
                sb.AppendLine($"[grey]Description:[/] [#FFA500]{(string.IsNullOrWhiteSpace(c.Description) ? "-" : Markup.Escape(c.Description))}[/]");
                sb.AppendLine($"[grey]Gender:[/]      [#FFA500]{(string.IsNullOrWhiteSpace(c.Gender) ? "-" : Markup.Escape(c.Gender))}[/]");
                sb.AppendLine($"[grey]Age:[/]         [#FFA500]{c.Age}[/]");
                sb.AppendLine($"[grey]Level:[/]       [#FFA500]{c.Level}[/]");
                sb.AppendLine($"[grey]Class:[/]       [#FFA500]{(string.IsNullOrWhiteSpace(c.Class) ? "-" : Markup.Escape(c.Class))}[/]");
                sb.AppendLine($"[grey]Other info:[/]  [#FFA500]{(string.IsNullOrWhiteSpace(c.OtherInfo) ? "-" : Markup.Escape(c.OtherInfo))}[/]");

                var panel = new Panel(new Markup(sb.ToString()))
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 0, 1, 0),
                };

                AnsiConsole.Write(panel);
                Console.WriteLine();
            }

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
                    if (int.TryParse(value, out var number))
                        return number;
                    AnsiConsole.MarkupLine("[red]Please enter a valid number.[/]");
                }
            }

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit character: [#FFA500]{Markup.Escape(temp.Name)}[/]");

                ShowSummary(temp);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to change?")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices("Name", "Race", "Description", "Gender", "Age", "Level", "Class", "Other Info", "Done"));

                if (choice == "Done")
                {
                    Console.Clear();
                    ShowSummary(temp);

                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with this character?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    if (confirm == "Exit")
                    {
                        Thread.Sleep(800);
                        Console.Clear();
                        return;
                    }

                    if (confirm == "No (Start over)")
                    {
                        temp.Name = this.Name;
                        temp.Race = this.Race;
                        temp.Description = this.Description;
                        temp.Gender = this.Gender;
                        temp.Age = this.Age;
                        temp.Level = this.Level;
                        temp.Class = this.Class;
                        temp.OtherInfo = this.OtherInfo;
                        continue;
                    }

                    if (confirm == "Yes")
                    {
                        this.Name = temp.Name;
                        this.Race = temp.Race;
                        this.Description = temp.Description;
                        this.Gender = temp.Gender;
                        this.Age = temp.Age;
                        this.Level = temp.Level;
                        this.Class = temp.Class;
                        this.OtherInfo = temp.OtherInfo;

                        userService.SaveUserService();
                        AnsiConsole.MarkupLine("[green]Character updated![/]");
                        Thread.Sleep(1000);
                        Console.Clear();
                        return;
                    }
                }

                switch (choice)
                {
                    case "Name": temp.Name = PromptNonEmpty("[#FFA500]New name:[/]"); break;
                    case "Race": temp.Race = PromptNonEmpty("[#FFA500]New race:[/]"); break;
                    case "Description": temp.Description = PromptNonEmpty("[#FFA500]New description:[/]"); break;
                    case "Gender": temp.Gender = PromptNonEmpty("[#FFA500]New gender:[/]"); break;
                    case "Age": temp.Age = PromptInt("[#FFA500]New age:[/]"); break;
                    case "Level": temp.Level = PromptInt("[#FFA500]New level:[/]"); break;
                    case "Class": temp.Class = PromptNonEmpty("[#FFA500]New class:[/]"); break;
                    case "Other Info": temp.OtherInfo = PromptNonEmpty("[#FFA500]New other info:[/]"); break;
                }
            }
        }

        /// Deletes this character from the given project after user confirmation.
        public void DeleteCharacter(Project project, UserService userService)
        {
            Console.Clear();

            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{Markup.Escape(this.Name)}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                if (project.Characters.Contains(this))
                {
                    project.Characters.Remove(this);
                    userService.SaveUserService();
                    AnsiConsole.MarkupLine($"Character '[orange1]{Markup.Escape(this.Name)}[/]' has been deleted!");
                    Thread.Sleep(1200);
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

        /// Generates a character using Google Gemini AI with Retry/Keep/Cancel workflow.
        /// Uses project/world/storyline context. Ensures Level is at least 1.
        public async Task<Character?> GenerateCharacterWithGeminiAI(Project currentProject, UserService userService)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string googleApiKey = config["GoogleAI:ApiKey"];
            var aiService = new GeminiAIService(googleApiKey);

            string worldContext = currentProject.Worlds != null && currentProject.Worlds.Any()
                ? $"World: {currentProject.Worlds.First().Name}, Regions: {currentProject.Worlds.First().Regions}, Factions: {currentProject.Worlds.First().Factions}"
                : "No world created yet.";

            string storylineContext = currentProject.Storylines != null && currentProject.Storylines.Any()
                ? $"Storyline: {currentProject.Storylines.First().Title}, Theme: {currentProject.Storylines.First().Theme}"
                : "No storyline created yet.";

            while (true)
            {
                string userContext = AiHelper.AskOptionalUserContext("Generate Character with AI");

                if (userContext == "E")
                    return null;

                string prompt = string.IsNullOrWhiteSpace(userContext)
     ? $@"You are a fantasy character creator for a Dungeons & Dragons campaign.

PROJECT CONTEXT:
{currentProject.description}

WORLD CONTEXT:
{worldContext}

STORYLINE CONTEXT:
{storylineContext}

TASK:
Generate a unique character that fits this campaign.

RULES:
- All fields MUST be filled. No empty values allowed.
- Use a unique, memorable name. Avoid common fantasy names like 'Aragorn' or 'Gandalf'.
- Age MUST be between 20 and 30 years old (inclusive).
- Level MUST ALWAYS be exactly 1 (do not use any other level).
- Return ONLY the formatted text below. No explanations, no markdown, no asterisks.
- BEFORE returning, self-check: Are ALL fields filled and do Age/Level follow the rules?

FORMAT:
Name: [unique character name]
Race: [fantasy race]
Description: [physical description and appearance]
Gender: [character gender]
Age: [character age between 20 and 30]
Level: [always 1]
Class: [character class]
OtherInfo: [additional details about background, personality, equipment, etc.]"
     : $@"You are a fantasy character creator for a Dungeons & Dragons campaign.

PROJECT CONTEXT:
{currentProject.description}

WORLD CONTEXT:
{worldContext}

STORYLINE CONTEXT:
{storylineContext}

USER REQUEST:
{userContext}

TASK:
Generate a unique character based on the user's request and campaign context.

RULES:
- All fields MUST be filled. No empty values allowed.
- Use a unique, memorable name.
- Age MUST be between 20 and 30 years old (inclusive).
- Level MUST ALWAYS be exactly 1 (do not use any other level).
- Return ONLY the formatted text below. No explanations, no markdown, no asterisks.
- BEFORE returning, self-check: Are ALL fields filled and do Age/Level follow the rules?

FORMAT:
Name: [unique character name]
Race: [fantasy race]
Description: [physical description and appearance]
Gender: [character gender]
Age: [character age between 20 and 30]
Level: [always 1]
Class: [character class]
OtherInfo: [additional details about background, personality, equipment, etc.]";

                AiHelper.ShowGeneratingText("Character");

                string result = await aiService.GenerateAsync(prompt);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var newCharacter = ParseAITextToCharacter(result);

                    if (newCharacter != null)
                    {
                        Console.Clear();
                        ConsoleHelpers.Info("Generated Character:");
                        Console.WriteLine();

                        // Use the same formatted card as in normal Show()
                        newCharacter.Show();

                        Console.WriteLine();

                        var choice = AiHelper.RetryMenu();

                        if (choice == "Keep")
                        {
                            currentProject.Characters.Add(newCharacter);
                            userService.SaveUserService();
                            AiHelper.ShowSaved("Character", newCharacter.Name);
                            AnsiClearHelper.WaitForKeyAndClear();
                            return newCharacter;
                        }
                        else if (choice == "Cancel")
                        {
                            AiHelper.ShowCancelled();
                            return null;
                        }
                    }
                    else
                    {
                        AiHelper.ShowError("Failed to parse AI response. Retrying...");
                    }
                }
                else
                {
                    AiHelper.ShowError("AI returned empty response. Retrying...");
                }
            }
        }

        /// Parses AI-generated plain text into a Character.
        /// Forces Level to 1 and clamps Age to a reasonable range (20–30).
        /// Returns null if required fields (Name, Race) are missing.
        public static Character? ParseAITextToCharacter(string input)
        {
            var character = new Character();
            var lines = input.Replace("\r\n", "\n").Split('\n');

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();

                if (cleanLine.StartsWith("Name:", StringComparison.OrdinalIgnoreCase))
                    character.Name = cleanLine.Substring(5).Trim();

                else if (cleanLine.StartsWith("Race:", StringComparison.OrdinalIgnoreCase))
                    character.Race = cleanLine.Substring(5).Trim();

                else if (cleanLine.StartsWith("Description:", StringComparison.OrdinalIgnoreCase))
                    character.Description = cleanLine.Substring(12).Trim();

                else if (cleanLine.StartsWith("Gender:", StringComparison.OrdinalIgnoreCase))
                    character.Gender = cleanLine.Substring(7).Trim();

                else if (cleanLine.StartsWith("Age:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(4).Trim(), out int age))
                        character.Age = age;
                }

                else if (cleanLine.StartsWith("Level:", StringComparison.OrdinalIgnoreCase))
                {
                    // Ignore whatever the model sent – always use Level 1
                    character.Level = 1;
                }

                else if (cleanLine.StartsWith("Class:", StringComparison.OrdinalIgnoreCase))
                    character.Class = cleanLine.Substring(6).Trim();

                else if (cleanLine.StartsWith("OtherInfo:", StringComparison.OrdinalIgnoreCase))
                    character.OtherInfo = cleanLine.Substring(10).Trim();
            }

            // Ensure non-null strings
            character.Name ??= "";
            character.Race ??= "";
            character.Description ??= "";
            character.Gender ??= "";
            character.Class ??= "";
            character.OtherInfo ??= "";

            // Force Level to 1 if somehow still 0
            if (character.Level <= 0)
                character.Level = 1;

            // Clamp Age into 20–30 if it ended up outside range or unset (0)
            if (character.Age < 20 || character.Age > 30)
                character.Age = 25;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(character.Name) ||
                string.IsNullOrWhiteSpace(character.Race))
                return null;

            return character;
        }
    }
}
