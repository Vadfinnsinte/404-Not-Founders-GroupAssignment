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
        private readonly Project _currentProject;
        private readonly UserService _userService;
        private readonly ProjectService _projectService;

        // Constructor with dependency injection
        public ProjectChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser;
            _currentProject = currentProject;
            _userService = userService;
            _projectService = projectService;
        }


        // --- FIX: async Task ---
        public async Task ShowProjectMenu()
        {
            // Ensure user is logged in
            if (_currentUser == null)
            {
                AnsiConsole.MarkupLine("[red]You must be logged in to view projects.[/]");
                await Task.CompletedTask;
                return;
            }
            // Main project menu loop
            bool runProjectMenu = true;
            while (runProjectMenu) // Loop för Project Menu
            {
                // Display project menu options
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Project Menu[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices(
                            "Show all projects",
                            "Search projects",
                            "Back"));

                // Sends user back to main menu
                if (choice == "Back")
                {
                    runProjectMenu = false;
                    break;
                }

                // Shows a list of all projects
                if (choice == "Show all projects")
                {
                    var list = _projectService.GetAll(_currentUser); // Hämta alla projekt för användaren

                    // Handle empty project list
                    if (list == null || list.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No projects yet.[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    var selected = SelectFromList(list, "Select a project");
                    if (selected == null)
                        continue;

                    // Open the selected project
                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    runProjectMenu = false;
                    ProjectEditMenu(selected);
                }

                // Search for projects by title or description
                else if (choice == "Search projects")
                {
                    var term = AnsiConsole.Ask<string>("Searchterm (title/description):").Trim(); // Be om sökterm
                    var hits = _projectService.Search(_currentUser, term); // Sök projekt

                    // Handles no search results
                    if (hits == null || hits.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No results[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    var selected = SelectFromList(hits, $"Select from search results for \"{term}\"");
                    if (selected == null) // Användaren valde "Back"
                        continue;

                    // Open the selected project
                    _projectService.SetLastSelected(_currentUser, selected.Id);
                    AnsiConsole.Clear();
                    await ProjectEditMenu(selected, _userService);
                    // INGEN runProjectMenu = false här heller!
                }
            }
            await Task.CompletedTask;
        }

        // Method to select a project from a list
        private Project? SelectFromList(IReadOnlyList<Project> projects, string title)
        {
            if (projects == null || projects.Count == 0)
                return null;

            var sorted = projects.OrderByDescending(p => p.DateOfCreation).ToList(); // Sortera efter skapelsedatum, nyast först
            var displayList = sorted.Select(p => p.title).ToList(); // Skapa lista med titlar
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

            var selectedProject = sorted.First(p => p.title == selectedTitle); // Hitta valt projekt baserat på titel
            AnsiConsole.Clear();
            return selectedProject; // Returnera valt projekt
        }

        public void ProjectEditMenu(Project project)
        {
            bool runEdit = true;

            // Main edit menu loop
            while (runEdit)
            {
                Console.Clear(); // <-- Rensa alltid inför varje menyval!

                var choice = ProjectEditVisuals.ShowEditMenu(project); // Visa menyn och få användarens val

                // Handle user choices and navigate to appropriate menus
                switch (choice)
                {
                    case "Edit/Add Characters":
                        {
                            var characterChoisesMenu = new CharacterChoisesMenu(_currentUser, _projectService, _userService); // Skapa instans av CharacterChoisesMenu
                            await characterChoisesMenu.ChracterMenu(project, userService); // Gå till karaktärsmenyn för valt projekt
                            // Efter undermeny: tillbaks till ProjectEditMenu, som nu är ren!
                        }
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
                        {
                            var storylineMenu = new StorylineChoisesMenu(_userService); // Skapa instans av StorylineChoisesMenu
                            await storylineMenu.StorylineMenu(project, _userService); // Gå till berättelsemenyn för valt projekt
                            // Efter undermeny: tillbaks till ProjectEditMenu, som nu är ren!
                        }
                        break;

                    case "Show Everything":
                        Console.Clear();
                        ShowEverything show = new ShowEverything(project);
                        show.ShowAll();
                        break;

                    case "Back to main menu":
                        Console.Clear();
                        runEdit = false;
                        break;
                    default:
                        {
                            Console.WriteLine("Something went wrong... going back to menu");
                            // Här gör du inget return – du looper vidare, menyn blir ren i nästa varv!
                        }
                        break;
                }
            }
            await Task.CompletedTask; // Avsluta async metoden
        }


        // --- FIX: async Task ---
        public async Task ShowLastProjectMenu(User currentUser) // Meny för att visa senaste valda projekt
        {
            Console.Clear();
            ConsoleHelpers.Info("Last selected Project");

            // Get last selected project
            var last = _projectService.GetLastSelected(currentUser);

            // Handle no last selected project
            if (last == null)
            {
                AnsiConsole.MarkupLine("[grey]You have no last selected Project yet.[/]");
                AnsiConsole.MarkupLine("[grey]Open a Project from \"Show Projects\" first.[/]");
                Console.ReadKey(true);
                await Task.CompletedTask; // FIX: await Task.CompletedTask
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
                new SelectionPrompt<string>() // Prompt för val
                    .Title("[#FFA500]What do you want to do with this Project?[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Open Project", "Back"));

            if (choice == "Open Project")
            {
                // Open the last selected projects edit menu
                Console.WriteLine("Going to project...");
                ConsoleHelpers.DelayAndClear();
                await ProjectEditMenu(last, _userService); // Gå till redigeringsmenyn för senaste projektet
            }
            else
            {
                // Go back to previous menu
                return;
            }
        }
    }
}
