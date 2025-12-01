using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI.Helpers;
using Spectre.Console;
using System.Text;

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
                orderInProject = original.orderInProject
            };

            void ShowSummary(Storyline s)
            {
                var sb = new StringBuilder();
                sb.AppendLine("[underline #FFA500]Storyline summary:[/]");
                sb.AppendLine($"[grey]Title:[/]      [#FFA500]{(string.IsNullOrWhiteSpace(s.Title) ? "(untitled)" : Markup.Escape(s.Title))}[/]");
                sb.AppendLine($"[grey]Synopsis:[/]   [#FFA500]{(string.IsNullOrWhiteSpace(s.Synopsis) ? "-" : Markup.Escape(s.Synopsis))}[/]");
                sb.AppendLine($"[grey]Theme:[/]      [#FFA500]{(string.IsNullOrWhiteSpace(s.Theme) ? "-" : Markup.Escape(s.Theme))}[/]");
                sb.AppendLine($"[grey]Genre:[/]      [#FFA500]{(string.IsNullOrWhiteSpace(s.Genre) ? "-" : Markup.Escape(s.Genre))}[/]");
                sb.AppendLine($"[grey]Story:[/]      [#FFA500]{(string.IsNullOrWhiteSpace(s.Story) ? "-" : Markup.Escape(s.Story))}[/]");
                sb.AppendLine($"[grey]Idea notes:[/] [#FFA500]{(string.IsNullOrWhiteSpace(s.IdeaNotes) ? "-" : Markup.Escape(s.IdeaNotes))}[/]");
                sb.AppendLine($"[grey]Other info:[/] [#FFA500]{(string.IsNullOrWhiteSpace(s.OtherInfo) ? "-" : Markup.Escape(s.OtherInfo))}[/]");
                sb.AppendLine($"[grey]Order:[/]      [#FFA500]{s.orderInProject}[/]");

                var panel = new Panel(new Markup(sb.ToString()))
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 0, 1, 0),
                };

                AnsiConsole.Write(panel);
                Console.WriteLine();
            }

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit storyline: [#FFA500]{Markup.Escape(temp.Title)}[/]");

                ShowSummary(temp);

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
                            "Done"));

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
                    ShowSummary(temp);

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
