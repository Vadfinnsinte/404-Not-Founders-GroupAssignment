using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;
using Spectre.Console;


namespace _404_not_founders.UI.CRUD.Storylines
{
    public class EditStoryline
    {
        public static void Edit(Project project, Storyline original, UserService userService)
        {

            var temp = new Storyline
            {
                Title = original.Title,
                Synopsis = original.Synopsis,
                Theme = original.Theme,
                Genre = original.Genre,
                Story = original.Story,
                IdeaNotes = original.IdeaNotes,
                OtherInfo = original.OtherInfo,
                orderInProject = original.orderInProject,
                dateOfLastEdit = original.dateOfLastEdit
            };

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit storyline: [#FFA500]{temp.Title}[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to change?")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices(
                            "Title",
                            "Synopsis",
                            "Theme",
                            "Genre",
                            "Story",
                            "Idea notes",
                            "Other info",
                            "Order in project",
                            "Done")
                        .HighlightStyle(Color.Orange1));


                string PromptNonEmpty(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (!string.IsNullOrWhiteSpace(value))
                            return value;

                        AnsiConsole.MarkupLine("[red]Value cannot be empty.[/]");
                    }
                }

                if (choice == "Done")
                {
                    Console.Clear();
                    ConsoleHelpers.Info("Storyline summary:");
                    AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{temp.Title}[/]");
                    AnsiConsole.MarkupLine($"[grey]Synopsis:[/] {temp.Synopsis}");
                    AnsiConsole.MarkupLine($"[grey]Theme:[/] {temp.Theme}");
                    AnsiConsole.MarkupLine($"[grey]Genre:[/] {temp.Genre}");
                    AnsiConsole.MarkupLine($"[grey]Story:[/] {temp.Story}");
                    AnsiConsole.MarkupLine($"[grey]Idea notes:[/] {temp.IdeaNotes}");
                    AnsiConsole.MarkupLine($"[grey]Other info:[/] {temp.OtherInfo}");
                    AnsiConsole.MarkupLine($"[grey]Order in project:[/] {temp.orderInProject}");

                    Console.WriteLine();
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with this storyline?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    if (confirm == "Exit")
                    {
                        ConsoleHelpers.DelayAndClear();
                        return;
                    }

                    if (confirm == "No (Start over)")
                    {
                        temp.Title = original.Title;
                        temp.Synopsis = original.Synopsis;
                        temp.Theme = original.Theme;
                        temp.Genre = original.Genre;
                        temp.Story = original.Story;
                        temp.IdeaNotes = original.IdeaNotes;
                        temp.OtherInfo = original.OtherInfo;
                        temp.orderInProject = original.orderInProject;
                        continue;
                    }

                    if (confirm == "Yes")
                    {
                        original.Title = temp.Title;
                        original.Synopsis = temp.Synopsis;
                        original.Theme = temp.Theme;
                        original.Genre = temp.Genre;
                        original.Story = temp.Story;
                        original.IdeaNotes = temp.IdeaNotes;
                        original.OtherInfo = temp.OtherInfo;
                        original.dateOfLastEdit = DateTime.Now;

                        int oldOrder = original.orderInProject;
                        int newOrder = temp.orderInProject;

                        if (newOrder != oldOrder)
                        {
                            if (newOrder < oldOrder)
                            {
                                foreach (var sl in project.Storylines.Where(s =>
                                             s != original &&
                                             s.orderInProject >= newOrder &&
                                             s.orderInProject < oldOrder))
                                {
                                    sl.orderInProject++;
                                }
                            }
                            else
                            {
                                foreach (var sl in project.Storylines.Where(s =>
                                             s != original &&
                                             s.orderInProject <= newOrder &&
                                             s.orderInProject > oldOrder))
                                {
                                    sl.orderInProject--;
                                }
                            }

                            original.orderInProject = newOrder;
                        }

                        userService.SaveUserService();
                        ConsoleHelpers.Info("Storyline updated!");
                        ConsoleHelpers.DelayAndClear();
                        return;
                    }
                }


                switch (choice)
                {
                    case "Title":
                        temp.Title = PromptNonEmpty("[#FFA500]New title:[/]");
                        break;
                    case "Synopsis":
                        temp.Synopsis = PromptNonEmpty("[#FFA500]New synopsis:[/]");
                        break;
                    case "Theme":
                        temp.Theme = PromptNonEmpty("[#FFA500]New theme:[/]");
                        break;
                    case "Genre":
                        temp.Genre = PromptNonEmpty("[#FFA500]New genre:[/]");
                        break;
                    case "Story":
                        temp.Story = PromptNonEmpty("[#FFA500]New story content:[/]");
                        break;
                    case "Idea notes":
                        temp.IdeaNotes = PromptNonEmpty("[#FFA500]New idea notes:[/]");
                        break;
                    case "Other info":
                        temp.OtherInfo = PromptNonEmpty("[#FFA500]New other information:[/]");
                        break;
                    case "Order in project":
                        int maxOrder = project.Storylines.Count;
                        while (true)
                        {
                            var input = AnsiConsole.Ask<string>(
                                $"[#FFA500]New order in project (1 - {maxOrder}):[/]");
                            if (int.TryParse(input, out var newOrder) &&
                                newOrder >= 1 && newOrder <= maxOrder)
                            {
                                temp.orderInProject = newOrder;
                                break;
                            }

                            AnsiConsole.MarkupLine("[red]Please enter a number within the range.[/]");
                        }
                        break;
                }
            }
        }
    }
}
