using _404_not_founders.Services;               // UserService, ProjectService, DungeonMasterAI
using _404_not_founders.UI;                     // ConsoleHelpers, ShowInfoCard
using Spectre.Console;                          // Meny, markeringar och prompts
using Microsoft.Extensions.Configuration;       // För API-nyckel och config

namespace _404_not_founders.Models
{
    public class Storyline
    {
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Theme { get; set; }
        public string Genre { get; set; }
        public string Story { get; set; }
        public string IdeaNotes { get; set; }
        public string OtherInfo { get; set; }
        public int orderInProject { get; set; }
        public List<World> worlds;
        public List<Character> chracters;
        public DateTime dateOfLastEdit;



        public void Add()
        {
            Console.WriteLine("Coming soon");
        }
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void DeleteStoryline(Project project, UserService userService)
        {
            Console.Clear();

            // Ask for confirmation
            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{this.Title}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                // Remove the world from the project's worlds list
                if (project.Storylines.Contains(this))
                {
                    project.Storylines.Remove(this);

                    // Save changes
                    userService.SaveUserService();

                    AnsiConsole.MarkupLine($"The storyline '[orange1]{this.Title}[/]' has been deleted!");
                    Thread.Sleep(1200);
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]Error: Storyline not found.[/]");
                    Thread.Sleep(1200);
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
                Thread.Sleep(1200);
            }
        }

        public async Task<Storyline?> GenerateStorylineWithGeminiAI(Project currentProject, UserService userService)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string googleApiKey = config["GoogleAI:ApiKey"];
            var aiHelper = new GeminiAIService(googleApiKey);

            Console.WriteLine("Describe your Storyline or press [Enter] for a fantasy default:");
            var input = Console.ReadLine();

            // Förbättrad prompt!
            string prompt = string.IsNullOrWhiteSpace(input)
            ? $@"Create an unpredictable and unique fantasy Storyline for a text-based RPG project.
                Never repeat plot, theme, motif, or title from previous generations.
                You must always echo every field header below—never omit any field, even if blank!
                Base your entire Storyline on this Project description: '{currentProject.description}'

                Format (NO markdown or asterisks, only plain text!):
                Title: ...
                Synopsis: ...
                Theme: ...
                Genre: ...
                Story: ...
                IdeaNotes: ...
                OtherInfo: ...

                Example:
                Title: The Aetheric Loom
                Synopsis: [your text here]
                Theme: [your text here]
                Genre: [your text here]
                Story: [your text here]
                IdeaNotes: [your text here]
                OtherInfo: [your text here]"
            : input + "\n\nFormat as above. Always include all headers, even if empty. Base your content on the current Project description.";


            Console.WriteLine("Generating Storyline with Gemini...");
            string result = await aiHelper.GenerateAsync(prompt);

            int nextOrder = (currentProject.Storylines?.Count ?? 0) + 1;

            if (!string.IsNullOrWhiteSpace(result))
            {
                Console.WriteLine("\nYour Gemini-generated Storyline:\n" + result);
                var newStoryline = ParseAITextToStoryline(result, nextOrder);
                if (newStoryline != null)
                {
                    currentProject.Storylines.Add(newStoryline);
                    userService.SaveUserService();
                    ConsoleHelpers.Info($"Storyline '{newStoryline.Title}' created!");
                    Console.Clear();
                    ShowInfoCard.ShowObject(newStoryline);

                    var postChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Done viewing your new Storyline![/]")
                            .HighlightStyle(Color.Orange1)
                            .AddChoices("Back"));

                    if (postChoice == "Back")
                    {
                        Console.Clear();
                        return newStoryline;
                    }
                }
                else
                {
                    Console.WriteLine("Unable to parse generated Storyline. Check the format.");
                    Console.ReadKey(true);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Unable to generate AI-Storyline with Gemini.");
                Thread.Sleep(1200);
                return null;
            }
            return null;
        }

        // Robust parser: alltid alla fält!
        public static Storyline? ParseAITextToStoryline(string input, int nextOrderInProject)
        {
            var storyline = new Storyline();
            var lines = input.Split('\n');
            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.StartsWith("Title:", StringComparison.OrdinalIgnoreCase))
                    storyline.Title = cleanLine.Substring(6).Trim();
                else if (cleanLine.StartsWith("Synopsis:", StringComparison.OrdinalIgnoreCase))
                    storyline.Synopsis = cleanLine.Substring(9).Trim();
                else if (cleanLine.StartsWith("Theme:", StringComparison.OrdinalIgnoreCase))
                    storyline.Theme = cleanLine.Substring(6).Trim();
                else if (cleanLine.StartsWith("Genre:", StringComparison.OrdinalIgnoreCase))
                    storyline.Genre = cleanLine.Substring(6).Trim();
                else if (cleanLine.StartsWith("Story:", StringComparison.OrdinalIgnoreCase))
                    storyline.Story = cleanLine.Substring(6).Trim();
                else if (cleanLine.StartsWith("IdeaNotes:", StringComparison.OrdinalIgnoreCase))
                    storyline.IdeaNotes = cleanLine.Substring(10).Trim();
                else if (cleanLine.StartsWith("OtherInfo:", StringComparison.OrdinalIgnoreCase))
                    storyline.OtherInfo = cleanLine.Substring(10).Trim();
                else if (cleanLine.StartsWith("orderInProject", StringComparison.OrdinalIgnoreCase))
                {
                    var order = 0;
                    if (int.TryParse(cleanLine.Substring(14).Trim(' ', ':'), out order))
                        storyline.orderInProject = order;
                }
            }

            // Sätt default till tom sträng om saknas
            storyline.Title ??= "";
            storyline.Synopsis ??= "";
            storyline.Theme ??= "";
            storyline.Genre ??= "";
            storyline.Story ??= "";
            storyline.IdeaNotes ??= "";
            storyline.OtherInfo ??= "";

            // Sätt orderInProject till nästa om den är 0 eller saknas
            storyline.orderInProject = storyline.orderInProject > 0 ? storyline.orderInProject : nextOrderInProject;

            // Grundkrav: minst titel och synopsis
            if (string.IsNullOrWhiteSpace(storyline.Title) || string.IsNullOrWhiteSpace(storyline.Synopsis))
                return null;
            return storyline;
        }
    }
}