using _404_not_founders.Menus;              // MenuChoises
using _404_not_founders.Services;           // UserService, ProjectService, DungeonMasterAI
using _404_not_founders.UI;                 // ConsoleHelpers, ShowInfoCard, AskStepInput
using Spectre.Console;                      // Meny, markeringar och prompts
using Microsoft.Extensions.Configuration;   // För API-nyckel och config

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

            var temp = new World
            {
                Name = this.Name,
                Climate = this.Climate,
                Regions = this.Regions,
                Enemies = this.Enemies,
                Factions = this.Factions,
                OtherInfo = this.OtherInfo
            };

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit world: [#FFA500]{temp.Name}[/]");

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
                    ConsoleHelpers.Info("World summary:");
                    AnsiConsole.MarkupLine($"[grey]Name:[/]      [#FFA500]{temp.Name}[/]");
                    AnsiConsole.MarkupLine($"[grey]Climate:[/]   {temp.Climate}");
                    AnsiConsole.MarkupLine($"[grey]Regions:[/]   {temp.Regions}");
                    AnsiConsole.MarkupLine($"[grey]Enemies:[/]   {temp.Enemies}");
                    AnsiConsole.MarkupLine($"[grey]Factions:[/]  {temp.Factions}");
                    AnsiConsole.MarkupLine($"[grey]Other info:[/] {temp.OtherInfo}");

                    Console.WriteLine();
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

        public async Task<World?> GenerateWorldWithGeminiAI(Project currentProject, UserService userService)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string googleApiKey = config["GoogleAI:ApiKey"];
            var aiHelper = new GeminiAIService(googleApiKey);

            Console.WriteLine("Describe your World, or press [Enter] for a fantasy default:");
            var input = Console.ReadLine();

            // Förbättrad prompt, alltid ALLA rubriker, projektbaserad:
            string prompt = string.IsNullOrWhiteSpace(input)
            ? $@"Generate a one-of-a-kind, highly randomized fantasy World for a Dungeons & Dragons campaign.
            Base all details on this Project description: '{currentProject.description}'.

            NEVER omit any section below. Always include EVERY header below, even if blank, even if unknown. 
            If you can't invent something, write the header and leave it empty. Format, NO markdown or asterisks:

            Name: ...
            Climate: ...
            Regions: ...
            Enemies: ...
            Factions: ...
            OtherInfo: ...

            Example:
            Name: Aetherium
            Climate: [your text]
            Regions: [your text]
            Enemies: [your text]
            Factions: [your text]
            OtherInfo: [your text]
            "
            : input + "\n\nFormat as above. Always include all headers, even if empty. Base your content on the current Project description.";

            Console.WriteLine("Generating World with Gemini...");
            string result = await aiHelper.GenerateAsync(prompt);

            if (!string.IsNullOrWhiteSpace(result))
            {
                Console.WriteLine("\nYour Gemini-generated World:\n" + result);

                var newWorld = ParseAITextToWorld(result);
                if (newWorld != null)
                {
                    currentProject.Worlds.Add(newWorld);
                    userService.SaveUserService();
                    ConsoleHelpers.Info($"World '{newWorld.Name}' created!");
                    Console.Clear();

                    // Visningskod - alltid ALLA rubriker:
                    AnsiConsole.MarkupLine("[bold orange1]World summary:[/]");
                    AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{newWorld.Name}[/]");
                    AnsiConsole.MarkupLine($"[grey]Climate:[/] {ShowValue(newWorld.Climate)}");
                    AnsiConsole.MarkupLine($"[grey]Regions:[/] {ShowValue(newWorld.Regions)}");
                    AnsiConsole.MarkupLine($"[grey]Enemies:[/] {ShowValue(newWorld.Enemies)}");
                    AnsiConsole.MarkupLine($"[grey]Factions:[/] {ShowValue(newWorld.Factions)}");
                    AnsiConsole.MarkupLine($"[grey]Other info:[/] {ShowValue(newWorld.OtherInfo)}");
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();

                    return newWorld;
                }
                else
                {
                    Console.WriteLine("Unable to parse generated World, check the format.");
                    Console.ReadKey(true);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Unable to generate AI-World with Gemini.");
                Thread.Sleep(1200);
                return null;
            }
        }

        private static string ShowValue(string value) => string.IsNullOrWhiteSpace(value) ? "[grey]N/A[/]" : value;

        public static World? ParseAITextToWorld(string input)
        {
            var world = new World();

            // Splitta på radbrytning, hantera både \n och \r\n
            var lines = input.Replace("\r\n", "\n").Split('\n');
            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.StartsWith("Name:", StringComparison.OrdinalIgnoreCase))
                    world.Name = cleanLine.Substring(5).Trim();
                else if (cleanLine.StartsWith("Climate:", StringComparison.OrdinalIgnoreCase))
                    world.Climate = cleanLine.Substring(8).Trim();
                else if (cleanLine.StartsWith("Regions:", StringComparison.OrdinalIgnoreCase))
                    world.Regions = cleanLine.Substring(8).Trim();
                else if (cleanLine.StartsWith("Enemies:", StringComparison.OrdinalIgnoreCase))
                    world.Enemies = cleanLine.Substring(8).Trim();
                else if (cleanLine.StartsWith("Factions:", StringComparison.OrdinalIgnoreCase))
                    world.Factions = cleanLine.Substring(9).Trim();
                else if (cleanLine.StartsWith("OtherInfo:", StringComparison.OrdinalIgnoreCase))
                    world.OtherInfo = cleanLine.Substring(10).Trim();
            }

            // Sätt alltid default till tom sträng om saknas
            world.Name = world.Name ?? "";
            world.Climate = world.Climate ?? "";
            world.Regions = world.Regions ?? "";
            world.Enemies = world.Enemies ?? "";
            world.Factions = world.Factions ?? "";
            world.OtherInfo = world.OtherInfo ?? "";

            // Minsta krav: Namn och klimat måste finnas!
            if (string.IsNullOrWhiteSpace(world.Name) || string.IsNullOrWhiteSpace(world.Climate))
                return null;

            return world;
        }
    }
}