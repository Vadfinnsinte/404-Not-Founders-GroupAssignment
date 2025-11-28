using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.CRUD.Storylines;
using _404_not_founders.UI.Helpers;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.Menus
{
    public class StorylineChoisesMenu
    {

        private readonly UserService _userService;

        public StorylineChoisesMenu(UserService userService)
        {

            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        public void StorylineMenu(Project project)
        {
            bool runStoryline = true;
            while (runStoryline)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Storylines[/]")
                        .AddChoices("Add Storyline", "Show Storylines", "Edit Storyline", "Delete Storyline", "Back")
                        .HighlightStyle(Color.Orange1));

                switch (choice)
                {
                    case "Add Storyline":
                        AddStorylineToProject(project);
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
                        if (project.Storylines == null || project.Storylines.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Storylines to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }

                        var storylineChoices = project.Storylines.Select(w => w.Title).ToList();

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
            var original = SelectStoryline(project, "Choose storyline to edit");
            if (original == null) return;

            EditStoryline.Edit(project, original, _userService);
           
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
