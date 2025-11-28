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

        public async Task ChracterMenu(Project currentProject, UserService userService)
        {
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
                            break;
                        }

                    case "Add Character":
                        {
                            var newChar = new Character();
                            // Skapar och sparar karaktären (Add sköter saved-meddelande)
                            newChar.Add(_currentUser, currentProject, _userService);
                            break;
                        }
                    case "Show Character":
                        {
                            ShowCharacterSelection(currentProject);
                            break;
                        }

                    case "Edit Character":
                        {
                            if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                            {
                                AnsiConsole.MarkupLine("[grey]No Characters in this project yet.[/]");
                                Console.ReadKey(true);
                                break;
                            }

                            var character = SelectCharacter(currentProject, "Select Character to edit");
                            if (character != null)
                            {
                                character.EditCharacter(_userService); // bara UserService, 1 argument
                            }
                            break;
                        }

                    case "Delete Character":
                        {
                            if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                            {
                                AnsiConsole.MarkupLine("[grey]No Characters to remove.[/]");
                                ConsoleHelpers.DelayAndClear();
                                break;
                            }

                            var character = SelectCharacter(currentProject, "Choose Character to remove");
                            if (character != null)
                            {
                                character.DeleteCharacter(currentProject, _userService);
                            }
                            break;
                        }

                    case "Back":
                        runCharacterMenu = false;
                        break;
                }
            }

            await Task.CompletedTask;
        }

        private Character? SelectCharacter(Project currentProject, string title)
        {
            var characterChoices = currentProject.Characters.Select(c => c.Name).ToList();
            characterChoices.Add("Back");

            var selectedCharacter = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(characterChoices));

            if (selectedCharacter == "Back")
                return null;

            return currentProject.Characters.First(c => c.Name == selectedCharacter);
        }

        private void ShowCharacterSelection(Project currentProject)
        {
            if (currentProject.Characters == null || currentProject.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No Characters in this project yet.[/]");
                Console.ReadKey(true);
                return;
            }

            var character = SelectCharacter(currentProject, "Select Character to show");
            if (character == null)
                return;

            Console.Clear();
            character.Show(); // din Character.Show()
            Console.WriteLine();
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}
