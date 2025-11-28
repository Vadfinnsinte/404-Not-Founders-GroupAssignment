using _404_not_founders.Models;
using _404_not_founders.UI.Helpers;
using Spectre.Console;
using System.Reflection.Metadata.Ecma335;

namespace _404_not_founders.UI.CRUD
{
    public class CreateStoryline
    {
        // Handles promts and input to user when adding a storyline to the project.
        public static void Create(Project project)
        {
            while (true)
            {

                Console.Clear();
                ConsoleHelpers.Info("Create new storyline");

                string title = "", synopsis = "", theme = "", genre = "", story = "", ideaNotes = "", otherInfo = "";
                int step = 0;

                while (true)
                {
                    Console.Clear();
                    ConsoleHelpers.Info("Create new storyline");
                    AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]\n");

                    // Show filled fields
                    if (step >= 1) AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{title}[/]");
                    if (step >= 2) AnsiConsole.MarkupLine($"[grey]Synopsis:[/] [#FFA500]{synopsis}[/]");
                    if (step >= 3) AnsiConsole.MarkupLine($"[grey]Theme:[/] [#FFA500]{theme}[/]");
                    if (step >= 4) AnsiConsole.MarkupLine($"[grey]Genre:[/] [#FFA500]{genre}[/]");
                    if (step >= 5) AnsiConsole.MarkupLine($"[grey]Story content:[/] [#FFA500]{story}[/]");
                    if (step >= 6) AnsiConsole.MarkupLine($"[grey]Idea notes:[/] [#FFA500]{ideaNotes}[/]");
                    if (step >= 7) AnsiConsole.MarkupLine($"[grey]Other info:[/] [#FFA500]{otherInfo}[/]");

                    // Determine prompt
                    string prompt = step switch
                    {
                        0 => "Title:",
                        1 => "Synopsis (short description):",
                        2 => "Theme:",
                        3 => "Genre:",
                        4 => "Story content:",
                        5 => "Idea notes:",
                        6 => "Other information:",
                        _ => ""
                    };

                    if (step > 6)
                        break;

                    string input = AskStepInput.AskStepInputs(prompt);

                    if (input == "E")
                    {
                        Console.Clear();
                        return; // Exit
                    }
                    if (input == "B")
                    {
                        if (step > 0) step--; // Go back
                        continue;
                    }

                    // Save input
                    switch (step)
                    {
                        case 0: title = input; break;
                        case 1: synopsis = input; break;
                        case 2: theme = input; break;
                        case 3: genre = input; break;
                        case 4: story = input; break;
                        case 5: ideaNotes = input; break;
                        case 6: otherInfo = input; break;
                    }

                    step++;
                }

                // Ask for order in project
                project.Storylines ??= new List<Storyline>();
                int maxOrder = project.Storylines.Count + 1;
                int orderInProject = 1;

                if (maxOrder == 1)
                {
                    orderInProject = 1;
                }
                else
                {
                    while (true)
                    {
                        string input = AskStepInput.AskStepInputs($"Order in project (1 - {maxOrder}):", validator: s => int.TryParse(s, out int val) && val >= 1 && val <= maxOrder, validationMessage: $"Enter a number between 1 and {maxOrder}");

                        if (input == "E") return;
                        if (input == "B") { step = 6; break; }

                        orderInProject = int.Parse(input);
                        break;
                    }
                }

                // Confirm storyline
                Console.Clear();
                ConsoleHelpers.Info("Storyline summary:");
                AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{title}[/]");
                AnsiConsole.MarkupLine($"[grey]Synopsis:[/] {synopsis}");
                AnsiConsole.MarkupLine($"[grey]Theme:[/] {theme}");
                AnsiConsole.MarkupLine($"[grey]Genre:[/] {genre}");
                AnsiConsole.MarkupLine($"[grey]Story:[/] {story}");
                AnsiConsole.MarkupLine($"[grey]Idea notes:[/] {ideaNotes}");
                AnsiConsole.MarkupLine($"[grey]Other info:[/] {otherInfo}");

                var confirm = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#FFA500]Are you happy with this storyline?[/]")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices("Yes", "No (Start over)", "Exit"));

                if (confirm == "Exit") return;
                if (confirm == "No (Start over)")
                {
                    //AddStorylineToProject(project); // Restart
                    continue;
                }

                // Adjust order
                foreach (var sl in project.Storylines.Where(sl => sl.orderInProject >= orderInProject))
                    sl.orderInProject++;

                var s = new Storyline
                {
                    Title = title,
                    Synopsis = synopsis,
                    Theme = theme,
                    Genre = genre,
                    Story = story,
                    IdeaNotes = ideaNotes,
                    OtherInfo = otherInfo,
                    orderInProject = orderInProject,
                    dateOfLastEdit = DateTime.Now
                };

                project.Storylines.Add(s);


                ConsoleHelpers.Info("Storyline created!");
                ConsoleHelpers.DelayAndClear();
                break;
            }
        }
    }
    
}

