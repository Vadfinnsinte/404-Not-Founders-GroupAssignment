using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI;
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

        public ProjectChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        public void ShowProjectMenu()
        {
            //             Info("Projektmeny");
            //             DelayAndClear();
            if (_currentUser == null)
            {
                AnsiConsole.MarkupLine("[red]You must be logged in to view projects.[/]");
                Console.WriteLine(_currentUser);
            }// 

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Project Menu[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices("Show all projects", "Search projects", "Back"));

                if (choice == "Back") break;

                if (choice == "Show all projects")
                {
                    var list = _projectService.GetAll(_currentUser);
                    if (list == null || list.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No projects yet.[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;

                    }

                    var selected = SelectFromList(list, "Select a project");
                    if (selected != null)
                        _projectService.SetLastSelected(_currentUser, selected.Id);
                    ProjectEditMenu(selected);
                }
                else if (choice == "Search projects")
                {
                    var term = AnsiConsole.Ask<string>("Searchterm (title/description):").Trim();
                    var hits = _projectService.Search(_currentUser, term);

                    if (hits == null || hits.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No results[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;
                    }

                    var selected = SelectFromList(hits, $"Select from search results for \"{term}\"");
                    if (selected != null)
                        _projectService.SetLastSelected(_currentUser, selected.Id);

                    AnsiConsole.Clear();
                    ProjectEditMenu(selected);
                }

            }

        }
        private Project? SelectFromList(IReadOnlyList<Project> projects, string title)
        {
            if (projects == null || projects.Count == 0) return null;

            var sorted = projects.OrderByDescending(p => p.DateOfCreation).ToList();

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<Project>()
                    .Title($"[bold]{title}[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(sorted)
                    .UseConverter(p => $"{p.title} ({p.DateOfCreation:yyyy-MM-dd})"));

            AnsiConsole.Clear();
            return selected;

        }
        public void ProjectEditMenu(Project project)
        {
            Character character = new Character();
            bool running = true, loggedIn = true;
            bool runEdit = true;
            string user = _currentUser?.Username ?? ""; // Add this if needed for ShowLoggedInMenu

            while (runEdit)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Project Edit Menu[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices(
                            "Edit/Add Characters",
                            "Edit/Add worlds",
                            "Edit/Add Storylines",
                            "Show Everything",
                            "Back to main menu"
                        )
                );

                switch (choice)
                {
                    case "Edit/Add Characters":
                        //character.ChracterMenu2(_userService, _projectService, this, project);
                        break;
                    case "Edit/Add worlds":
                        if (_currentUser != null)
                        {
                            //WorldMenu(_currentUser, project);
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

            // hämta senaste valda projektet för den här användaren
            var last = _projectService.GetLastSelected(currentUser);

            if (last == null)
            {
                AnsiConsole.MarkupLine("[grey]You have no last selected project yet.[/]");
                AnsiConsole.MarkupLine("[grey]Open a project from \"Show projects\" first.[/]");
                Console.ReadKey(true);
                return;
            }

            // Visa lite info om projektet
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{last.title}[/]");
            AnsiConsole.MarkupLine($"[grey]Description:[/] {last.description}");
            AnsiConsole.MarkupLine($"[grey]Created:[/] {last.DateOfCreation:yyyy-MM-dd}");
            AnsiConsole.MarkupLine("");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#FFA500]What do you want to do with this project?[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Open project", "Back"));

            if (choice == "Open project")
            {
                // gå direkt till samma meny som när man valt projekt via listan
                ProjectEditMenu(last);
            }
            else
            {
                // Back – bara gå tillbaka till huvudmenyn
                return;
            }
        }
    }
}
