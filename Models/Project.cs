using System;                               // Console, Thread
using System.Collections.Generic;           // List<T>
using System.Linq;                          // LINQ
using _404_not_founders.Menus;              // MenuChoises
using _404_not_founders.Services;           // UserService, DungeonMasterAI
using _404_not_founders.UI;                 // BigHeader, AnsiClearHelper, AskStepInput, AiHelper, ShowInfoCard
using Spectre.Console;                      // AnsiConsole, Markup, Table, Panel, Color, BoxBorder, SelectionPrompt
using Microsoft.Extensions.Configuration;   // ConfigurationBuilder;
using System.Threading.Tasks;
using System.Threading;

namespace _404_not_founders.Models
{
    public class Project
    {
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; } = DateTime.Now;
        public Guid Id { get; set; } = Guid.NewGuid();

        // AI-genererade fält
        public string highConcept { get; set; } = string.Empty;
        public string campaignGoal { get; set; } = string.Empty;
        public string themes { get; set; } = string.Empty;
        public int orderInProject { get; set; }

        public List<World> Worlds { get; set; } = new();
        public List<Storyline> Storylines { get; set; } = new();
        public List<Character> Characters { get; set; } = new();

        public async Task<Project> Add(User currentUser, UserService userService)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[underline #FFA500]Create New Project[/]");
                var mode = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("How do you want to create your project?")
                        .HighlightStyle(Color.Orange1)
                        .AddChoices(
                            "Generate with AI",
                            "Manually",
                            "Exit"
                        ));

                if (mode == "Exit")
                {
                    Console.Clear();
                    return null;
                }

                if (mode == "Manually")
                {
                    string title = "";
                    string description = "";
                    int step = 0;

                    while (true)
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[underline #FFA500]Create New Project[/]");
                        AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");

                        if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Title:[/] {Markup.Escape(title)}");
                        if (step >= 2) AnsiConsole.MarkupLine($"[#FFA500]Description:[/] {Markup.Escape(description)}");

                        string prompt = step switch
                        {
                            0 => "Enter Project title:",
                            1 => "Enter Project description:",
                            _ => ""
                        };

                        if (step == 2)
                        {
                            var confirm = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[#FFA500]Save this Project?[/]")
                                    .HighlightStyle(Color.Orange1)
                                    .AddChoices("Yes", "No (Start over)", "Exit"));

                            if (confirm == "Exit") return null;
                            if (confirm == "No (Start over)") { step = 0; continue; }

                            if (confirm == "Yes")
                            {
                                var newProject = new Project
                                {
                                    title = title,
                                    description = description,
                                    DateOfCreation = DateTime.Now,
                                    Storylines = new List<Storyline>(),
                                    Characters = new List<Character>(),
                                    Worlds = new List<World>(),
                                    orderInProject = (currentUser.Projects?.Count ?? 0) + 1
                                };
                                currentUser.Projects.Add(newProject);
                                userService.SaveUserService();
                                AnsiConsole.MarkupLine("[green]Project created![/]");
                                Thread.Sleep(1200);
                                return newProject;
                            }
                        }

                        string input = AskStepInput.AskStepInputs(prompt);

                        if (input == "B")
                        {
                            if (step > 0) step--;
                            continue;
                        }
                        if (input == "E")
                        {
                            Console.Clear();
                            return null;
                        }
                        if (step == 0) title = input;
                        if (step == 1) description = input;
                        step++;
                    }
                }

