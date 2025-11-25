using _404_not_founders.UI;             // ConsoleHelpers, AnsiClearHelper, ShowInfoCard
using _404_not_founders.Models;         // User, Project, Character, World, Storyline
using _404_not_founders.Services;       // UserService, ProjectService, DungeonMasterAI
using Spectre.Console;                  // Meny, markeringar och prompts
using Microsoft.Extensions.Configuration; // För API-nyckel och config
using System;                           // Console, Thread.Sleep etc
using System.Threading.Tasks;           // async/await

namespace _404_not_founders.Menus
{
    public class WorldChoisesMenu
    {
        private readonly UserService _userService;

        public WorldChoisesMenu(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // FIX: async Task
        public async Task WorldMenu(User loggedInUser, Project currentProject, UserService userService)
        {
            bool runWorldMenu = true;
            while (runWorldMenu)
            {
                Console.Clear();
                ConsoleHelpers.Info("World Menu");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]World Menu[/]")
                        .AddChoices(
                            "Generate World with AI",
                            "Add World",
                            "Show Worlds",
                            "Edit World",
                            "Delete World",
                            "Back")
                        .HighlightStyle(Color.Orange1)
                );

                switch (choice)
                {
                    case "Generate World with AI":
                        var worldForAi = new World();
                        await worldForAi.GenerateWorldWithGeminiAI(currentProject, userService);
                        break;

                    case "Add World":
                        var newWorld = new World();
                        newWorld.Add(loggedInUser, currentProject, userService);
                        break;

                    case "Show Worlds":
                        currentProject.ShowAllWorlds();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;

                    case "Edit World":
                        if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Worlds in this Project yet.[/]");
                            Console.ReadKey(true);
                            break;
                        }
                        var worldToEdit = SelectWorld(currentProject, "Choose World to edit");
                        if (worldToEdit != null)
                            worldToEdit.EditWorld(userService);
                        break;

                    case "Delete World":
                        if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Worlds to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }
                        var worldChoices = currentProject.Worlds.Select(w => w.Name).ToList();
                        worldChoices.Add("Back to Menu");
                        var selectedWorld = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose World to Remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(worldChoices));
                        if (selectedWorld == "Back to Menu")
                            break;
                        var worldToDelete = currentProject.Worlds.FirstOrDefault(w => w.Name == selectedWorld);
                        if (worldToDelete != null)
                            worldToDelete.DeleteWorld(currentProject, userService);
                        break;

                    case "Back":
                        runWorldMenu = false;
                        break;
                }
            }
            await Task.CompletedTask;
        }


        private World? SelectWorld(Project project, string title)
        {
            if (project.Worlds == null || project.Worlds.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No Worlds yet.[/]");
                Console.ReadKey(true);
                return null;
            }
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