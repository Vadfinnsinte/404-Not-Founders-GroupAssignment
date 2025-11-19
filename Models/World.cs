

using _404_not_founders.Services;
using _404_not_founders.UI;
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
                // Display header
                AnsiConsole.MarkupLine($"[underline #FFA500]Create New World[/]");
                AnsiConsole.MarkupLine("[grey italic]Type 'B' to go back or 'E' to exit[/]"); 
                Console.WriteLine();

                // Display current inputs
                if (step >= 1) AnsiConsole.MarkupLine($"[#FFA500]Name:[/] {name}");
                if (step >= 2) AnsiConsole.MarkupLine($"[#FFA500]Climate:[/] {climate}");
                if (step >= 3) AnsiConsole.MarkupLine($"[#FFA500]Regions:[/] {regions}");
                if (step >= 4) AnsiConsole.MarkupLine($"[#FFA500]Enemies:[/] {enemies}");
                if (step >= 5) AnsiConsole.MarkupLine($"[#FFA500]Factions:[/] {factions}");

                // Prompt based on step
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

                // When all steps are done, ask for confirmation
                if (step == 6)
                {
                    Console.WriteLine(); // Estetic spacing
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with this world?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    if (confirm == "Exit") return;
                    if (confirm == "No (Start over)") { step = 0; continue; }

                    if (confirm == "Yes")
                    {
                        // Save to object
                        this.Name = name;
                        this.Climate = climate;
                        this.Regions = regions;
                        this.Enemies = enemies;
                        this.Factions = factions;
                        this.OtherInfo = otherInfo;

                        // Add to user's project
                        project.Worlds.Add(this);
                        userService.SaveUserService();

                        AnsiConsole.MarkupLine($"[orange1]World '{this.Name}' has been saved![/]");
                        Thread.Sleep(1200);
                        return;
                    }
                }

                // Input from user
                string input = AnsiConsole.Ask<string>($"[#FFA500]{prompt}[/]");

                if (input.Trim().Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0) step--;
                    continue;
                }

                if (input.Trim().Equals("E", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

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
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
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
