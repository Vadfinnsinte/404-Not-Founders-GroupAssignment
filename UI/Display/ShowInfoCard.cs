using _404_not_founders.Models;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Reflection;

namespace _404_not_founders.UI.Display
{
    public class ShowInfoCard
    {
        public static void ShowObject<T>(T obj)
        {

            if (obj == null)
            {
                AnsiConsole.MarkupLine("[red]Object is null![/]");
                return;
            }

            var type = typeof(T); // determines which class was passed.
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);// only takes public attributes, instance (does not take static ones)

            var nameProperty = type.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance); // Tries to find the value for "name"
            var titleProperty = type.GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);// Tries to find the value for "title"

            // Sets the value to name or title. If the class doesn't have these, the value becomes the class name itself (e.g. "CHARACTER")
            var title = nameProperty?.GetValue(obj)?.ToString()
                     ?? titleProperty?.GetValue(obj)?.ToString()
                     ?? type.Name.ToUpper();
           

            var table = new Table()
                .Border(TableBorder.Simple)
                .BorderColor(Color.Orange1)
                .AddColumn(new TableColumn("").RightAligned())
                .AddColumn(new TableColumn($"[bold orange1]{title}[/]").LeftAligned());


            foreach (var prop in properties) // creates table-row for each value
            {
                // Skip "Name" and "Title"
                if (prop.Name.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                    prop.Name.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                // Get the value of the property
                var value = prop.GetValue(obj) ?? "[grey]null[/]";

                // Add the property name and value to the table
                table.AddRow($"{prop.Name}:", value.ToString());
                table.AddEmptyRow();
            }

            // Create a panel to hold the table
            var panel = new Panel(table)
            {
                Border = BoxBorder.Rounded,
                Expand = false
            }.BorderColor(Color.Orange1);

            AnsiConsole.Write(panel);
        }
    }
}
