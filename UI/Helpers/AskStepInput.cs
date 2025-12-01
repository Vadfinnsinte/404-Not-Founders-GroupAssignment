using Spectre.Console;

namespace _404_not_founders.UI.Helpers
{
    // Asks the user for input with optional Back/Exit handling and custom validation.
    public class AskStepInput
    {
        public static string AskStepInputs(
        string prompt,
        bool allowBack = true,
        bool allowExit = true,
        Func<string, bool>? validator = null,
        string? validationMessage = null)
        {
            while (true)
            {
                string input = AnsiConsole.Ask<string>($"[#FFA500]{prompt}[/]");


                if (allowBack && input.Equals("B", StringComparison.OrdinalIgnoreCase))
                    return "B";

                if (allowExit && input.Equals("E", StringComparison.OrdinalIgnoreCase))
                    return "E";

                if (validator != null && !validator(input))
                {
                    AnsiConsole.MarkupLine($"[red]{validationMessage ?? "Invalid input"}[/]");
                    continue;
                }

                return input;
            }
        }
    }
}
