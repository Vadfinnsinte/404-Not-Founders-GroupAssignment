using Spectre.Console;

namespace _404_not_founders.UI.Display
{
    public class MenuChoises
    {
        private const string MainTitleColor = "#FFA500";
        /// Menu with Orange highlight (active choice) and White for normal choices (inactive).
        public static string Menu(string title, params string[] choices) =>
             AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#{MainTitleColor}]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(choices)
                    .UseConverter(choice => $"[white]{choice}[/]")
            );

    }
}
