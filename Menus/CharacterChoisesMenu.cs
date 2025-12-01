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
    public class CharacterChoisesMenu
    {
        private readonly User _currentUser;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;

        // Constructor to initialize services and current user
        public CharacterChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        // Character Menu handling character-related operations
        public void ChracterMenu(Project currentProject)
        {
            Character newCharacter = new Character();
            User? currentUser = _currentUser;
            bool runCharacterMenu = true;
            // Loop to display the character menu until the user chooses to go back
            while (runCharacterMenu)
            {
                var choice = MenuChoises.Menu("Character Menu", "Add Character", "Show Character", "Edit Character", "Delete Character", "Back");

                // Switch case to handle user choices
                switch (choice)
                {
                    case "Add Character":
                        // Call method to add a new character
                        newCharacter.Add(currentUser, _projectService, _userService);

                        break;
                    case "Show Character":
                        // Show characters from the actual project
                        newCharacter.ShowCharacters(currentProject);

                        break;
                    case "Edit Character":
                        // Check if there are characters to edit
                        if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No characters in this project yet.[/]");
                            Console.ReadKey(true);
                            break;
                        }
                        // Call method to edit a character
                        newCharacter.EditCharacter(currentProject, _userService);

                        break;
                    case "Delete Character":
                        // Check if there are characters to remove
                        if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Characters to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }
                        // create a list of character names for selection
                        var characterChoices = currentProject.Characters.Select(w => w.Name).ToList();

                        characterChoices.Add("Back"); // Add back option to the list

                        // Show selection prompt to choose character to remove
                        var selectedCharacter = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose character to remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(characterChoices));

                        // Goes back to the previous menu if "Back" is selected
                        if (selectedCharacter == "Back")
                        {
                            break;
                        }

                        // Find selected character and call delete method
                        var characterToDelete = currentProject.Characters.First(w => w.Name == selectedCharacter);
                        characterToDelete.DeleteCharacter(currentProject, _userService);

                        break;
                    case "Back":
                        // Sets the boolean to false to exit the loop and go back to the previous menu
                        runCharacterMenu = false;

                        break;
                }
            }
        }


    }
}