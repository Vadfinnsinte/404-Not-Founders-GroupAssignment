
using _404_not_founders.Services;
using Spectre.Console;
using _404_not_founders.UI.Helpers;
using _404_not_founders.UI.Display;

using System.Text;


namespace _404_not_founders.Models
{
    public class Character
    {
        public string Name { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public int Level { get; set; } = 1;
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public string Backstory { get; set; }
        public string Personality { get; set; }
        public string Equipment { get; set; }
        public int orderInProject { get; set; }

        public void Add(User user, Project project, UserService userService)
        {

            string name = "", race = "", description = "", gender = "", characterClass = "", otherInfo = "";
            int age = 0, level = 0;

            int step = 0;
            bool addingCharacter = true;

            // Loop for adding character using step
            while (addingCharacter)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[underline #FFA500]Create New Character[/]");
                AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");
                Console.WriteLine();

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

                string input = AskStepInput.AskStepInputs(prompt);

                if (input == "E") return;
                if (input == "B")
                {

                    if (step > 0) step--;
                    continue;
                }

                // Store input based on step
                switch (step)
                {
                    case 0: name = input; break;
                    case 1: race = input; break;
                    case 2: charClass = input; break;
                    case 3: if (int.TryParse(input, out int lvl)) level = lvl; break;
                    case 4: if (int.TryParse(input, out int s)) str = s; break;
                    case 5: if (int.TryParse(input, out int d)) dex = d; break;
                    case 6: if (int.TryParse(input, out int c)) con = c; break;
                    case 7: if (int.TryParse(input, out int i)) intel = i; break;
                    case 8: if (int.TryParse(input, out int w)) wis = w; break;
                    case 9: if (int.TryParse(input, out int ch)) cha = ch; break;
                    case 10: backstory = input; break;
                    case 11: personality = input; break;
                    case 12: equipment = input; break;
                }

                // Advance to next step
                step++;
            }
        }

        public void EditCharacter(UserService userService)
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
                Name = this.Name,
                Race = this.Race,
                Class = this.Class,
                Level = this.Level,
                Strength = this.Strength,
                Dexterity = this.Dexterity,
                Constitution = this.Constitution,
                Intelligence = this.Intelligence,
                Wisdom = this.Wisdom,
                Charisma = this.Charisma,
                Backstory = this.Backstory,
                Personality = this.Personality,
                Equipment = this.Equipment
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
                        .AddChoices("Name", "Race", "Class", "Level", "Stats", "Backstory", "Personality", "Equipment", "Done"));

                // Helper methods for input validation
                string PromptNonEmpty(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (!string.IsNullOrWhiteSpace(value)) return value;
                        AnsiConsole.MarkupLine("[red]Value cannot be empty.[/]");
                    }
                }

