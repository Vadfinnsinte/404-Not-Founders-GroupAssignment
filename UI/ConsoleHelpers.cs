using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace _404_not_founders.UI
{
    public class ConsoleHelpers
    {
        public const string MainTitleColor = "#FFA500";
        public static void Info(string text) => AnsiConsole.MarkupLine($"[underline {MainTitleColor}]{text}[/]");


        /// Delay och skärmrens – anropas efter bekräftelse eller fel
        public static void DelayAndClear(int ms = 800) { Thread.Sleep(ms); Console.Clear(); }
    }
}
