using _404_not_founders.UI;             // ConsoleHelpers, AnsiClearHelper, ShowInfoCard
using _404_not_founders.Models;         // User, Project, Character, World, Storyline
using _404_not_founders.Services;       // UserService, ProjectService, DungeonMasterAI
using Spectre.Console;                  // Meny, markeringar och prompts
using Microsoft.Extensions.Configuration; // För API-nyckel och config
using System;                           // Console, Thread.Sleep etc
using System.Threading.Tasks;           // async/await
using System.Collections.Generic;       // IReadOnlyList
using System.Linq;                      // LINQ

namespace _404_not_founders.Menus
{
    public class ProjectChoisesMenu
    {
        private readonly User _currentUser;
        private readonly Project _currentProject;
        private readonly UserService _userService;
        private readonly ProjectService _projectService;

        public ProjectChoisesMenu(User currentUser, Project currentProject, UserService userService, ProjectService projectService)
        {
            _currentUser = currentUser;
            _currentProject = currentProject;
            _userService = userService;
            _projectService = projectService;
        }


        // --- FIX: async Task ---
        public async Task ShowProjectMenu()
        {
            if (_currentUser == null) // Safety check
            {
                AnsiConsole.MarkupLine("[red]You must be logged in to view projects.[/]");
                await Task.CompletedTask;
                return;
            }
            bool runProjectMenu = true;
            while (runProjectMenu) // Loop för Project Menu
            {
                Console.Clear();
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
                    var list = _projectService.GetAll(_currentUser); // Hämta alla projekt för användaren

                    if (list == null || list.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No projects yet.[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    var selected = SelectFromList(list, "Select a project");
                    if (selected == null)
                        continue;

                    _projectService.SetLastSelected(_currentUser, selected.Id); // Spara som senaste valda projekt
                    await ProjectEditMenu(selected, _userService); // Gå till redigeringsmenyn för valt projekt
                    // INGEN runProjectMenu = false här – så menyn loopar kvar
                }
                else if (choice == "Search projects")
                {
                    var term = AnsiConsole.Ask<string>("Searchterm (title/description):").Trim(); // Be om sökterm
                    var hits = _projectService.Search(_currentUser, term); // Sök projekt

                    if (hits == null || hits.Count == 0) // Inga träffar
                    {
                        AnsiConsole.MarkupLine("[red]No results[/]");
                        AnsiClearHelper.WaitForKeyAndClear();
                        continue;
                    }

                    var selected = SelectFromList(hits, $"Select from search results for \"{term}\"");
                    if (selected == null) // Användaren valde "Back"
                        continue;

                    _projectService.SetLastSelected(_currentUser, selected.Id); // Spara som senaste valda projekt
                    AnsiConsole.Clear();
                    await ProjectEditMenu(selected, _userService);
                    // INGEN runProjectMenu = false här heller!
                }
            }
            await Task.CompletedTask;
        }

        private Project? SelectFromList(IReadOnlyList<Project> projects, string title) // Hjälpmetod för att välja projekt från lista
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

        // --- FIX: async Task ---
        public async Task ProjectEditMenu(Project project, UserService userService) // Meny för att redigera valt projekt
        {
            bool runEdit = true;

            while (runEdit)
            {
                Console.Clear(); // <-- Rensa alltid inför varje menyval!

                var choice = ProjectEditVisuals.ShowEditMenu(project); // Visa menyn och få användarens val

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
                        {
                            if (_currentUser != null)
                            {
                                var worldChoisesMenu = new WorldChoisesMenu(_userService); // Skapa instans av WorldChoisesMenu
                                await worldChoisesMenu.WorldMenu(_currentUser, project, _userService); // Gå till världsmenyn för valt projekt
                                // Efter undermeny: tillbaks till ProjectEditMenu, som nu är ren!
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("[red]No User logged in![/]");
                                ConsoleHelpers.DelayAndClear();
                            }
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
                        {
                            Console.Clear(); // Rensa inför visning!
                            project.ShowAll(project);
                            // Efter visning: tillbaks till ProjectEditMenu, ren!
                        }
                        break;

                    case "Back to main menu":
                        Console.Clear(); // Rensa innan tillbaks
                        runEdit = false; // Avsluta loopen för ProjectEditMenu
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

            var last = _projectService.GetLastSelected(currentUser); // Hämta senaste valda projekt

            if (last == null)
            {
                AnsiConsole.MarkupLine("[grey]You have no last selected Project yet.[/]");
                AnsiConsole.MarkupLine("[grey]Open a Project from \"Show Projects\" first.[/]");
                Console.ReadKey(true);
                await Task.CompletedTask; // FIX: await Task.CompletedTask
                return;
            }

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{last.title}[/]");
            AnsiConsole.MarkupLine($"[grey]Description:[/] {last.description}");
            AnsiConsole.MarkupLine($"[grey]Created:[/] {last.DateOfCreation:yyyy-MM-dd}");
            AnsiConsole.MarkupLine("");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>() // Prompt för val
                    .Title("[#FFA500]What do you want to do with this Project?[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Open Project", "Back"));

            if (choice == "Open Project")
            {
                Console.WriteLine("Going to Project...");
                ConsoleHelpers.DelayAndClear();
                await ProjectEditMenu(last, _userService); // Gå till redigeringsmenyn för senaste projektet
            }
            else
            {
                await Task.CompletedTask; // FIX: await Task.CompletedTask
                return;
            }
        }
    }
}
