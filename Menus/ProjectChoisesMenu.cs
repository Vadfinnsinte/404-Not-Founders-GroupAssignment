using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    /// Menu for listing, searching and editing Projects for the current user.
    public class ProjectChoisesMenu
    {
        private readonly User _currentUser;        // The currently logged-in user
        private readonly UserService _userService; // Service for user-related operations
        private readonly ProjectService _projectService; // Service for project-related operations

        /// Constructor with dependency injection for user, project and user services.
        public ProjectChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        /// Shows the main project menu (Show all, Search, Back) for the current user.
        /// Lets the user select a project and then opens the project edit menu.
        public async Task ShowProjectMenu()
        {
            if (_currentUser == null)
            {
                AnsiConsole.MarkupLine("[red]You must be logged in to view projects.[/]");
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
                    // Get all projects for the current user
                    var list = _projectService.GetAll(_currentUser);

                    if (list == null || list.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No projects yet.[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    // Let user choose a project from the full list
                    var selected = SelectFromList(list, "Select a project");
                    if (selected == null)
                        continue;

                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    await ProjectEditMenu(selected);
                }
                else if (choice == "Search projects")
                {
                    // Ask for search term (matches title or description)
                    var term = AnsiConsole.Ask<string>("Searchterm (title/description):").Trim();
                    if (string.IsNullOrWhiteSpace(term))
                    {
                        AnsiConsole.MarkupLine("[grey]Search term was empty.[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    // Search in current user's projects
                    var hits = _projectService.Search(_currentUser, term);

                    if (hits == null || hits.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No results[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    // Let user choose one of the search hits
                    var selected = SelectFromList(hits, $"Select from search results for \"{term}\"");
                    if (selected == null)
                        continue;

                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    AnsiConsole.Clear();
                    await ProjectEditMenu(selected);
                }
            }
        }

        /// Helper prompt to select a Project from a given list.
        /// Returns null if user selects Back or list is empty.
        private Project? SelectFromList(IReadOnlyList<Project> projects, string title)
        {
            if (projects == null || projects.Count == 0)
                return null;

            // Show newest projects first
            var sorted = projects.OrderByDescending(p => p.DateOfCreation).ToList();

            // We display titles, and add a "Back" entry
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

            // Find the project that matches the selected title
            var selectedProject = sorted.First(p => p.title == selectedTitle);
            AnsiConsole.Clear();
            return selectedProject;
        }

        /// Edit menu for a specific Project.
        /// From here the user can manage characters, worlds, storylines or show everything.
        public async Task ProjectEditMenu(Project project)
        {
            bool runEdit = true;

            while (runEdit)
            {
                Console.Clear();

                // Show edit menu UI and get user choice
                var choice = ProjectEditVisuals.ShowEditMenu(project);

                switch (choice)
                {
                    case "Edit/Add Characters":
                        {
                            // Navigate to character choices menu
                            var characterChoisesMenu = new CharacterChoisesMenu(_currentUser, _projectService, _userService);
                            await characterChoisesMenu.ChracterMenu(project, _userService);
                            break;
                        }

                    case "Edit/Add worlds":
                        if (_currentUser != null)
                        {
                            // Navigate to world choices menu
                            var worldChoisesMenu = new WorldChoisesMenu(_userService);
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
                            // Navigate to storyline choices menu
                            var storylineMenu = new StorylineChoisesMenu(_userService);
                            await storylineMenu.StorylineMenu(project, _userService);
                            break;
                        }

                    case "Show Everything":
                        // Show all details of the selected project
                        Console.Clear();
                        var show = new ShowEverything(project);
                        show.ShowAll();
                        break;

                    case "Back to main menu":
                        // Exit project edit menu
                        Console.Clear();
                        runEdit = false;
                        break;

                    default:
                        {
                            Console.WriteLine("Something went wrong... going back to menu");
                            Thread.Sleep(1000);
                            break;
                        }
                }
            }
        }

        /// Shows the last selected Project for the given user and offers to open it.
        public async Task ShowLastProjectMenu(User currentUser)
        {
            Console.Clear();
            ConsoleHelpers.Info("Last selected Project");

            // Retrieve the last selected project for this user
            var last = _projectService.GetLastSelected(currentUser);

            if (last == null)
            {
                AnsiConsole.MarkupLine("[grey]You have no last selected Project yet.[/]");
                AnsiConsole.MarkupLine("[grey]Open a Project from \"Show Projects\" first.[/]");
                Console.ReadKey(true);
                return;
            }

            // Basic summary of the last selected project
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{Markup.Escape(last.title)}[/]");
            AnsiConsole.MarkupLine($"[grey]Description:[/] {Markup.Escape(last.description)}");
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
