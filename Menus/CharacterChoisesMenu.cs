using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.CRUD;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Menus
{
    /// Menu for managing Characters inside a selected Project.
    /// Handles AI generation, manual add, show, edit and delete operations.
    public class CharacterChoisesMenu
    {
        private readonly User _currentUser;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;

        /// Constructor to initialize current user and required services.
        public CharacterChoisesMenu(User currentUser, ProjectService projectService, UserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// Main character menu loop for a specific project.
        /// Lets the user generate, add, show, edit and delete characters.
        public async Task ChracterMenu(Project currentProject, UserService userService)
        {
            Character newCharacter = new Character();
            User? currentUser = _currentUser;
            bool runCharacterMenu = true;

            // Loop to display the character menu until the user chooses to go back
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

                // Handle user choice
                switch (choice)
                {
                    case "Generate Character with AI":
                        {
                            // Call method to generate a character using AI
                            var aiCharacter = new Character();
                            await aiCharacter.GenerateCharacterWithGeminiAI(currentProject, _userService);
                            break;
                        }

                    case "Add Character":
                        // Manual, step-by-step character creation
                        newCharacter.Add(currentUser, currentProject, _userService);
                        break;

                    case "Show Character":
                        // Show all characters in the current project
                        var show = new ShowEverything(currentProject);
                        show.ShowAllCharacters();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;

                    case "Edit Character":
                        // Check if there are characters to edit
                        if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No characters in this project yet.[/]");
                            Console.ReadKey(true);
                            break;
                        }

                        // Let the user select a character to edit
                        var characterToEdit = SelectCharacter(currentProject, "Choose character to edit");
                        if (characterToEdit != null)
                        {
                            characterToEdit.EditCharacter(currentProject, _userService);
                        }
                        break;

                    case "Delete Character":
                        // Check if there are characters to remove
                        if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No Characters to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }

                        // Create a list of character names for selection
                        var characterChoices = currentProject.Characters
                            .Select(c => c.Name)
                            .ToList();

                        // Add back option to the list
                        characterChoices.Add("Back");

                        var selectedCharacter = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose character to remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(characterChoices));

                        // Go back to the menu if "Back" is selected
                        if (selectedCharacter == "Back")
                        {
                            break;
                        }

                        // Find selected character and call delete method
                        var characterToDelete = currentProject.Characters.First(c => c.Name == selectedCharacter);
                        characterToDelete.DeleteCharacter(currentProject, _userService);
                        break;

                    case "Back":
                        // Exit the loop and return to the previous menu
                        runCharacterMenu = false;
                        break;
                }
            }
        }

        /// Lets the user select a character from the project.
        /// Returns null if there are no characters or user cancels.
        private Character? SelectCharacter(Project project, string title)
        {
            // Check if there are characters available
            if (project.Characters == null || project.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No characters yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            // Prompt user to choose one character from the list
            return AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(project.Characters)
                    .UseConverter(c => $"{c.Name} ({c.Race})"));
        }
    }
}
