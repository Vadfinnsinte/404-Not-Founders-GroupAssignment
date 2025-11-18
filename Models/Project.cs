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
                Storylines = new List<Storyline>()
            };

            // Add project to user's project list and save
            currentUser.Projects.Add(newProject);
            userService.SaveUserService();
            return (newProject);

        }
        public void Show(Project project)
        {
            var worldNames = project.Worlds != null && project.Worlds.Any()
                ? string.Join(", ", project.Worlds.Select(w => w.Name))
                : "None";

            var allTitles = project.Storylines != null && project.Storylines.Any()
                ? string.Join(", ", project.Storylines.Select(s => s.Title))
                : "None";

            var table = new Table()
                .Border(TableBorder.Simple)
                .BorderColor(Color.Orange1)
                .AddColumn(new TableColumn("").RightAligned())
                .AddColumn(new TableColumn($"[bold orange1]{Markup.Escape(project.title)}[/]").LeftAligned());

            table.AddRow("Description:", Markup.Escape(project.description));
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
            AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
            ShowSection("Project", () => Show(project));
            ShowSection("Storylines", () =>
            {
                if (project.Storylines != null && project.Storylines.Any())
                    foreach (var story in project.Storylines)
                        story.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No storylines found.[/]");
            });

            ShowSection("Worlds", () =>
            {
                if (project.Worlds != null && project.Worlds.Any())
                    foreach (var world in project.Worlds)
                        world.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No worlds found.[/]");
            });

            //ShowSection("Characters", () =>
            //{
            //    if (project.Characters != null && project.Characters.Any())
            //        foreach (var character in project.Characters)
            //            character.Show();
            //    else
            //        AnsiConsole.MarkupLine("[grey]No characters found.[/]");
            //});

            AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
            Console.ReadKey(true);
            return;
        }

        private void ShowSection(string header, Action action)
        {
            BigHeader.Show(header);
            action();
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