                // Prompt for integer with validation
                int PromptInt(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (int.TryParse(value, out int result)) return result;
                        AnsiConsole.MarkupLine("[red]Please enter a valid number.[/]");
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
                            .Title("[#FFA500]Are you happy with this character?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    // If exit, return without saving
                    if (confirm == "Exit")
                    {
                        Console.Clear();
                        return;
                    }

                    // If start over, reset temp to original and continue editing
                    if (confirm == "No (Start over)")
                    {
                        temp.Name = this.Name;
                        temp.Race = this.Race;
                        temp.Class = this.Class;
                        temp.Level = this.Level;
                        temp.Strength = this.Strength;
                        temp.Dexterity = this.Dexterity;
                        temp.Constitution = this.Constitution;
                        temp.Intelligence = this.Intelligence;
                        temp.Wisdom = this.Wisdom;
                        temp.Charisma = this.Charisma;
                        temp.Backstory = this.Backstory;
                        temp.Personality = this.Personality;
                        temp.Equipment = this.Equipment;
                        continue;
                    }

                    // If yes, save changes to original and exit
                    if (confirm == "Yes")
                    {
                        this.Name = temp.Name;
                        this.Race = temp.Race;
                        this.Class = temp.Class;
                        this.Level = temp.Level;
                        this.Strength = temp.Strength;
                        this.Dexterity = temp.Dexterity;
                        this.Constitution = temp.Constitution;
                        this.Intelligence = temp.Intelligence;
                        this.Wisdom = temp.Wisdom;
                        this.Charisma = temp.Charisma;
                        this.Backstory = temp.Backstory;
                        this.Personality = temp.Personality;
                        this.Equipment = temp.Equipment;

                        userService.SaveUserService();
                        AnsiConsole.MarkupLine("[green]Character updated![/]");
                        Thread.Sleep(1000);
                        Console.Clear();
                        return;
                    }
                }

                // Handle editing each field
                switch (choice)
                {
                    case "Name": temp.Name = PromptNonEmpty("[#FFA500]New name:[/]"); break;
                    case "Race": temp.Race = PromptNonEmpty("[#FFA500]New race:[/]"); break;
                    case "Class": temp.Class = PromptNonEmpty("[#FFA500]New class:[/]"); break;
                    case "Level": temp.Level = PromptInt("[#FFA500]New level:[/]"); break;
                    case "Stats":
                        temp.Strength = PromptInt("[#FFA500]New Strength:[/]");
                        temp.Dexterity = PromptInt("[#FFA500]New Dexterity:[/]");
                        temp.Constitution = PromptInt("[#FFA500]New Constitution:[/]");
                        temp.Intelligence = PromptInt("[#FFA500]New Intelligence:[/]");
                        temp.Wisdom = PromptInt("[#FFA500]New Wisdom:[/]");
                        temp.Charisma = PromptInt("[#FFA500]New Charisma:[/]");
                        break;
                    case "Backstory": temp.Backstory = PromptNonEmpty("[#FFA500]New backstory:[/]"); break;
                    case "Personality": temp.Personality = PromptNonEmpty("[#FFA500]New personality:[/]"); break;
                    case "Equipment": temp.Equipment = PromptNonEmpty("[#FFA500]New equipment:[/]"); break;
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

            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{this.Name}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                if (project.Characters.Contains(this))
                {
                    project.Characters.Remove(this);
                    userService.SaveUserService();
                    AnsiConsole.MarkupLine($"Character '[orange1]{this.Name}[/]' has been deleted!");
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
Generate a unique Level 1 starting character that fits this campaign.

RULES:
- Character MUST be Level 1 with appropriate starting stats.
- Stats should be balanced for a new adventurer (8-16 range, no stat above 16 before racial bonuses).
- All fields MUST be filled. No empty values allowed.
- Use a unique, memorable name. Avoid common fantasy names like 'Aragorn' or 'Gandalf'.
- Return ONLY the formatted text below. No explanations, no markdown, no asterisks.
- BEFORE returning, self-check: Are ALL fields filled and stats appropriate for Level 1?

FORMAT:
Name: [unique character name]
Race: [fantasy race]
Class: [character class]
Level: 1
Strength: [8-16]
Dexterity: [8-16]
Constitution: [8-16]
Intelligence: [8-16]
Wisdom: [8-16]
Charisma: [8-16]
Backstory: [character backstory]
Personality: [personality traits]
Equipment: [starting equipment]"
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
Generate a unique Level 1 starting character based on the user's request and campaign context.

RULES:
- Character MUST be Level 1 with appropriate starting stats.
- Stats should be balanced for a new adventurer (8-16 range, no stat above 16 before racial bonuses).
- All fields MUST be filled. No empty values allowed.
- Use a unique, memorable name.
- Return ONLY the formatted text below. No explanations, no markdown, no asterisks.
- BEFORE returning, self-check: Are ALL fields filled and stats appropriate for Level 1?

FORMAT:
Name: [unique character name]
Race: [fantasy race]
Class: [character class]
Level: 1
Strength: [8-16]
Dexterity: [8-16]
Constitution: [8-16]
Intelligence: [8-16]
Wisdom: [8-16]
Charisma: [8-16]
Backstory: [character backstory]
Personality: [personality traits]
Equipment: [starting equipment]";

                AiHelper.ShowGeneratingText("Character");

                string result = await aiService.GenerateAsync(prompt);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    int nextOrder = (currentProject.Characters?.Count ?? 0) + 1;
                    var newCharacter = ParseAITextToCharacter(result, nextOrder);

                    if (newCharacter != null)
                    {
                        Console.Clear();
                        ConsoleHelpers.Info("Generated Character:");
                        Console.WriteLine();
                        AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{Markup.Escape(newCharacter.Name)}[/]");
                        AnsiConsole.MarkupLine($"[grey]Race:[/] {Markup.Escape(newCharacter.Race)}");
                        AnsiConsole.MarkupLine($"[grey]Class:[/] {Markup.Escape(newCharacter.Class)}");
                        AnsiConsole.MarkupLine($"[grey]Level:[/] {newCharacter.Level}");
                        AnsiConsole.MarkupLine($"[grey]Stats:[/] STR:{newCharacter.Strength} DEX:{newCharacter.Dexterity} CON:{newCharacter.Constitution} INT:{newCharacter.Intelligence} WIS:{newCharacter.Wisdom} CHA:{newCharacter.Charisma}");
                        AnsiConsole.MarkupLine($"[grey]Backstory:[/] {Markup.Escape(newCharacter.Backstory)}");
                        AnsiConsole.MarkupLine($"[grey]Personality:[/] {Markup.Escape(newCharacter.Personality)}");
                        AnsiConsole.MarkupLine($"[grey]Equipment:[/] {Markup.Escape(newCharacter.Equipment)}");
                        Console.WriteLine();

                        var choice = AiHelper.RetryMenu();

                        if (choice == "Keep")
                        {
                            currentProject.Characters.Add(newCharacter);
                            userService.SaveUserService();
                            AiHelper.ShowSaved("Character", newCharacter.Name);
                            // Vänta och cleara innan vi går tillbaka till menyn
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

        public static Character? ParseAITextToCharacter(string input, int nextOrderInProject)
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
                else if (cleanLine.StartsWith("Class:", StringComparison.OrdinalIgnoreCase))
                    character.Class = cleanLine.Substring(6).Trim();
                else if (cleanLine.StartsWith("Level:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(6).Trim(), out int lvl))
                        character.Level = lvl;
                    else
                        character.Level = 1;
                }
                else if (cleanLine.StartsWith("Strength:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(9).Trim(), out int str))
                        character.Strength = str;
                }
                else if (cleanLine.StartsWith("Dexterity:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(10).Trim(), out int dex))
                        character.Dexterity = dex;
                }
                else if (cleanLine.StartsWith("Constitution:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(13).Trim(), out int con))
                        character.Constitution = con;
                }
                else if (cleanLine.StartsWith("Intelligence:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(13).Trim(), out int intel))
                        character.Intelligence = intel;
                }
                else if (cleanLine.StartsWith("Wisdom:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(7).Trim(), out int wis))
                        character.Wisdom = wis;
                }
                else if (cleanLine.StartsWith("Charisma:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cleanLine.Substring(9).Trim(), out int cha))
                        character.Charisma = cha;
                }
                else if (cleanLine.StartsWith("Backstory:", StringComparison.OrdinalIgnoreCase))
                    character.Backstory = cleanLine.Substring(10).Trim();
                else if (cleanLine.StartsWith("Personality:", StringComparison.OrdinalIgnoreCase))
                    character.Personality = cleanLine.Substring(12).Trim();
                else if (cleanLine.StartsWith("Equipment:", StringComparison.OrdinalIgnoreCase))
                    character.Equipment = cleanLine.Substring(10).Trim();
            }

            character.Name ??= "";
            character.Race ??= "";
            character.Class ??= "";
            character.Backstory ??= "";
            character.Personality ??= "";
            character.Equipment ??= "";
            if (character.Level == 0) character.Level = 1;

            character.orderInProject = nextOrderInProject;

            if (string.IsNullOrWhiteSpace(character.Name) || string.IsNullOrWhiteSpace(character.Race))
                return null;

            return character;
        }
    }
}