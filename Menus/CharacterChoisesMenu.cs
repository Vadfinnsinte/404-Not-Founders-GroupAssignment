using _404_not_founders.UI;             // ConsoleHelpers, AnsiClearHelper, ShowInfoCard
using _404_not_founders.Models;         // User, Project, Character, World, Storyline
using _404_not_founders.Services;       // UserService, ProjectService, DungeonMasterAI
using Spectre.Console;                  // Meny, markeringar och prompts
using Microsoft.Extensions.Configuration; // För API-nyckel och config
using System;                           // Console, Thread.Sleep etc
using System.Threading.Tasks;           // async/await
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _404_not_founders.Menus
{
    public class CharacterChoisesMenu
    {
        private readonly User _currentUser;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;

        public CharacterChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // FIX: async Task
        public async Task ChracterMenu(Project currentProject, UserService userService)
        {
            Character newCharacter = new Character();
            User? currentUser = _currentUser;
            bool runCharacterMenu = true;

            while (runCharacterMenu)
            {
                var choice = MenuChoises.Menu("Character Menu",
                    "Generate Character with AI",
                    "Add Character",
                    "Show Character",
                    "Edit Character",
                    "Delete Character",
                    "Back"
                );

                switch (choice)
                {
                    case "Generate Character with AI":
                        {
                            var newChar = new Character();
                            await newChar.GenerateCharacterWithGeminiAI(currentProject, userService);
                        }
                        break;
                    case "Add Character":
                        {
                            var newChar = new Character();
                            newChar.Add(_currentUser, _projectService, _userService);

                            ConsoleHelpers.Info("Character created!");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                            Console.Clear();

                            var postChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[#FFA500]What do you want to do next?[/]")
                                    .HighlightStyle(Color.Orange1)
                                    .AddChoices("Show Character", "Back"));

                            if (postChoice == "Show Character")
                                newChar.ShowCharacters(currentProject);
                            else if (postChoice == "Back")
                                break;
                            break;
                        }
                    case "Show Character":
                        newCharacter.ShowCharacters(currentProject);
                        break;
                    case "Edit Character":
                        if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Characters in this project yet.[/]");
                            Console.ReadKey(true);
                            break;
                        }
                        newCharacter.EditCharacter(currentProject, _userService);
                        break;
                    case "Delete Character":
                        if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Characters to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }
                        var characterChoices = currentProject.Characters.Select(w => w.Name).ToList();
                        characterChoices.Add("Back");

                        var selectedCharacter = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose Character to remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(characterChoices));

                        if (selectedCharacter == "Back")
                        {
                            break;
                        }

                        var characterToDelete = currentProject.Characters.First(w => w.Name == selectedCharacter);
                        characterToDelete.DeleteCharacter(currentProject, _userService);
                        break;
                    case "Back":
                        runCharacterMenu = false;
                        break;
                }
            }
            await Task.CompletedTask;
        }
    }
}