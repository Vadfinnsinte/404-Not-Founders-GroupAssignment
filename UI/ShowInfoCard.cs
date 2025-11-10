using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.UI
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

            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var nameProperty = type.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            var titleProperty = type.GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);
            var title = nameProperty?.GetValue(obj)?.ToString()
                     ?? titleProperty?.GetValue(obj)?.ToString()
                     ?? type.Name.ToUpper();

            var table = new Table()
                .Border(TableBorder.Simple)
                .BorderColor(Color.Orange1)
                .AddColumn(new TableColumn("").RightAligned())
                .AddColumn(new TableColumn($"[bold orange1]{title}[/]").LeftAligned());


            foreach (var prop in properties)
            {
                // Hoppa över "Name" och "Title"
                if (prop.Name.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                    prop.Name.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                // Lägg till så att den visar namnet på karaktärer och världar när man kör storyline. - Använd de existerande listorna och kör t.ex worlds.Select(w => w.Name).ToList(); (gör sedan om till string och använd dessa i en row.

                var value = prop.GetValue(obj) ?? "[grey]null[/]";
                table.AddRow($"{prop.Name}:", value.ToString());
                table.AddEmptyRow();
            }

            var panel = new Panel(table)
            {
                Border = BoxBorder.Rounded,
            }.BorderColor(Color.Orange1);

            AnsiConsole.Write(panel);
        }
    }
}
