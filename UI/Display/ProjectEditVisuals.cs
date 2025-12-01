using _404_not_founders.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.UI.Display
{
    public class ProjectEditVisuals
    {
        public static string ShowEditMenu(Project project)
        {
            
            var shortDesc = project.description?.Length > 25
            ? project.description.Substring(0, 25) + "..."
            : project.description;

            AnsiConsole.MarkupLine(""); // to create some air
            AnsiConsole.MarkupLine("[bold #FFA500]Title: [/]" + $"{project.title}");
            AnsiConsole.MarkupLine("[bold #FFA500]Description: [/]" + $"{shortDesc}");
            var choises = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]What do you want to do?[/]")
                    .PageSize(10)
                    .AddChoices("Edit/Add Characters", "Edit/Add worlds", "Edit/Add Storylines", "Show Everything", "Back to main menu")
                    .HighlightStyle(Color.Orange1));

            return choises;
        }

    }
}
