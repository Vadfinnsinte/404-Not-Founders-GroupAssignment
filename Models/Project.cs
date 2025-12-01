
using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

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

            // Loop that handles steps for project creation
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[underline #FFA500]Create New Project[/]");
                AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");
                Console.WriteLine();

                // Display current inputs
                if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Title:[/] {title}");
                if (step >= 2)
                    AnsiConsole.MarkupLine($"[#FFA500]Description:[/] {description}");
                string prompt = step switch
                {
                    0 => "Enter project title:",
                    1 => "Enter project description:",
                    _ => ""
                };

                // Handle confirmation step
                if (step == 2)
                {

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

        // Method to add a character to the project
        public void AddCharacter(Character character, UserService userService)
        {
            // Validate input
            if (character == null) throw new ArgumentNullException(nameof(character));

            // Initialize Characters list if null
            Characters ??= new List<Character>();
            character.orderInProject = (Characters?.Count ?? 0) + 1;
            Characters.Add(character);

            userService?.SaveUserService();
        }

        public void Show()
        {
            // Ensure content is not null
            var worldNames = Worlds != null && Worlds.Any()
                ? string.Join(", ", Worlds.Select(w => Markup.Escape(w.Name)))
                : "None";

            var allTitles = Storylines != null && Storylines.Any()
                ? string.Join(", ", Storylines.Select(s => Markup.Escape(s.Title)))
                : "None";

            // Create and configure the table
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

            // Create and configure the panel
            var panel = new Panel(table)
            {
                Border = BoxBorder.Rounded,
            }.BorderColor(Color.Orange1);

            AnsiConsole.Write(panel);
        }
       

    }
}
