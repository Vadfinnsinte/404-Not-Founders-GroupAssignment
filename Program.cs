using _404_not_founders.Menus;
using _404_not_founders.Models;
using _404_not_founders.Services;
using Microsoft.Extensions.Configuration; // Kräver paket: Microsoft.Extensions.Configuration.Json

namespace _404_not_founders
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Hämta API-nyckeln (säkrare än att skriva den direkt i koden)
            // Från Environment Variable (bäst för säkerhet)
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_KEY");

            // 2. Initiera AI-tjänsten
            var aiService = new GeminiAIService(apiKey);

            // 3. Initiera övriga tjänster
            var userService = new UserService();
            var projectService = new ProjectService(userService);

            userService.LoadUserService();

            // 4. Skicka med aiService till RunApp!
            // OBS: Du behöver uppdatera konstruktorn i RunApp.cs för att ta emot denna parameter.
            var runApp = new RunApp(userService, projectService, aiService);

            await runApp.Run();
        }
    }
}