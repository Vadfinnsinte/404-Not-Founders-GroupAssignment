using System.Text;
using System.Text.Json; // Vi kör inbyggda JSON-hanteraren rakt igenom
using System.Text.Json.Nodes; // För att parsa svaret enkelt

namespace _404_not_founders.Services
{
    public class GeminiAIService
    {
        private readonly string _apiKey;
        // VIKTIGT: HttpClient ska vara static så den återanvänds
        private static readonly HttpClient _httpClient = new HttpClient();

        public GeminiAIService(string googleApiKey)
        {
            _apiKey = googleApiKey;
        }

        public async Task<string?> GenerateAsync(string prompt)
        {
            // Använder modellen 'gemini-2.5-flash' som är snabb och billig/gratis
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            string jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Gemini API Error: {response.StatusCode} - {responseText}");
                    Console.ResetColor();
                    return null;
                }

                // Parsa JSON svaret
                var jsonNode = JsonNode.Parse(responseText);
                string? aiReply = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                return aiReply?.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini Connection Error: {ex.Message}");
                return null;
            }
        }
    }
}