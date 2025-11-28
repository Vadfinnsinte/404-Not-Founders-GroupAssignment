using _404_not_founders.Menus;
using _404_not_founders.Services;
using _404_not_founders.UI;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
            string name = "", race = "", charClass = "", backstory = "", personality = "", equipment = "";
            int level = 1, str = 10, dex = 10, con = 10, intel = 10, wis = 10, cha = 10;
            int step = 0;

            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[underline #FFA500]Create New Character[/]");
                AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");
                Console.WriteLine();

                if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Name:[/] {name}");
                if (step >= 2) AnsiConsole.MarkupLine($"[#FFA500]Race:[/] {race}");
                if (step >= 3) AnsiConsole.MarkupLine($"[#FFA500]Class:[/] {charClass}");
                if (step >= 4) AnsiConsole.MarkupLine($"[#FFA500]Level:[/] {level}");
                if (step >= 5) AnsiConsole.MarkupLine($"[#FFA500]Strength:[/] {str}");
                if (step >= 6) AnsiConsole.MarkupLine($"[#FFA500]Dexterity:[/] {dex}");
                if (step >= 7) AnsiConsole.MarkupLine($"[#FFA500]Constitution:[/] {con}");
                if (step >= 8) AnsiConsole.MarkupLine($"[#FFA500]Intelligence:[/] {intel}");
                if (step >= 9) AnsiConsole.MarkupLine($"[#FFA500]Wisdom:[/] {wis}");
                if (step >= 10) AnsiConsole.MarkupLine($"[#FFA500]Charisma:[/] {cha}");
                if (step >= 11) AnsiConsole.MarkupLine($"[#FFA500]Backstory:[/] {backstory}");
                if (step >= 12) AnsiConsole.MarkupLine($"[#FFA500]Personality:[/] {personality}");

                string prompt = step switch
                {
                    0 => "Character Name:",
                    1 => "Race:",
                    2 => "Class:",
                    3 => "Level:",
                    4 => "Strength:",
                    5 => "Dexterity:",
                    6 => "Constitution:",
                    7 => "Intelligence:",
                    8 => "Wisdom:",
                    9 => "Charisma:",
                    10 => "Backstory:",
                    11 => "Personality:",
                    12 => "Equipment:",
                    _ => ""
                };

                if (step == 13)
                {
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with this character?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit")
                    );

                    if (confirm == "Exit") return;
                    if (confirm == "No (Start over)") { step = 0; continue; }

                    if (confirm == "Yes")
                    {
                        this.Name = name;
                        this.Race = race;
                        this.Class = charClass;
                        this.Level = level;
                        this.Strength = str;
                        this.Dexterity = dex;
                        this.Constitution = con;
                        this.Intelligence = intel;
                        this.Wisdom = wis;
                        this.Charisma = cha;
                        this.Backstory = backstory;
                        this.Personality = personality;
                        this.Equipment = equipment;
                        this.orderInProject = (project.Characters?.Count ?? 0) + 1;

                        project.Characters.Add(this);
                        userService.SaveUserService();

                        AnsiConsole.MarkupLine($"[orange1]Character '{this.Name}' has been saved![/]");
                        Thread.Sleep(1200);
                        return;
                    }
                }

                string input = AskStepInput.AskStepInputs(prompt);

                if (input == "E") return;
                if (input == "B")
                {
                    if (step > 0) step--;
                    continue;
                }

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

                step++;
            }
        }

        public void EditCharacter(UserService userService)
        {
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

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit character: [#FFA500]{temp.Name}[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to change?")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices("Name", "Race", "Class", "Level", "Stats", "Backstory", "Personality", "Equipment", "Done"));

                string PromptNonEmpty(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (!string.IsNullOrWhiteSpace(value)) return value;
                        AnsiConsole.MarkupLine("[red]Value cannot be empty.[/]");
                    }
                }

                int PromptInt(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (int.TryParse(value, out int result)) return result;
                        AnsiConsole.MarkupLine("[red]Please enter a valid number.[/]");
                    }
                }

                if (choice == "Done")
                {
                    Console.Clear();
                    ConsoleHelpers.Info("Character summary:");
                    AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{temp.Name}[/]");
                    AnsiConsole.MarkupLine($"[grey]Race:[/] {temp.Race}");
                    AnsiConsole.MarkupLine($"[grey]Class:[/] {temp.Class}");
                    AnsiConsole.MarkupLine($"[grey]Level:[/] {temp.Level}");
                    AnsiConsole.MarkupLine($"[grey]STR:[/] {temp.Strength} [grey]DEX:[/] {temp.Dexterity} [grey]CON:[/] {temp.Constitution}");
                    AnsiConsole.MarkupLine($"[grey]INT:[/] {temp.Intelligence} [grey]WIS:[/] {temp.Wisdom} [grey]CHA:[/] {temp.Charisma}");
                    AnsiConsole.MarkupLine($"[grey]Backstory:[/] {temp.Backstory}");
                    AnsiConsole.MarkupLine($"[grey]Personality:[/] {temp.Personality}");
                    AnsiConsole.MarkupLine($"[grey]Equipment:[/] {temp.Equipment}");

                    Console.WriteLine();
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with this character?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    if (confirm == "Exit")
                    {
                        Console.Clear();
                        return;
                    }

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

        public void Show()
        {
            ShowInfoCard.ShowObject(this);
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