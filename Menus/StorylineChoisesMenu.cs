using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.CRUD.Storylines;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    /// Menu for managing Storylines inside a selected Project.
    /// Handles AI generation, manual add, show, edit and delete operations.
    public class StorylineChoisesMenu
    {
        private readonly UserService _userService;

        /// Constructor with dependency injection for UserService.
        public StorylineChoisesMenu(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// Main storyline menu loop for a specific project.
        /// Lets the user generate, add, show, edit and delete storylines.
        public async Task StorylineMenu(Project project, UserService userService)
        {
            bool runStoryline = true;

            while (runStoryline)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Storylines[/]")
                        .AddChoices(
                            "Generate Storyline with AI",
                            "Add Storyline",
                            "Show Storylines",
                            "Edit Storyline",
                            "Delete Storyline",
                            "Back")
                        .HighlightStyle(Color.Orange1));

                switch (choice)
                {
                    case "Generate Storyline with AI":
                        {
                            // Generate a new Storyline using Gemini AI for this project
                            var storylineForAi = new Storyline();
                            await storylineForAi.GenerateStorylineWithGeminiAI(project, userService);
                            break;
                        }

                    case "Add Storyline":
                        // Manual, step-by-step storyline creation (with orderInProject handling)
                        CreateStoryline.Create(project, _userService);
                        break;

                    case "Show Storylines":
                        // Show all storylines for the current project
                        var show = new ShowEverything(project);
                        show.ShowAllStorylines();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;

                    case "Edit Storyline":
                        // Let user pick which storyline to edit (ordered by orderInProject)
                        var original = SelectStoryline(project, "Choose storyline to edit");
                        if (original == null)
                            break;

                        EditStoryline.Edit(project, original, _userService);
                        break;

                    case "Delete Storyline":
                        // Guard: no storylines to delete
                        if (project.Storylines == null || project.Storylines.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Storylines to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }

                        // Build selection list "1. Title", "2. Title", ...
                        var storylineChoices = project.Storylines
                            .OrderBy(s => s.orderInProject)
                            .Select(s => $"{s.orderInProject}. {s.Title}")
                            .ToList();

                        storylineChoices.Add("Back to Menu");

                        var selectedStoryline = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose storyline to Remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(storylineChoices));

                        // User chose to go back
                        if (selectedStoryline == "Back to Menu")
                        {
                            break;
                        }

                        // Extract the title part after "N. "
                        var separatorIndex = selectedStoryline.IndexOf(". ");
                        var titleOnly = separatorIndex >= 0
                            ? selectedStoryline[(separatorIndex + 2)..]
                            : selectedStoryline;

                        // Find and delete the selected storyline
                        var storylineToDelete = project.Storylines.First(s => s.Title == titleOnly);
                        storylineToDelete.DeleteStoryline(project, _userService);
                        break;

                    case "Back":
                        // Exit storyline menu
                        runStoryline = false;
                        break;
                }
            }
        }

        /// Lets the user select a storyline from the project, ordered by orderInProject.
        /// Returns null if there are no storylines.
        private Storyline? SelectStoryline(Project project, string title)
        {
            if (project.Storylines == null || project.Storylines.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No storylines yet.[/]");
                Console.ReadKey(true);
                Console.Clear();
                return null;
            }

            var sorted = project.Storylines
                .OrderBy(s => s.orderInProject)
                .ToList();

            return AnsiConsole.Prompt(
                new SelectionPrompt<Storyline>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(sorted)
                    .UseConverter(s => $"{s.orderInProject}. {s.Title}"));
        }
    }
}
