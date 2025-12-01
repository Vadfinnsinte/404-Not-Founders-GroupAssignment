using Spectre.Console;


namespace _404_not_founders.UI.Helpers
{
    // Waits for a key press, then fully clears the console (including scrollback).
    public static class AnsiClearHelper
    {
        public static void WaitForKeyAndClear()
        {
            AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
            Console.ReadKey(true);

         
            Console.Write("\u001b[3J"); // added because of duplcation issues with BigHeader in show everything.
            Console.Clear();
        }
    }
}
