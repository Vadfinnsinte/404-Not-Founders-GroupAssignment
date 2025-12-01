using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    public class WorldChoisesMenu
    {

        private readonly UserService _userService;

        // Constructor with dependency injection
        public WorldChoisesMenu(UserService userService)
        {

            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // ----- SELECT WORLD -----
        private World? SelectWorld(Project project, string title)
        {
            // Handle case where there are no worlds
            if (project.Worlds == null || project.Worlds.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No worlds yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            // Sort worlds alphabetically and prompt user to select one
            var sorted = project.Worlds.ToList();

            return AnsiConsole.Prompt(
                new SelectionPrompt<World>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(sorted)
                    .UseConverter(w => w.Name));
        }





        // ----- WORLD MENU -----

        public void WorldMenu(User loggedInUser, Project currentProject)
        {
            bool runWorld = true;

            // Main loop for the World Menu
            while (runWorld)
            {
                Console.Clear();
                ConsoleHelpers.Info("World Menu");
                var choice = MenuChoises.Menu("",
                    "Add World",
                    "Show Worlds",
                    "Edit World",
                    "Remove World",
                    "Back");

                // Handle user choice for the World Menu and call appropriate methods
                switch (choice)
                {
                    case "Add World":
                        World newWorld = new World();
                        newWorld.Add(loggedInUser, currentProject, _userService);
                        break;

                    case "Show Worlds":
                        currentProject.ShowAllWorlds();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;
                    case "Edit World":
                        // Check if there are any worlds to edit
                        if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No worlds in this project yet.[/]");
                            Console.ReadKey(true);
                            break;
                        }
                        // Let the user select a world to edit
                        var worldToEdit = SelectWorld(currentProject, "Choose world to edit");
                        if (worldToEdit != null)
                            worldToEdit.EditWorld(_userService);
                        break;
                    case "Delete World":
                        // Check if there are any worlds to remove
                        if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No worlds to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }

                        // Create a list of world names for selection
                        var worldChoices = currentProject.Worlds.Select(w => w.Name).ToList();

                        // Add a "Back to Menu" option
                        worldChoices.Add("Back to Menu");

                        // Show the selection prompt
                        var selectedWorld = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose World to Remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(worldChoices));

                        // If the user selected "Back to Menu", go back to the previous menu
                        if (selectedWorld == "Back to Menu")
                        {
                            break;
                        }

                        // Find and delete the selected world
                        var worldToDelete = currentProject.Worlds.First(w => w.Name == selectedWorld);
                        worldToDelete.DeleteWorld(currentProject, _userService);
                        break;

                    case "Back":
                        runWorld = false;
                        break;
                }
            }
        }
    }
}

