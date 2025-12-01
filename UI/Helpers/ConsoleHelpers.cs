using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.Menus;
using Spectre.Console;

namespace _404_not_founders.UI.Helpers
{
    public class ConsoleHelpers
    {
        public const string MainTitleColor = "#FFA500";
        public static void Info(string text) => AnsiConsole.MarkupLine($"[underline {MainTitleColor}]{text}[/]");


        // Delay and screen clear – called after confirmation or error
        public static void DelayAndClear(int ms = 800) { Thread.Sleep(ms); Console.Clear(); }
        public static string ReadBackOrExit()
        {
            var input = Console.ReadLine();
            if (string.Equals(input, "E", StringComparison.OrdinalIgnoreCase))
                return "E";
            if (string.Equals(input, "B", StringComparison.OrdinalIgnoreCase))
                return "B";
            return input;
        }



        // Display instructions to the user about 'E' and 'B'
        public static void InputInstruction(bool back = false) =>
            AnsiConsole.MarkupLine(back
                ? "[grey italic]Press E to go back or B to return to the previous step[/]"
                : "[grey italic]Press E to go back[/]");


        // AskInput handles both secret and regular input, and always supports "E" for exit
        public static string AskInput(string prompt, bool secret = false)
        {
            var input = secret
                ? AnsiConsole.Prompt(new TextPrompt<string>(prompt).PromptStyle(MainTitleColor).Secret())
                : AnsiConsole.Ask<string>(prompt);
            if (input.Trim().Equals("E", StringComparison.OrdinalIgnoreCase)) return null;
            return input.Trim();
        }

        // displays the result with green/red text and an orange underline
        public static void Result(bool success, string text)
        {
            var color = success ? "green" : "red";
            AnsiConsole.MarkupLine($"[underline {MainTitleColor}][bold {color}]{text}[/][/]");
        }
    }
}
