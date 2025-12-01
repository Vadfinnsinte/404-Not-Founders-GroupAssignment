using _404_not_founders.Models;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.UI.CRUD
{
    /// Helper class for displaying a full overview of a Project.
    /// Can show the project itself, all storylines, worlds and characters.
    public class ShowEverything
    {
        private readonly Project _project;

        /// Stores a reference to the project that will be displayed.
        public ShowEverything(Project project)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
        }

        /// Shows the project, all storylines, all worlds and all characters in one flow.
        /// Waits for a key press at the end before clearing.
        public void ShowAll()
        {
            // Project header + core info
            ShowSection("Project", () => _project.Show());

            // All storylines
            ShowAllStorylines();

            // All worlds
            ShowAllWorlds();

            // All characters
            ShowAllCharacters();

            AnsiClearHelper.WaitForKeyAndClear();
        }

        /// Utility method to show a big header and then run the provided action.
        /// Keeps header formatting consistent across all sections.
        private void ShowSection(string header, Action action)
        {
            BigHeader.Show(header);
            action();
        }

        /// Shows all storylines in the project, or a "none found" message.
        public void ShowAllStorylines()
        {
            ShowSection("Storylines", () =>
            {
                if (_project.Storylines != null && _project.Storylines.Any())
                {
                    foreach (var story in _project.Storylines)
                        story.Show();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]No storylines found.[/]");
                }
            });
        }

        /// Shows all worlds in the project, or a "none found" message.
        public void ShowAllWorlds()
        {
            ShowSection("Worlds", () =>
            {
                if (_project.Worlds != null && _project.Worlds.Any())
                {
                    foreach (var world in _project.Worlds)
                        world.Show();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]No worlds found.[/]");
                }
            });
        }

        /// Shows all characters in the project, or a "none found" message.
        public void ShowAllCharacters()
        {
            ShowSection("Characters", () =>
            {
                if (_project.Characters != null && _project.Characters.Any())
                {
                    foreach (var character in _project.Characters)
                        character.Show();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]No characters found.[/]");
                }
            });
        }
    }
}
