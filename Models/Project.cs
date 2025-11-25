using System;                               // Console, Thread
using System.Collections.Generic;           // List<T>
using System.Linq;                          // LINQ
using _404_not_founders.Menus;              // MenuChoises
using _404_not_founders.Services;           // UserService, DungeonMasterAI
using _404_not_founders.UI;                 // BigHeader, AnsiClearHelper, AskStepInput
using Spectre.Console;                      // AnsiConsole, Markup, Table, Panel, Color, BoxBorder, SelectionPrompt
using Microsoft.Extensions.Configuration;   // ConfigurationBuilder

namespace _404_not_founders.Models
{
    public class Project
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime DateOfCreation { get; set; } = DateTime.Now;
        public Guid Id { get; set; } = Guid.NewGuid();
     
        public List<World> Worlds { get; set; } = new List<World>();
        public List<Storyline> Storylines { get; set; } = new();

        public List<Character> Characters { get; set; } = new List<Character>();

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
                            "Manually",
                            "Generate with AI",
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

                        if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Title:[/] {title}");
                        if (step >= 2) AnsiConsole.MarkupLine($"[#FFA500]Description:[/] {description}");

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
                                    Characters = new List<Character>()
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
                    var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();
                    string googleApiKey = config["GoogleAI:ApiKey"];
                    var aiHelper = new GeminiAIService(googleApiKey);

                    Console.WriteLine("Describe your Project/Campaign, or press [Enter] for a fantasy default:");
                    var input = Console.ReadLine();
                    string prompt = string.IsNullOrWhiteSpace(input)
                        ? "Create a new RPG campaign Project concept. Include a campaign title, description, story hook, and themes."
                        : input;

                    Console.WriteLine("Generating with Gemini...");
                    string result = await aiHelper.GenerateAsync(prompt);

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Console.WriteLine("\nYour Gemini-generated Project:\n" + result);

                        Console.WriteLine("Enter a title for your Project (or copy paste from above):");
                        string aiTitle = Console.ReadLine();
                        Console.WriteLine("Enter a description for your Project (or copy paste from above):");
                        string aiDesc = Console.ReadLine();

                        var newProject = new Project
                        {
                            title = aiTitle,
                            description = aiDesc,
                            DateOfCreation = DateTime.Now,
                            Storylines = new List<Storyline>(),
                            Characters = new List<Character>()
                        };

                        currentUser.Projects.Add(newProject);
                        userService.SaveUserService();
                        AnsiConsole.MarkupLine("[green]Project created from Gemini AI![/]");
                        Thread.Sleep(1200);
                        return newProject;
                    }
                    else
                    {
                        Console.WriteLine("Unable to generate AI-Answer.");
                        Thread.Sleep(1200);
                        Console.WriteLine($"ApiKey: {googleApiKey}");
                        Console.WriteLine($"Prompt: {prompt}");
                        continue;
                    }
                }
            }
        }

        public void AddCharacter(Character character, UserService userService)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));

            Characters ??= new List<Character>();
     
            Characters.Add(character);

            // Persist entire userstore (caller is expected to supply the same UserService instance used by the app)
            userService?.SaveUserService();
        }


        public void Show()
        {
            var worldNames = Worlds != null && Worlds.Any()
                ? string.Join(", ", Worlds.Select(w => w.Name))
                : "None";

            var allTitles = Storylines != null && Storylines.Any()
                ? string.Join(", ", Storylines.Select(s => s.Title))
                : "None";

            var table = new Table()
                .Border(TableBorder.Simple)
                .BorderColor(Color.Orange1)
                .AddColumn(new TableColumn("").RightAligned())
                .AddColumn(new TableColumn($"[bold orange1]{Markup.Escape(title)}[/]").LeftAligned());

            table.AddRow("Description:", Markup.Escape(description));
            table.AddEmptyRow();
            table.AddRow("Storylines:", Markup.Escape(allTitles));
            table.AddEmptyRow();
            table.AddRow("Worlds:", Markup.Escape(worldNames));

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
            // Ingen prompt här – navigation styrs i menysystemet!
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
                var choices = project.Storylines.Select(s => string.IsNullOrWhiteSpace(s.Title) ? "(untitled)" : s.Title).ToList();
                choices.Add("Back");

                var selected = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Select storyline to show[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .PageSize(10)
                        .AddChoices(choices));

                if (selected == "Back")
                {
                    Console.Clear();
                    break;
                }

                var obj = project.Storylines.First(s => s.Title == selected);
                Console.Clear();
                ShowInfoCard.ShowObject(obj);
                Console.WriteLine();
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
                Console.Clear();
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
                        world.Show();                                // Visar info för varje world
                        Console.WriteLine();                         // Extra radbrytning mellan worlds
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
                    foreach (var character in Characters)
                        character.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No Characters found.[/]");
            });
        }

        public async Task<Project?> GenerateProjectWithGeminiAI(Project project, User currentUser, UserService userService)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string googleApiKey = config["GoogleAI:ApiKey"];
            var aiHelper = new GeminiAIService(googleApiKey);

            Console.WriteLine("Describe your Project/Campaign, or press [Enter] for a fantasy default:");
            var input = Console.ReadLine();

            string prompt = string.IsNullOrWhiteSpace(input)
                ? "Invent a brand-new, wildly original RPG Project concept for a campaign. Never reuse names, overarching plots, or quest ideas from previous answers. Surprise me with new campaign titles, hooks, and themes! Format(Without asterisks or markdown): Title: ..., Description: ..."
                : input;

            Console.WriteLine("Generating AI Project...");
            var result = await aiHelper.GenerateAsync(prompt);

            if (!string.IsNullOrWhiteSpace(result))
            {
                Console.WriteLine("\nYour AI-generated Project:\n");
                Console.WriteLine(result);

                // PARSA direkt och skapa projekt
                var newProject = Project.ParseAITextToProject(result);
                if (newProject != null)
                {
                    newProject.DateOfCreation = DateTime.Now;
                    newProject.Storylines = new List<Storyline>();
                    newProject.Characters = new List<Character>();

                    currentUser.Projects.Add(newProject);
                    userService.SaveUserService();
                    ConsoleHelpers.Info($"Project '{newProject.title}' created!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();

                    while (true)
                    {
                        var choice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]What do you want to do next?[/]")
                                .HighlightStyle(Color.Orange1)
                                .AddChoices("Show Project", "Back"));

                        if (choice == "Show Project")
                            newProject.ShowAll(project);
                        else if (choice == "Back")
                            break;
                    }
                    return newProject;
                }
                else
                {
                    Console.WriteLine("Could not parse AI Project result. Please check the output format.");
                    Console.ReadKey();
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Kunde inte generera AI-svar.");
                Console.ReadKey();
                return null;
            }
        }

        public static Project? ParseAITextToProject(string input)
        {
            var project = new Project();
            var lines = input.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("Title:", StringComparison.OrdinalIgnoreCase))
                    project.title = line.Substring(6).Trim();
                else if (line.StartsWith("Description:", StringComparison.OrdinalIgnoreCase))
                    project.description = line.Substring(12).Trim();
                // Lägg till fler fält om din AI-output innehåller t.ex. Storylines, Worlds, etc (parsa dem separat)
            }
            // Kontroll: måste ha titel och beskrivning
            if (string.IsNullOrWhiteSpace(project.title) || string.IsNullOrWhiteSpace(project.description))
                return null;
            return project;
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