                if (mode == "Generate with AI")
                {
                    return await GenerateProjectWithGeminiAI(currentUser, userService);
                }
            }
        }

        public void AddCharacter(Character character, UserService userService)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));

            Characters ??= new List<Character>();
            character.orderInProject = (Characters?.Count ?? 0) + 1;
            Characters.Add(character);

            userService?.SaveUserService();
        }

        public void Show()
        {
            var worldNames = Worlds != null && Worlds.Any()
                ? string.Join(", ", Worlds.Select(w => Markup.Escape(w.Name)))
                : "None";

            var allTitles = Storylines != null && Storylines.Any()
                ? string.Join(", ", Storylines.Select(s => Markup.Escape(s.Title)))
                : "None";

            var characterNames = Characters != null && Characters.Any()
                ? string.Join(", ", Characters.Select(c => Markup.Escape(c.Name)))
                : "None";

            var table = new Table()
                .Border(TableBorder.Simple)
                .BorderColor(Color.Orange1)
                .AddColumn(new TableColumn("").RightAligned())
                .AddColumn(new TableColumn($"[bold orange1]{Markup.Escape(title)}[/]").LeftAligned());

            table.AddRow("Description:", Markup.Escape(description));

            if (!string.IsNullOrWhiteSpace(highConcept))
            {
                table.AddRow("High Concept:", Markup.Escape(highConcept));
                table.AddRow("Campaign Goal:", Markup.Escape(campaignGoal));
                table.AddRow("Themes:", Markup.Escape(themes));
            }

            table.AddEmptyRow();
            table.AddRow("Storylines:", Markup.Escape(allTitles));
            table.AddEmptyRow();
            table.AddRow("Worlds:", Markup.Escape(worldNames));
            table.AddEmptyRow();
            table.AddRow("Characters:", Markup.Escape(characterNames));

            var panel = new Panel(table)
            {
                Border = BoxBorder.Rounded,
            }.BorderColor(Color.Orange1);

            AnsiConsole.Write(panel);
        }

        public void ShowAll(Project project)
        {
            ShowSection("Project", () => Show());
            ShowAllStorylines(project);
            ShowAllWorlds();
            ShowAllCharacters();
        }

        private void ShowSection(string header, Action action)
        {
            BigHeader.Show(header);
            action();
        }

        public void ShowAllStorylines(Project project)
        {
            while (true)
            {
                var choices = project.Storylines
                .Select(s => string.IsNullOrWhiteSpace(s.Title) ? "(untitled)" : s.Title)
                .ToList();
                choices.Add("Back");

                var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
               .Title("[#FFA500]Select storyline to show[/]")
               .HighlightStyle(new Style(Color.Orange1))
               .PageSize(10)
               .AddChoices(choices));

                if (selected == "Back")
                {
                    Console.Clear();   // OK: rensa och lämna tillbaka till ShowAll
                    break;
                }

                var obj = project.Storylines.First(s => s.Title == selected);
                Console.Clear();
                ShowInfoCard.ShowObject(obj);
                Console.WriteLine();
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
                Console.Clear();       // rensa och gå tillbaka till samma while (Select storyline to show)
            }
        }

        public void ShowAllWorlds()
        {
            ShowSection("Worlds", () =>
            {
                if (Worlds != null && Worlds.Any())
                {
                    foreach (var world in Worlds)
                    {
                        world.Show();
                        Console.WriteLine();
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]No Worlds found.[/]");
                }
            });
        }

        public void ShowAllCharacters()
        {
            ShowSection("Characters", () =>
            {
                if (Characters != null && Characters.Any())
                {
                    foreach (var character in Characters)
                        character.Show();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]No Characters found.[/]");
                }
            });
        }

        public async Task<Project?> GenerateProjectWithGeminiAI(User currentUser, UserService userService)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            var googleApiKey = config["GoogleAI:ApiKey"];
            var aiService = new GeminiAIService(googleApiKey);

            while (true)
            {
                string userContext = AiHelper.AskOptionalUserContext("Generate Project with AI");
                if (userContext == "E") return null;

                string prompt = string.IsNullOrWhiteSpace(userContext)
                    ? @"You are a D&D campaign designer. Create a complete campaign project.

TASK: Generate a unique RPG campaign that fits a text-based D&D game.
RULES: All fields MUST be filled. Use unique names. No explanations, no markdown.

FORMAT:
Title: [campaign title]
Description: [short campaign description]
High Concept: [1-sentence elevator pitch]
Campaign Goal: [main campaign objective]
Themes: [themes separated by |]"
                    : $@"You are a D&D campaign designer. Create a complete campaign project based on this request:

{userContext}

FORMAT:
Title: [campaign title]
Description: [short campaign description]
High Concept: [1-sentence elevator pitch]
Campaign Goal: [main campaign objective]
Themes: [themes separated by |]";

                AiHelper.ShowGeneratingText("Project");

                var result = await aiService.GenerateAsync(prompt);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    int nextOrder = (currentUser.Projects?.Count ?? 0) + 1;
                    var newProject = ParseAITextToProject(result, nextOrder);

                    if (newProject != null)
                    {
                        Console.Clear();
                        ConsoleHelpers.Info("Generated Project:");
                        Console.WriteLine();
                        AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{Markup.Escape(newProject.title)}[/]");
                        AnsiConsole.MarkupLine($"[grey]Description:[/] {Markup.Escape(newProject.description)}");
                        AnsiConsole.MarkupLine($"[grey]High Concept:[/] {Markup.Escape(newProject.highConcept)}");
                        AnsiConsole.MarkupLine($"[grey]Campaign Goal:[/] {Markup.Escape(newProject.campaignGoal)}");
                        AnsiConsole.MarkupLine($"[grey]Themes:[/] {Markup.Escape(newProject.themes)}");
                        Console.WriteLine();

                        var choice = AiHelper.RetryMenu();

                        if (choice == "Keep")
                        {
                            currentUser.Projects.Add(newProject);
                            userService.SaveUserService();
                            AiHelper.ShowSaved("Project", newProject.title);
                            return newProject;
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

        public static Project? ParseAITextToProject(string input, int nextOrderInProject)
        {
            var project = new Project();
            var lines = input.Replace("\r\n", "\n").Split('\n'); // robust newline-hantering [web:50][web:56]

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.StartsWith("Title:", StringComparison.OrdinalIgnoreCase))
                    project.title = cleanLine.Substring(6).Trim();
                else if (cleanLine.StartsWith("Description:", StringComparison.OrdinalIgnoreCase))
                    project.description = cleanLine.Substring(12).Trim();
                else if (cleanLine.StartsWith("High Concept:", StringComparison.OrdinalIgnoreCase))
                    project.highConcept = cleanLine.Substring(12).Trim();
                else if (cleanLine.StartsWith("Campaign Goal:", StringComparison.OrdinalIgnoreCase))
                    project.campaignGoal = cleanLine.Substring(14).Trim();
                else if (cleanLine.StartsWith("Themes:", StringComparison.OrdinalIgnoreCase))
                    project.themes = cleanLine.Substring(7).Trim();
            }

            project.orderInProject = nextOrderInProject;
            project.DateOfCreation = DateTime.Now;
            project.Storylines = new List<Storyline>();
            project.Characters = new List<Character>();
            project.Worlds = new List<World>();

            return string.IsNullOrWhiteSpace(project.title) || string.IsNullOrWhiteSpace(project.description)
                ? null : project;
        }

        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
        public void Filter()
        {
            Console.WriteLine("Coming soon");
        }
        public void Search()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
