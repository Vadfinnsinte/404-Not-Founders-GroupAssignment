

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



        public void Add(User user, UserService userService)
        {
            // Prompt user for world details
            string worldName = AnsiConsole.Ask<string>("[#FFA500]Enter world name:[/]");
            string worldClimate = AnsiConsole.Ask<string>("[#FFA500]Climate:[/]");
            string worldRegions = AnsiConsole.Ask<string>("[#FFA500]Regions:[/]");
            string worldEnemies = AnsiConsole.Ask<string>("[#FFA500]Enemies:[/]");
            string worldFactions = AnsiConsole.Ask<string>("[#FFA500]Factions:[/]");
            string worldOtherInfo = AnsiConsole.Ask<string>("[#FFA500]Other information:[/]");

            // Create new World object
            var newWorld = new World
            {
                Name = worldName,
                Climate = worldClimate,
                Regions = worldRegions,
                Enemies = worldEnemies,
                Factions = worldFactions,
                OtherInfo = worldOtherInfo
            };

            // Add to user's worlds
            user.Worlds.Add(newWorld);

            // Save changes to JSON
            userService.SaveUserService();

            AnsiConsole.MarkupLine($"Världen '[#FFA500]{newWorld.Name}[/]' har sparats!");
            Thread.Sleep(1200);

        }
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
