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
        public DateTime dateOfCreation;
        public Guid Id { get; set; } = Guid.NewGuid();
        //public List<Storyline> Storylines;
        public List<World> Worlds { get; set; } = new List<World>();
        public List<Storyline> Storylines { get; set; } = new();

        public List<Character> Characters { get; set; } = new List<Character>();

        public Project Add(User currentUser, UserService userService)
        {
            // Prompt user for project details
            string addProjectName = AnsiConsole.Ask<string>("[#FFA500]Enter project title:[/]");
            string addProjectDescription = AnsiConsole.Ask<string>("[#FFA500]Enter project description:[/]");

            // Create new project
            var newProject = new Project
            {
                title = addProjectName,
                description = addProjectDescription,
                dateOfCreation = DateTime.Now,
                Storylines = new List<Storyline>(),
                Characters = new List<Character>()
             };

            // Add project to user's project list and save
            currentUser.Projects.Add(newProject);
            userService.SaveUserService();
            return (newProject);

        }

        public void AddCharacter(Character character, UserService userService)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));

            Characters ??= new List<Character>();

            if (Characters.Any(c => string.Equals(c.Names, character.Names, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A character with the name '{character.Names}' already exists in project '{title}'.");
            }

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
        public void ShowAll()
        {
            AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
            ShowSection("Project", () => Show());

            ShowAllStorylines();

            ShowAllWorlds();
            //ShowSection("Characters", () =>
            //{
            //    if (project.Characters != null && project.Characters.Any())
            //        foreach (var character in project.Characters)
            //            character.Show();
            //    else
            //        AnsiConsole.MarkupLine("[grey]No characters found.[/]");
            //});
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
