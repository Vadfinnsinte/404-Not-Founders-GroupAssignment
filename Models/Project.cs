using System;
using System.Collections.Generic;
using System.Linq;
using _404_not_founders.Menus;
using _404_not_founders.Services;
using _404_not_founders.UI;
using Spectre.Console;

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

        public Project Add(User currentUser, UserService userService)
        {
            string title = "";
            string description = "";
            int step = 0;

            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[underline #FFA500]Create New Project[/]");
                AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");
                Console.WriteLine();

                // Show previous inputs
                if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Title:[/] {title}");
                if (step >= 2)
                    AnsiConsole.MarkupLine($"[#FFA500]Description:[/] {description}");
                string prompt = step switch
                {
                    0 => "Enter project title:",
                    1 => "Enter project description:",
                    _ => ""
                };

                if (step == 2)
                {
                    // Confirm
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Save this project?[/]")
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

                // Handle B
                if (input == "B")
                {
                    if (step > 0) step--;
                    continue;
                }

                // Handle E
                if (input == "E")
                {
                    Console.Clear();
                    return null;

                }

                // Store values
                if (step == 0) title = input;
                if (step == 1) description = input;

                step++;
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
        public void ShowAll(UserService userService, ProjectService projectService, MenuHelper menuHelper)
        {
            //AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
            ShowSection("Project", () => Show());

            ShowAllStorylines();

            ShowAllWorlds();

            ShowAllCharacters(userService, projectService, menuHelper);


            AnsiClearHelper.WaitForKeyAndClear();


   
            return;
        }

        private void ShowSection(string header, Action action)
        {
            BigHeader.Show(header);
            action();
        }
        public void ShowAllStorylines()
        {
            ShowSection("Storylines", () =>
            {
                if (Storylines != null && Storylines.Any())
                    foreach (var story in Storylines)
                        story.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No storylines found.[/]");
            });
        }
        public void ShowAllWorlds()
        {
            ShowSection("Worlds", () =>
            {
                if (Worlds != null && Worlds.Any())
                    foreach (var world in Worlds)
                        world.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No worlds found.[/]");
            });
        }
        public void ShowAllCharacters(UserService userService, ProjectService projectService, MenuHelper menuHelper)
        {
            ShowSection("Characters", () =>
            {
                if (Characters != null && Characters.Any())
                    foreach (var character in Characters)
                        character.ShowCharacters(userService, projectService, menuHelper, this);
                else
                    AnsiConsole.MarkupLine("[grey]No characters found.[/]");
            });
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
