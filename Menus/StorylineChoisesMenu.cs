using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.CRUD.Storylines;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    public class StorylineChoisesMenu
    {
        private readonly UserService _userService;

        public StorylineChoisesMenu(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task StorylineMenu(Project project, UserService userService)
        {
            bool runStoryline = true;
            while (runStoryline)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Storylines[/]")
                        .AddChoices("Generate Storyline with AI", "Add Storyline", "Show Storylines", "Edit Storyline", "Delete Storyline", "Back")
                        .HighlightStyle(Color.Orange1));

                switch (choice)
                {
                    case "Generate Storyline with AI":
                        {
                            var storylineForAi = new Storyline();
                            await storylineForAi.GenerateStorylineWithGeminiAI(project, userService);
                        }
                        break;

                    case "Add Storyline":
                        CreateStoryline.Create(project, _userService);
                        break;

                    case "Show Storylines":
                        ShowEverything show = new ShowEverything(project);
                        show.ShowAllStorylines();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;

                    case "Edit Storyline":
                        var original = SelectStoryline(project, "Choose storyline to edit");
                        if (original == null) break;
                        EditStoryline.Edit(project, original, _userService);
                        break;

                    case "Delete Storyline":
                        if (project.Storylines == null || project.Storylines.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Storylines to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }

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

                        if (selectedStoryline == "Back to Menu")
                        {
                            break;
                        }

                        var titleOnly = selectedStoryline.Substring(selectedStoryline.IndexOf(". ") + 2);
                        var storylineToDelete = project.Storylines.First(w => w.Title == titleOnly);
                        storylineToDelete.DeleteStoryline(project, _userService);
                        break;

                    case "Back":
                        runStoryline = false;
                        break;
                }
            }
        }

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
