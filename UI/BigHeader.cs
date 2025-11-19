using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.UI
{
    public class BigHeader
    {
        public static void Show(string name)
        {
        AnsiConsole.Write(
          new FigletText($"{name}: ")
              .Centered()
              .Color(Color.White));
        }
    }
}
