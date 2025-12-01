using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;


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
        public void ChracterMenu(Project currentProject)
        {
            Character newCharacter = new Character();
            User? currentUser = _currentUser;
            bool runCharacterMenu = true;
            while (runCharacterMenu)
            {
                var choice = MenuChoises.Menu("Character Menu", "Add Character", "Show Character", "Edit Character", "Delete Character", "Back");

                switch (choice)
                {
                    case "Add Character":

                        newCharacter.Add(currentUser, _projectService, _userService);

                        break;
                    case "Show Character":
                        // Show characters from the actual project
                        newCharacter.ShowCharacters(currentProject);

                        break;
                    case "Edit Character":

                        if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No characters in this project yet.[/]");
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
                                .Title("[#FFA500]Choose character to remove[/]")
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
        }


    }
}