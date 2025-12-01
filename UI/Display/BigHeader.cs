using Spectre.Console;


namespace _404_not_founders.UI.Display
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
