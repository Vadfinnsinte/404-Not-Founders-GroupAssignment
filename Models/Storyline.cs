using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;
using Microsoft.Extensions.Configuration;

namespace _404_not_founders.Models
{
    /// Represents a campaign storyline/plot arc in the D&D project
    /// Includes narrative elements and maintains order within the project
    public class Storyline
    {
        // Storyline properties
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Theme { get; set; }
        public string Genre { get; set; }
        public string Story { get; set; }
        public string IdeaNotes { get; set; }
        public string OtherInfo { get; set; }
        public int orderInProject { get; set; }  // Position in storyline sequence

        /// Displays the storyline information in a formatted card
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }

        /// Deletes this storyline from the project after user confirmation
        /// Automatically adjusts the order of remaining storylines
        public void DeleteStoryline(Project project, UserService userService)
        {
            Console.Clear();

            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{Markup.Escape(this.Title)}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                if (project.Storylines.Contains(this))
                {
                    int deletedOrder = this.orderInProject;
                    project.Storylines.Remove(this);

                    // Adjust order of remaining storylines (shift down)
                    foreach (var sl in project.Storylines.Where(s => s.orderInProject > deletedOrder))
                    {
                        sl.orderInProject--;
                    }

                    userService.SaveUserService();

                    AnsiConsole.MarkupLine($"The storyline '[orange1]{Markup.Escape(this.Title)}[/]' has been deleted!");
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

        /// Generates a storyline using Google Gemini AI with Retry/Keep/Cancel workflow
        /// Creates unique, unpredictable storylines that fit the project context
        /// Includes optional user customization
        public async Task<Storyline?> GenerateStorylineWithGeminiAI(Project currentProject, UserService userService)
        {
            // Load AI configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            var googleApiKey = config["GoogleAI:ApiKey"];
            var aiService = new GeminiAIService(googleApiKey);

            while (true)
            {
                // Get optional user customization
                string userContext = AiHelper.AskOptionalUserContext("Generate Storyline with AI");
                if (userContext == "E")
                    return null;

                // Base AI prompt with strict formatting requirements
                string basePrompt = $@"You are a creative Storyline generator for a text-based Dungeons & Dragons RPG project.

PROJECT CONTEXT:
{currentProject.description}

TASK:
Create a unique, unpredictable fantasy Storyline for this project.
Never repeat plot, theme, motif, or title from previous generations.

RULES:
- You MUST output ALL headers exactly once: Title, Synopsis, Theme, Genre, Story, IdeaNotes, OtherInfo.
- Each field MUST have non-empty content (at least a full sentence for Story, and meaningful text for IdeaNotes and OtherInfo).
- Do NOT skip or rename any header.
- Return ONLY the formatted text below. No explanations, no markdown, no asterisks.

FORMAT (NO markdown or asterisks, only plain text!):
Title: [unique storyline title]
Synopsis: [2-3 sentence summary]
Theme: [central theme]
Genre: [fantasy subgenre]
Story: [detailed story description]
IdeaNotes: [ideas for encounters, NPCs, twists]
OtherInfo: [pacing, tone, future hooks]";

                // Add user context if provided
                string prompt = string.IsNullOrWhiteSpace(userContext)
                    ? basePrompt
                    : basePrompt + $@"

USER REQUEST:
{userContext}

Adapt the Storyline to the user request, but keep the exact same headers and format.";

                AiHelper.ShowGeneratingText("Storyline");

                // Call AI service
                var result = await aiService.GenerateAsync(prompt);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    // Calculate next order position
                    int nextOrder = (currentProject.Storylines?.Count ?? 0) + 1;
                    var newStoryline = ParseAITextToStoryline(result, nextOrder);

                    if (newStoryline != null)
                    {
                        // Display generated result
                        Console.Clear();
                        ConsoleHelpers.Info("Generated Storyline:");
                        Console.WriteLine();

                        // Use the same formatted view as storyline.Show()
                        newStoryline.Show();

                        Console.WriteLine();

                        // Retry/Keep/Cancel menu
                        var choice = AiHelper.RetryMenu();

                        if (choice == "Keep")
                        {
                            currentProject.Storylines.Add(newStoryline);
                            userService.SaveUserService();
                            AiHelper.ShowSaved("Storyline", newStoryline.Title);
                            AnsiClearHelper.WaitForKeyAndClear();
                            return newStoryline;
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
                        AiHelper.ShowError("Unable to parse generated Storyline. Retrying...");
                    }
                }
                else
                {
                    AiHelper.ShowError("Unable to generate AI-Storyline with Gemini. Retrying...");
                }
            }
        }

        /// Parses AI-generated text into a Storyline object
        /// Expects format: "Title: ...\nSynopsis: ...\nTheme: ..." etc.
        /// Provides fallback content if AI leaves certain fields empty
        /// Returns null if required fields (Title, Synopsis) are missing
        public static Storyline? ParseAITextToStoryline(string input, int nextOrderInProject)
        {
            var storyline = new Storyline();
            var lines = input.Replace("\r\n", "\n").Split('\n');

            // Parse each line
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
            }

            // Ensure non-null values
            storyline.Title ??= "";
            storyline.Synopsis ??= "";
            storyline.Theme ??= "";
            storyline.Genre ??= "";
            storyline.Story ??= "";
            storyline.IdeaNotes ??= "";
            storyline.OtherInfo ??= "";

            // Provide fallback content if AI left fields empty
            if (string.IsNullOrWhiteSpace(storyline.Story))
                storyline.Story = $"Short story based on synopsis: {storyline.Synopsis}";
            if (string.IsNullOrWhiteSpace(storyline.IdeaNotes))
                storyline.IdeaNotes = "Extra ideas for encounters, NPCs, and twists related to the main plot.";
            if (string.IsNullOrWhiteSpace(storyline.OtherInfo))
                storyline.OtherInfo = "Notes for pacing, tone, and possible future hooks.";

            // Set order position
            storyline.orderInProject = nextOrderInProject;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(storyline.Title) || string.IsNullOrWhiteSpace(storyline.Synopsis))
                return null;

            return storyline;
        }
    }
}
