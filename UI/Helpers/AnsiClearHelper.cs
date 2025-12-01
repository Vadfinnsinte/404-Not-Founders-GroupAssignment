using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.UI.Helpers
{
    public static class AnsiClearHelper
    {
        public static void WaitForKeyAndClear()
        {
            AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
            Console.ReadKey(true);

         
            Console.Write("\u001b[3J");
            Console.Clear();
        }
    }
}
