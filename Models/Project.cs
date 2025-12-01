using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;
using Spectre.Console;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace _404_not_founders.Models
{

    /// Represents a D&D campaign project that contains worlds, storylines, and characters
    /// Supports both manual creation and AI-generated content
    public class Project
    {
        // Core project properties
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; } = DateTime.Now;
        public Guid Id { get; set; } = Guid.NewGuid();

        // Collections of project content
        public List<World> Worlds { get; set; } = new();
        public List<Storyline> Storylines { get; set; } = new();
        public List<Character> Characters { get; set; } = new();


        /// Interactive project creation with option for manual or AI-generated content
        /// Returns null if user exits without creating
        public async Task<Project> Add(User currentUser, UserService userService)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[underline #FFA500]Create New Project[/]");

                // Let user choose creation mode
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

                // AI generation path
                if (mode == "Generate with AI")
                {
                    var aiProject = await GenerateProjectWithGeminiAI(currentUser, userService);
                    if (aiProject != null)
                    {
                        return aiProject;
                    }
                    continue;
                }

                // Manual creation path
                if (mode == "Manually")
                {
                    string title = "";
                    string description = "";
                    int step = 0;

                    // Step-by-step input loop
                    while (true)
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[underline #FFA500]Create New Project[/]");
                        AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");
                        Console.WriteLine();

                        // Display filled fields
                        if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Title:[/] {title}");
                        if (step >= 2) AnsiConsole.MarkupLine($"[#FFA500]Description:[/] {description}");

                        string prompt = step switch
                        {
                            0 => "Enter project title:",
                            1 => "Enter project description:",
                            _ => ""
                        };

                        // Confirmation step
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
                                // Create and save new project
                                var newProject = new Project
                                {
                                    title = title,
                                    description = description,
                                    DateOfCreation = DateTime.Now,
                                    Storylines = new List<Storyline>(),
                                    Characters = new List<Character>(),
                                    Worlds = new List<World>()
                                };
                                currentUser.Projects.Add(newProject);
                                userService.SaveUserService();
                                AnsiConsole.MarkupLine("[green]Project created![/]");
                                Thread.Sleep(1200);
                                return newProject;
                            }
                        }

                        // Get input for current step
                        string input = AskStepInput.AskStepInputs(prompt);

                        // Handle navigation commands
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

                        // Store input
                        if (step == 0) title = input;
                        if (step == 1) description = input;
                        step++;
                    }
                }
            }
        }


        /// Adds a character to the project and saves changes
        /// Validates that character is not null
        public void AddCharacter(Character character, UserService userService)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));

            Characters ??= new List<Character>();
            Characters.Add(character);

            userService?.SaveUserService();
        }

        /// Displays project information in a formatted table
        /// Shows all associated worlds, storylines, and characters
        public void Show()
        {
            // Prepare display data with null checks
            var worldNames = Worlds != null && Worlds.Any()
                ? string.Join(", ", Worlds.Select(w => Markup.Escape(w.Name)))
                : "None";

            var storylineTitles = Storylines != null && Storylines.Any()
                ? string.Join(", ", Storylines.Select(s => Markup.Escape(s.Title)))
                : "None";

            var characterNames = Characters != null && Characters.Any()
                ? string.Join(", ", Characters.Select(c => Markup.Escape(c.Name)))
                : "None";

            // Create formatted table
            var table = new Table()
                .Border(TableBorder.Simple)
                .BorderColor(Color.Orange1)
                .AddColumn(new TableColumn("").RightAligned())
                .AddColumn(new TableColumn($"[bold orange1]{Markup.Escape(title)}[/]").LeftAligned());

            table.AddRow("Description:", Markup.Escape(description));
            table.AddEmptyRow();
            table.AddRow("Storylines:", Markup.Escape(storylineTitles));
            table.AddEmptyRow();
            table.AddRow("Worlds:", Markup.Escape(worldNames));
            table.AddEmptyRow();
            table.AddRow("Characters:", Markup.Escape(characterNames));

            // Wrap table in panel
            var panel = new Panel(table)
            {
                Border = BoxBorder.Rounded,
            }.BorderColor(Color.Orange1);

            AnsiConsole.Write(panel);
        }

        /// Generates a project using Google Gemini AI with Retry/Keep/Cancel workflow
        /// Includes optional user context for customization
        public async Task<Project?> GenerateProjectWithGeminiAI(User currentUser, UserService userService)
        {
            // Load AI configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string googleApiKey = config["GoogleAI:ApiKey"];
            var aiService = new GeminiAIService(googleApiKey);

            while (true)
            {
                // Get optional user customization
                string userContext = AiHelper.AskOptionalUserContext("Generate Project with AI");

                if (userContext == "E")
                    return null;

                // Build AI prompt based on whether user provided context
                string prompt = string.IsNullOrWhiteSpace(userContext)
                    ? @"You are a D&D campaign project creator.

TASK:
Generate a unique D&D campaign project concept.

RULES:
- All fields MUST be filled. No empty values allowed.
- Use a creative, unique title. Avoid generic names like 'The Adventure'.
- Return ONLY the formatted text below. No explanations, no markdown, no asterisks.
- BEFORE returning, self-check: Are ALL fields filled?

FORMAT:
Title: [unique campaign title]
Description: [2-3 sentence campaign description]"
                    : $@"You are a D&D campaign project creator.

USER REQUEST:
{userContext}

TASK:
Generate a unique D&D campaign project concept based on the user's request.

RULES:
- All fields MUST be filled. No empty values allowed.
- Use a creative, unique title that reflects the user's request.
- Return ONLY the formatted text below. No explanations, no markdown, no asterisks.
- BEFORE returning, self-check: Are ALL fields filled?

FORMAT:
Title: [unique campaign title]
Description: [2-3 sentence campaign description]";

                AiHelper.ShowGeneratingText("Project");

                // Call AI service
                string result = await aiService.GenerateAsync(prompt);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var newProject = ParseAITextToProject(result);

                    if (newProject != null)
                    {
                        // Display generated result
                        Console.Clear();
                        ConsoleHelpers.Info("Generated Project:");
                        Console.WriteLine();

                        // Use the same formatted view as everywhere else
                        newProject.Show();

                        Console.WriteLine();

                        // Retry/Keep/Cancel menu
                        var choice = AiHelper.RetryMenu();

                        if (choice == "Keep")
                        {
                            currentUser.Projects.Add(newProject);
                            userService.SaveUserService();
                            AiHelper.ShowSaved("Project", newProject.title);
                            AnsiClearHelper.WaitForKeyAndClear();
                            return newProject;
                        }
                        else if (choice == "Cancel")
                        {
                            AiHelper.ShowCancelled();
                            return null;
                        }
                        // If "Retry", loop continues
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

        /// Parses AI-generated text into a Project object
        /// Expects format: "Title: ...\nDescription: ..."
        /// Returns null if required fields are missing
        public static Project? ParseAITextToProject(string input)
        {
            var project = new Project();
            var lines = input.Replace("\r\n", "\n").Split('\n');

            // Parse each line
            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.StartsWith("Title:", StringComparison.OrdinalIgnoreCase))
                    project.title = cleanLine.Substring(6).Trim();
                else if (cleanLine.StartsWith("Description:", StringComparison.OrdinalIgnoreCase))
                    project.description = cleanLine.Substring(12).Trim();
            }

            // Ensure non-null values
            project.title ??= "";
            project.description ??= "";

            // Validate required fields
            if (string.IsNullOrWhiteSpace(project.title) || string.IsNullOrWhiteSpace(project.description))
                return null;

            return project;
        }
    }
}
