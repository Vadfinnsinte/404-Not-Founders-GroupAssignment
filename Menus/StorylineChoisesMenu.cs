using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.CRUD.Storylines;
using _404_not_founders.UI.Helpers;
using Spectre.Console;
using Microsoft.Extensions.Configuration;

namespace _404_not_founders.Menus
{
    public class StorylineChoisesMenu
    {

        private readonly UserService _userService;

        // Constructor with dependency injection
        public StorylineChoisesMenu(UserService userService)
        {

            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task StorylineMenu(Project project, UserService userService)
        {
            // Storyline menu loop
            bool runStoryline = true;
            while (runStoryline)
            {
                // Show storyline menu choices
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Storylines[/]")
                        .AddChoices("Generate Storyline with AI", "Add Storyline", "Show Storylines", "Edit Storyline", "Delete Storyline", "Back")
                        .HighlightStyle(Color.Orange1));

                // Handle storyline menu choices and call appropriate methods
                switch (choice)
                {
                    case "Generate Storyline with AI":
                        {
                            var storylineForAi = new Storyline();
                            await storylineForAi.GenerateStorylineWithGeminiAI(project, userService);
                        }
                        break;
                    case "Add Storyline":
                        var newStoryline = new Storyline();
                        newStoryline.Add();
                        break;
                    case "Show Storylines":
                        ShowEverything show = new ShowEverything(project);
                        show.ShowAllStorylines();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;
                    case "Edit Storyline":
                        EditStorylineMenu(project);
                        break;
                    case "Delete Storyline":
                        // Handle if no storylines exist
                        if (project.Storylines == null || project.Storylines.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Storylines to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }

                        // Show list of storylines to delete
                        var storylineChoices = project.Storylines.Select(w => w.Title).ToList();

                        // Add option to go back to menu
                        storylineChoices.Add("Back to Menu");

                        // Prompt user to select storyline to delete or go back
                        var selectedStoryline = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose storyline to Remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(storylineChoices));

                        // Handle going back to menu
                        if (selectedStoryline == "Back to Menu")
                        {
                            break;
                        }

                        // Find and delete the selected storyline
                        var storylineToDelete = project.Storylines.First(w => w.Title == selectedStoryline);
                        storylineToDelete.DeleteStoryline(project, _userService);
                        break;

                    case "Back":
                        runStoryline = false;
                        break;
                }
            }
        }


        private void AddStorylineToProject(Project project)
        {
            CreateStoryline.Create(project);
            _userService.SaveUserService();

        }
        private void EditStorylineMenu(Project project)
        {
            // Select storyline to edit and create a temporary copy for editing
            var original = SelectStoryline(project, "Choose storyline to edit");
            if (original == null) return;

            EditStoryline.Edit(project, original, _userService);
           
        }
        private Storyline? SelectStoryline(Project project, string title)
        {
            // Handle if no storylines exist
            if (project.Storylines == null || project.Storylines.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No storylines yet.[/]");
                Console.ReadKey(true);

                Console.Clear();
                return null;
            }

            // Show sorted list of storylines to select from
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