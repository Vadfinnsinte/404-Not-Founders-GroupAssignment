using _404_not_founders.Menus;
using _404_not_founders.Models;
using _404_not_founders.Services;
using Microsoft.Extensions.Configuration;

namespace _404_not_founders
{
    /// Application entry point
    /// Initializes services and starts the application
    internal class Program
    {
        /// Main entry point for the application
        /// Sets up configuration, services, and runs the app
        static async Task Main(string[] args)
        {
            // 1. Get API key from environment variable (most secure)
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_KEY");

            // 2. If environment variable not set, try reading from appsettings.json
            if (string.IsNullOrEmpty(apiKey))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                apiKey = config["GoogleAI:ApiKey"];
            }

            // 3. Initialize AI service with API key
            var aiService = new GeminiAIService(apiKey);

            // 4. Initialize user and project services
            var userService = new UserService();
            var projectService = new ProjectService(userService);

            // 5. Load existing user data from storage
            userService.LoadUserService();

            // 6. Create and run the application
            var runApp = new RunApp(userService, projectService, aiService);

            // Start the application's main loop (async)
            await runApp.Run();
        }
    }
}
