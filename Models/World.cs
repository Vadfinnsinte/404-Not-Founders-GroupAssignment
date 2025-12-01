

using System.Text;
using _404_not_founders.Services;
using _404_not_founders.UI.Display;
using _404_not_founders.UI.Helpers;
using Spectre.Console;

namespace _404_not_founders.Models
{
    public class World
    {
        public string Name { get; set; }
        public string Climate { get; set; }
        public string Regions { get; set; }
        public string Enemies { get; set; }
        public string Factions { get; set; }
        public string OtherInfo { get; set; }



        public void Add(User user, Project project, UserService userService)
        {
            // Initiating variables to store user input
            string name = "", climate = "", regions = "", enemies = "", factions = "", otherInfo = "";
            int step = 0;

            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[underline #FFA500]Create New World[/]");
                AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]");
                Console.WriteLine();

                // Show current progress
                if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Name:[/] {name}");
                if (step >= 2) AnsiConsole.MarkupLine($"[#FFA500]Climate:[/] {climate}");
                if (step >= 3) AnsiConsole.MarkupLine($"[#FFA500]Regions:[/] {regions}");
                if (step >= 4) AnsiConsole.MarkupLine($"[#FFA500]Enemies:[/] {enemies}");
                if (step >= 5) AnsiConsole.MarkupLine($"[#FFA500]Factions:[/] {factions}");

                // Step prompt
                string prompt = step switch
                {
                    0 => "World Name:",
                    1 => "Climate:",
                    2 => "Regions:",
                    3 => "Enemies:",
                    4 => "Factions:",
                    5 => "Other information:",
                    _ => ""
                };

                // Confirmation step
                if (step == 6)
                {
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with this world?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit")
                    );

                    // Handle confirmation choices
                    if (confirm == "Exit") return;
                    if (confirm == "No (Start over)") { step = 0; continue; }

                    if (confirm == "Yes")
                    {
                        // Save values
                        this.Name = name;
                        this.Climate = climate;
                        this.Regions = regions;
                        this.Enemies = enemies;
                        this.Factions = factions;
                        this.OtherInfo = otherInfo;

                        // Add world to project and save
                        project.Worlds.Add(this);
                        userService.SaveUserService();

                        AnsiConsole.MarkupLine($"[orange1]World '{this.Name}' has been saved![/]");
                        Thread.Sleep(1200);
                        return;
                    }
                }

                // Use generic AskStepInputs
                string input = AskStepInput.AskStepInputs(prompt);

                if (input == "E")
                    return;

                if (input == "B")
                {
                    if (step > 0) step--;
                    continue;
                }

                // Store input
                switch (step)
                {
                    case 0: name = input; break;
                    case 1: climate = input; break;
                    case 2: regions = input; break;
                    case 3: enemies = input; break;
                    case 4: factions = input; break;
                    case 5: otherInfo = input; break;
                }

                step++;
            }
        }
        public void EditWorld(UserService userService)
        {
            // Create a temporary copy of the world to edit
            var temp = new World
            {
                Name = this.Name,
                Climate = this.Climate,
                Regions = this.Regions,
                Enemies = this.Enemies,
                Factions = this.Factions,
                OtherInfo = this.OtherInfo
            };

            void ShowSummary(World w)
            {
                // Build a multiline markup string for the panel so the summary is visible above the prompt
                var sb = new StringBuilder();
                sb.AppendLine("[underline #FFA500]World summary:[/]");
                sb.AppendLine($"[grey]Name:[/]       [#FFA500]{(string.IsNullOrWhiteSpace(w.Name) ? "(unnamed)" : w.Name)}[/]");
                sb.AppendLine($"[grey]Climate:[/]    [#FFA500]{(string.IsNullOrWhiteSpace(w.Climate) ? "-" : w.Climate)}[/]");
                sb.AppendLine($"[grey]Regions:[/]    [#FFA500]{(string.IsNullOrWhiteSpace(w.Regions) ? "-" : w.Regions)}[/]");
                sb.AppendLine($"[grey]Enemies:[/]    [#FFA500]{(string.IsNullOrWhiteSpace(w.Enemies) ? "-" : w.Enemies)}[/]");
                sb.AppendLine($"[grey]Factions:[/]   [#FFA500]{(string.IsNullOrWhiteSpace(w.Factions) ? "-" : w.Factions)}[/]");
                sb.AppendLine($"[grey]Other info:[/] [#FFA500]{(string.IsNullOrWhiteSpace(w.OtherInfo) ? "-" : w.OtherInfo)}[/]");

                // Create and fix the panel
                var panel = new Panel(new Markup(sb.ToString()))
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 0, 1, 0),
                };

                AnsiConsole.Write(panel);
                Console.WriteLine();
            }

            // Loop until user is done editing
            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit world: [#FFA500]{temp.Name}[/]");

                // Show live summary before presenting choices
                ShowSummary(temp);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to change?")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices(
                            "Name",
                            "Climate",
                            "Regions",
                            "Enemies",
                            "Factions",
                            "Other info",
                            "Done"));

                // Helper function to prompt for non-empty input
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
                    // Re-show summary for final confirmation (consistent with live preview) and ask for confirmation
                    ShowSummary(temp);

                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with this world?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    if (confirm == "Exit")
                    {
                        Thread.Sleep(800);
                        Console.Clear();
                        return;
                    }

                    // Restart editing from the beginning
                    if (confirm == "No (Start over)")
                    {
                        temp.Name = this.Name;
                        temp.Climate = this.Climate;
                        temp.Regions = this.Regions;
                        temp.Enemies = this.Enemies;
                        temp.Factions = this.Factions;
                        temp.OtherInfo = this.OtherInfo;
                        continue;
                    }

                    // Save changes to the original world
                    if (confirm == "Yes")
                    {
                        this.Name = temp.Name;
                        this.Climate = temp.Climate;
                        this.Regions = temp.Regions;
                        this.Enemies = temp.Enemies;
                        this.Factions = temp.Factions;
                        this.OtherInfo = temp.OtherInfo;

                        userService.SaveUserService();
                        AnsiConsole.MarkupLine("[green]World updated![/]");
                        Thread.Sleep(1000);
                        Console.Clear();
                        return;
                    }
                }

                // Handle individual field edits
                switch (choice)
                {
                    case "Name":
                        temp.Name = PromptNonEmpty("[#FFA500]New name:[/]");
                        break;
                    case "Climate":
                        temp.Climate = PromptNonEmpty("[#FFA500]New climate:[/]");
                        break;
                    case "Regions":
                        temp.Regions = PromptNonEmpty("[#FFA500]New regions:[/]");
                        break;
                    case "Enemies":
                        temp.Enemies = PromptNonEmpty("[#FFA500]New enemies:[/]");
                        break;
                    case "Factions":
                        temp.Factions = PromptNonEmpty("[#FFA500]New factions:[/]");
                        break;
                    case "Other info":
                        temp.OtherInfo = PromptNonEmpty("[#FFA500]New other info:[/]");
                        break;
                }
            }
        }
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }

        public void DeleteWorld(Project project, UserService userService)
        {
            Console.Clear();

            // Ask for confirmation
            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{this.Name}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                // Remove the world from the project's worlds list
                if (project.Worlds.Contains(this))
                {
                    project.Worlds.Remove(this);

                    // Save changes
                    userService.SaveUserService();

                    AnsiConsole.MarkupLine($"World '[orange1]{this.Name}[/]' has been deleted!");
                    Thread.Sleep(1200);
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]Error: World not found.[/]");
                    Thread.Sleep(1200);
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
                Thread.Sleep(1200);
            }
        }
    }
}
