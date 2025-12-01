using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace _404_not_founders.UI.Helpers
{
    /// Helper utilities for AI-driven workflows (Gemini).
    /// Handles optional user context, Retry/Keep/Cancel flow and basic status messages.
    public static class AiHelper
    {
        private const string MainTitleColor = "#FFA500";

        /// Asks the user for an optional custom description to guide AI generation.
        /// Enter = no extra context, 'E' = cancel and go back.
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

        /// Shows the standard Retry/Keep/Cancel menu after a successful AI generation.
        /// Returns the selected choice as string.
        public static string RetryMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{MainTitleColor}]What would you like to do?[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Keep", "Regenerate", "Cancel")
                    .UseConverter(choice => $"[white]{choice}[/]"));
        }

        /// Displays a simple summary of AI-generated fields for any entity type.
        /// Expects a dictionary where Key = label, Value = text.
        public static void ShowGeneratedSummary(string title, Dictionary<string, string> fields)
        {
            Console.Clear();
            ConsoleHelpers.Info($"Generated {title}:");
            Console.WriteLine();

            foreach (var field in fields)
            {
                var value = string.IsNullOrWhiteSpace(field.Value) ? "[grey]N/A[/]" : field.Value;

                // First field is usually the main title/name, so it gets highlight color
                if (field.Key == fields.Keys.First())
                    AnsiConsole.MarkupLine($"[grey]{field.Key}:[/] [{MainTitleColor}]{Markup.Escape(value)}[/]");
                else
                    AnsiConsole.MarkupLine($"[grey]{field.Key}:[/] {Markup.Escape(value)}");
            }

            Console.WriteLine();
        }

        /// Shows a short "Generating..." message while AI is working.
        public static void ShowGeneratingText(string entityType)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[yellow]Generating {entityType} with Gemini AI...[/]");
            Console.WriteLine();
        }

        /// Shows an error message when AI generation fails.
        public static void ShowError(string message)
        {
            AnsiConsole.MarkupLine($"[red]{message}[/]");
            Thread.Sleep(1500);
        }

        /// Shows a success message when an AI-generated entity has been saved.
        public static void ShowSaved(string entityType, string name)
        {
            ConsoleHelpers.Result(true, $"{entityType} '{Markup.Escape(name)}' saved!");
            Thread.Sleep(1200);
        }

        /// Shows a short info message when AI generation is cancelled by the user.
        public static void ShowCancelled()
        {
            AnsiConsole.MarkupLine("[grey]Generation cancelled.[/]");
            Thread.Sleep(800);
        }
    }
}
