using _404_not_founders.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.UI
{
    public class ProjectEditVisuals
    {
        public static string ShowEditMenu(Project project)
        {
            var shortDesc = project.description?.Length > 20
            ? project.description.Substring(0, 25) + "..."
            : project.description;

            AnsiConsole.MarkupLine($"Title: {project.title}");
            AnsiConsole.MarkupLine($"Description: {shortDesc}");
            var choises = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]What do you want to do?[/]")
                    .PageSize(10)
                    .AddChoices("Edit/Add Charachters", "Edit/Add worlds", "Edit/Add Storylines", "Show Everything", "Back to main manu")
                    .HighlightStyle(Color.Orange1));

            return choises;
        }

    }
}
