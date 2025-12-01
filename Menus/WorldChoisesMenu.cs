using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    /// Menu for managing Worlds inside a selected Project.
    /// Handles AI generation, manual add, show, edit and delete operations.
    public class WorldChoisesMenu
    {
        private readonly UserService _userService;

        /// Constructor with dependency injection for UserService.
        public WorldChoisesMenu(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// Main world menu loop for a specific project and logged-in user.
        /// Lets the user generate, add, show, edit and delete worlds.
        public async Task WorldMenu(User loggedInUser, Project currentProject)
        {
            bool runWorld = true;

            while (runWorld)
            {
                Console.Clear();
                ConsoleHelpers.Info("World Menu");

                var choice = MenuChoises.Menu("",
                    "Generate World with AI",
                    "Add World",
                    "Show Worlds",
                    "Edit World",
                    "Delete World",
                    "Back");

                switch (choice)
                {
                    case "Generate World with AI":
                        {
                            // Generate a new World using Gemini AI for this project
                            var worldForAi = new World();
                            await worldForAi.GenerateWorldWithGeminiAI(currentProject, _userService);
                            break;
                        }

                    case "Add World":
                        {
                            // Manual, step-by-step world creation
                            var newWorld = new World();
                            newWorld.Add(loggedInUser, currentProject, _userService);
                            break;
                        }

                    case "Show Worlds":
                        {
                            // Show all worlds for the current project
                            var show = new ShowEverything(currentProject);
                            show.ShowAllWorlds();
                            AnsiClearHelper.WaitForKeyAndClear();
                            break;
                        }

                    case "Edit World":
                        {
                            // Guard: no worlds to edit
                            if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                            {
                                AnsiConsole.MarkupLine("[grey]No Worlds in this Project yet.[/]");
                                Console.ReadKey(true);
                                break;
                            }

                            // Let user pick a world and open its edit menu
                            var worldToEdit = SelectWorld(currentProject, "Choose world to edit");
                            if (worldToEdit != null)
                                worldToEdit.EditWorld(_userService);
                            break;
                        }

                    case "Delete World":
                        {
                            // Guard: no worlds to delete
                            if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                            {
                                AnsiConsole.MarkupLine("[grey]No Worlds to remove.[/]");
                                ConsoleHelpers.DelayAndClear();
                                break;
                            }

                            // Build list of world names for selection
                            var worldChoices = currentProject.Worlds
                                .Select(w => w.Name)
                                .ToList();

                            worldChoices.Add("Back to Menu");

                            var selectedWorld = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[#FFA500]Choose World to Remove[/]")
                                    .HighlightStyle(new Style(Color.Orange1))
                                    .AddChoices(worldChoices));

                            // User chose to go back
                            if (selectedWorld == "Back to Menu")
                                break;

                            // Find and delete the selected world
                            var worldToDelete = currentProject.Worlds.First(w => w.Name == selectedWorld);
                            worldToDelete.DeleteWorld(currentProject, _userService);
                            break;
                        }

                    case "Back":
                        // Exit world menu
                        runWorld = false;
                        break;
                }
            }
        }

        /// Lets the user select a world from the project.
        /// Returns null if there are no worlds.
        private World? SelectWorld(Project project, string title)
        {
            if (project.Worlds == null || project.Worlds.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No Worlds yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            // Can be extended later if you want ordering (e.g. by name)
            var sorted = project.Worlds.ToList();

            return AnsiConsole.Prompt(
                new SelectionPrompt<World>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(sorted)
                    .UseConverter(w => w.Name));
        }
    }
}
