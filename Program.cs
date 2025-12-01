using _404_not_founders.Menus;
using _404_not_founders.Models;
using _404_not_founders.Services;
using Microsoft.Extensions.Configuration;

namespace _404_not_founders
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Hämta API-nyckeln (säkrare än att skriva den direkt i koden)
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_KEY");

            // Om miljövariabeln inte finns, försök läsa från appsettings.json
            if (string.IsNullOrEmpty(apiKey))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                apiKey = config["GoogleAI:ApiKey"];
            }

            // 2. Initiera AI-tjänsten
            var aiService = new GeminiAIService(apiKey);

            // 3. Initiera övriga tjänster
            var userService = new UserService();
            var projectService = new ProjectService(userService);

            userService.LoadUserService();

            // 4. Skapa och kör appen
            var runApp = new RunApp(userService, projectService, aiService);

            // Start the application's main loop (await eftersom Run() är async)
            await runApp.Run();
        }
    }
}
