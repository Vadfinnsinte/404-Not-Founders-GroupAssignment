using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace _404_not_founders.UI
{
    public static class AiHelper
    {
        private const string MainTitleColor = "#FFA500";

        /// <summary>
        /// Frågar användaren om valfri beskrivning för AI-generering.
        /// Tryck Enter = AI genererar själv, annars tas input med i prompt.
        /// </summary>
        public static string AskOptionalUserContext(string header)
        {
            Console.Clear();
            ConsoleHelpers.Info(header);
            AnsiConsole.MarkupLine("[grey]Write your own description, or press [bold]Enter[/] to let AI generate automatically.[/]");
            AnsiConsole.MarkupLine("[grey italic]Type 'E' to cancel and go back.[/]");
            Console.WriteLine();
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim() ?? "";

            if (input.Equals("E", StringComparison.OrdinalIgnoreCase))
                return "E";

            return input;
        }

        /// <summary>
        /// Visar Retry/Keep/Cancel-meny efter lyckad AI-generering.
        /// </summary>
        public static string RetryMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{MainTitleColor}]What would you like to do?[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Keep", "Regenerate", "Cancel")
                    .UseConverter(choice => $"[white]{choice}[/]"));
        }

        /// <summary>
        /// Visar AI-genererat resultat i en snygg summary.
        /// </summary>
        public static void ShowGeneratedSummary(string title, Dictionary<string, string> fields)
        {
            Console.Clear();
            ConsoleHelpers.Info($"Generated {title}:");
            Console.WriteLine();

            foreach (var field in fields)
            {
                var value = string.IsNullOrWhiteSpace(field.Value) ? "[grey]N/A[/]" : field.Value;

                if (field.Key == fields.Keys.First())
                    AnsiConsole.MarkupLine($"[grey]{field.Key}:[/] [{MainTitleColor}]{Markup.Escape(value)}[/]");
                else
                    AnsiConsole.MarkupLine($"[grey]{field.Key}:[/] {Markup.Escape(value)}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Visar laddningstext under AI-generering.
        /// </summary>
        public static void ShowGeneratingText(string entityType)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[yellow]Generating {entityType} with Gemini AI...[/]");
            Console.WriteLine();
        }

        /// <summary>
        /// Visar felmeddelande vid misslyckad AI-generering.
        /// </summary>
        public static void ShowError(string message)
        {
            AnsiConsole.MarkupLine($"[red]{message}[/]");
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Visar bekräftelse när något sparats.
        /// </summary>
        public static void ShowSaved(string entityType, string name)
        {
            ConsoleHelpers.Result(true, $"{entityType} '{name}' saved!");
            Thread.Sleep(1200);
        }

        /// <summary>
        /// Visar meddelande när generering avbryts.
        /// </summary>
        public static void ShowCancelled()
        {
            AnsiConsole.MarkupLine("[grey]Generation cancelled.[/]");
            Thread.Sleep(800);
        }
    }
}
