using _404_not_founders.Models;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;


namespace _404_not_founders.UI.CRUD
{
    public class ShowEverything
    {
        private readonly Project _project;

        public ShowEverything(Project project)
        {
            _project = project;
        }
        public void ShowAll()
        {

            ShowSection("Project", () => _project.Show());

            ShowAllStorylines();

            ShowAllWorlds();

            ShowAllCharacters();

            AnsiClearHelper.WaitForKeyAndClear();

            return;
        }

        // Shows the section header using BigHeader, then runs the provided action for that section.
        private void ShowSection(string header, Action action) 
        {
            BigHeader.Show(header);
            action();
        }
        // Shows all storylines in the project, or a message if none are found.
        public void ShowAllStorylines()
        {
            ShowSection("Storylines", () =>
            {
                if (_project.Storylines != null && _project.Storylines.Any())
                    foreach (var story in _project.Storylines)
                        story.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No storylines found.[/]");
            });
        }
        public void ShowAllWorlds()
        {
            ShowSection("Worlds", () =>
            {
                if (_project.Worlds != null && _project.Worlds.Any())
                    foreach (var world in _project.Worlds)
                        world.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No worlds found.[/]");
            });
        }
        public void ShowAllCharacters()
        {
            ShowSection("Characters", () =>
            {
                if (_project.Characters != null && _project.Characters.Any())
                    foreach (var character in _project.Characters)
                        character.Show();
                else
                    AnsiConsole.MarkupLine("[grey]No characters found.[/]");
            });
        }
    }
}
