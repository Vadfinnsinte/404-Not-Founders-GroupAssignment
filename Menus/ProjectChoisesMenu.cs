using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    public class ProjectChoisesMenu
    {
        private readonly User _currentUser;
        private readonly UserService _userService;
        private readonly ProjectService _projectService;

        public ProjectChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser;
            _userService = userService;
            _projectService = projectService;
        }

        public async Task ShowProjectMenu()
        {
            if (_currentUser == null)
            {
                AnsiConsole.MarkupLine("[red]You must be logged in to view projects.[/]");
                await Task.CompletedTask;
                return;
            }

            bool runProjectMenu = true;
            while (runProjectMenu)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Project Menu[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices(
                            "Show all projects",
                            "Search projects",
                            "Back"));

                if (choice == "Back")
                {
                    runProjectMenu = false;
                    break;
                }

                if (choice == "Show all projects")
                {
                    var list = _projectService.GetAll(_currentUser);

                    if (list == null || list.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No projects yet.[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    var selected = SelectFromList(list, "Select a project");
                    if (selected == null)
                        continue;

                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    await ProjectEditMenu(selected);
                }
                else if (choice == "Search projects")
                {
                    var term = AnsiConsole.Ask<string>("Searchterm (title/description):").Trim();
                    var hits = _projectService.Search(_currentUser, term);

                    if (hits == null || hits.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No results[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    var selected = SelectFromList(hits, $"Select from search results for \"{term}\"");
                    if (selected == null)
                        continue;

                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    AnsiConsole.Clear();
                    await ProjectEditMenu(selected);
                }
            }
            await Task.CompletedTask;
        }

        private Project? SelectFromList(IReadOnlyList<Project> projects, string title)
        {
            if (projects == null || projects.Count == 0)
                return null;

            var sorted = projects.OrderByDescending(p => p.DateOfCreation).ToList();
            var displayList = sorted.Select(p => p.title).ToList();
            displayList.Add("Back");

            var selectedTitle = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold]{title}[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(displayList));

            if (selectedTitle == "Back")
            {
                AnsiConsole.Clear();
                return null;
            }

            var selectedProject = sorted.First(p => p.title == selectedTitle);
            AnsiConsole.Clear();
            return selectedProject;
        }

        public async Task ProjectEditMenu(Project project)
        {
            bool runEdit = true;

            while (runEdit)
            {
                Console.Clear();

                var choice = ProjectEditVisuals.ShowEditMenu(project);

                switch (choice)
                {
                    case "Edit/Add Characters":
                        {
                            var characterChoisesMenu = new CharacterChoisesMenu(_currentUser, _projectService, _userService);
                            await characterChoisesMenu.ChracterMenu(project, _userService);
                        }
                        break;

                    case "Edit/Add worlds":
                        if (_currentUser != null)
                        {
                            WorldChoisesMenu worldChoisesMenu = new WorldChoisesMenu(_userService);
                            await worldChoisesMenu.WorldMenu(_currentUser, project);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]No user logged in![/]");
                            ConsoleHelpers.DelayAndClear();
                        }
                        break;

                    case "Edit/Add Storylines":
                        {
                            var storylineMenu = new StorylineChoisesMenu(_userService);
                            await storylineMenu.StorylineMenu(project, _userService);
                        }
                        break;

                    case "Show Everything":
                        Console.Clear();
                        ShowEverything show = new ShowEverything(project);
                        show.ShowAll();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;

                    case "Back to main menu":
                        Console.Clear();
                        runEdit = false;
                        break;

                    default:
                        {
                            Console.WriteLine("Something went wrong... going back to menu");
                            Thread.Sleep(1000);
                        }
                        break;
                }
            }
            await Task.CompletedTask;
        }

        public async Task ShowLastProjectMenu(User currentUser)
        {
            Console.Clear();
            ConsoleHelpers.Info("Last selected Project");

            var last = _projectService.GetLastSelected(currentUser);

            if (last == null)
            {
                AnsiConsole.MarkupLine("[grey]You have no last selected Project yet.[/]");
                AnsiConsole.MarkupLine("[grey]Open a Project from \"Show Projects\" first.[/]");
                Console.ReadKey(true);
                await Task.CompletedTask;
                return;
            }

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{last.title}[/]");
            AnsiConsole.MarkupLine($"[grey]Description:[/] {last.description}");
            AnsiConsole.MarkupLine($"[grey]Created:[/] {last.DateOfCreation:yyyy-MM-dd}");
            AnsiConsole.MarkupLine("");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#FFA500]What do you want to do with this Project?[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Open Project", "Back"));

            if (choice == "Open Project")
            {
                Console.WriteLine("Going to project...");
                ConsoleHelpers.DelayAndClear();
                await ProjectEditMenu(last);
            }
            else
            {
                return;
            }
        }
    }
}
