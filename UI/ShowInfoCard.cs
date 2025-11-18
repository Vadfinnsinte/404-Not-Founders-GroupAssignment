using _404_not_founders.Models;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Reflection;

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

            var type = typeof(T); // tar reda på vilken class som skickats. 
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);// tar bara Public attribut, Instance(tar inte statiska)

            var nameProperty = type.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance); // Försöker hitta värdet för "name" 
            var titleProperty = type.GetProperty("Title", BindingFlags.Public | BindingFlags.Instance); // Försöker hitta värdet för"title" 

            //sätter värdet till name eller title. Om classen inte har dessa blir värdet classen, altså typ "CHARACHTER"
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
                // Lägg till så att den visar namnet på karaktärer och världar när man kör storyline. - Använd de existerande listorna och kör t.ex worlds.Select(w => w.Name).ToList(); (gör sedan om till string och använd dessa i en table.AddRow.) 

                var value = prop.GetValue(obj) ?? "[grey]null[/]";


                table.AddRow($"{prop.Name}:", value.ToString());
                table.AddEmptyRow();
            }

            var panel = new Panel(table)
            {
                Border = BoxBorder.Rounded,
                Expand = false
            }.BorderColor(Color.Orange1);

            AnsiConsole.Write(panel);
        }
    }
}
