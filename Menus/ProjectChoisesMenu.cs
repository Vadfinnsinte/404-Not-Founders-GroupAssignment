using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.Menus
{
    public class ProjectChoisesMenu
    {
        private readonly User _currentUser;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;

        // Constructor with dependency injection
        public ProjectChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        public void ShowProjectMenu()
        {
            // Ensure user is logged in
            if (_currentUser == null)
            {
                AnsiConsole.MarkupLine("[red]You must be logged in to view projects.[/]");
                return;
            }
            // Main project menu loop
            bool runProjectMenu = true;
            while (runProjectMenu)
            {
                // Display project menu options
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Project Menu[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices("Show all projects", "Search projects", "Back"));

                // Sends user back to main menu
                if (choice == "Back")
                {
                    runProjectMenu = false;
                    break;
                }

                // Shows a list of all projects
                if (choice == "Show all projects")
                {
                    var list = _projectService.GetAll(_currentUser);

                    // Handle empty project list
                    if (list == null || list.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No projects yet.[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    // ---- SELECT with BACK ----
                    var selected = SelectFromList(list, "Select a project");
                    if (selected == null)
                        continue; // Back pressed

                    // Open the selected project
                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    runProjectMenu = false;
                    ProjectEditMenu(selected);
                }

                // Search for projects by title or description
                else if (choice == "Search projects")
                {
                    var term = AnsiConsole.Ask<string>("Searchterm (title/description):").Trim();
                    var hits = _projectService.Search(_currentUser, term);

                    // Handles no search results
                    if (hits == null || hits.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No results[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    // ---- SELECT with BACK ----
                    var selected = SelectFromList(hits, $"Select from search results for \"{term}\"");
                    if (selected == null)
                        continue; // Back pressed

                    // Open the selected project
                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    AnsiConsole.Clear();
                    runProjectMenu = false;
                    ProjectEditMenu(selected);
                }

            }
        }

        // Method to select a project from a list
        private Project? SelectFromList(IReadOnlyList<Project> projects, string title)
        {
            if (projects == null || projects.Count == 0)
                return null;

            var sorted = projects.OrderByDescending(p => p.DateOfCreation).ToList();

            // --- Create display list ---
            var displayList = sorted.Select(p => p.title).ToList();
            displayList.Add("Back");

            var selectedTitle = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold]{title}[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(displayList));

            // If user selects “Back”
            if (selectedTitle == "Back")
            {
                AnsiConsole.Clear();
                return null;
            }

            // Return the matching project
            var selectedProject = sorted.First(p => p.title == selectedTitle);
            AnsiConsole.Clear();
            return selectedProject;
        }

        public void ProjectEditMenu(Project project)
        {
            Character character = new Character();
            bool running = true, loggedIn = true;
            bool runEdit = true;
            string user = _currentUser?.Username ?? ""; // Add this if needed for ShowLoggedInMenu

            // Main edit menu loop
            while (runEdit)
            {
                var choice = ProjectEditVisuals.ShowEditMenu(project);

                // Handle user choices and navigate to appropriate menus
                switch (choice)
                {
                    case "Edit/Add Characters":
                        CharacterChoisesMenu characterChoisesMenu = new CharacterChoisesMenu(_currentUser, _projectService, _userService);
                        characterChoisesMenu.ChracterMenu(project);
                        break;
                    case "Edit/Add worlds":
                        // Ensure user is logged in and open world menu
                        if (_currentUser != null)
                        {
                            WorldChoisesMenu worldChoisesMenu = new WorldChoisesMenu(_userService);
                            worldChoisesMenu.WorldMenu(_currentUser, project);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]No user logged in![/]");
                            ConsoleHelpers.DelayAndClear();
                        }
                        break;
                    case "Edit/Add Storylines":
                        StorylineChoisesMenu storylineMenu = new StorylineChoisesMenu(_userService);
                        storylineMenu.StorylineMenu(project);
                        break;
                    case "Show Everything":
                        Console.Clear();
                        project.ShowAll();
                        break;
                    case "Back to main menu":
                        Console.Clear();
                        runEdit = false;
                        break;
                    default:
                        Console.WriteLine("Something went wrong... going back to menu");
                        return;
                }
            }
            //DelayAndClear();
        }
        public void ShowLastProjectMenu(User currentUser)
        {
            Console.Clear();
            ConsoleHelpers.Info("Last selected project");

            // Get last selected project
            var last = _projectService.GetLastSelected(currentUser);

            // Handle no last selected project
            if (last == null)
            {
                AnsiConsole.MarkupLine("[grey]You have no last selected project yet.[/]");
                AnsiConsole.MarkupLine("[grey]Open a project from \"Show projects\" first.[/]");
                Console.ReadKey(true);
                return;
            }

            // Show last selected project details
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{last.title}[/]");
            AnsiConsole.MarkupLine($"[grey]Description:[/] {last.description}");
            AnsiConsole.MarkupLine($"[grey]Created:[/] {last.DateOfCreation:yyyy-MM-dd}");
            AnsiConsole.MarkupLine("");

            // Prompt user for action on last selected project
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#FFA500]What do you want to do with this project?[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Open project", "Back"));

            if (choice == "Open project")
            {
                // Open the last selected projects edit menu
                Console.WriteLine("Going to project...");
                ConsoleHelpers.DelayAndClear();
                ProjectEditMenu(last);
            }
            else
            {
                // Go back to previous menu
                return;
            }
        }
    }
}
