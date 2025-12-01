using System.Text;
using System.Text.Json;       // Built-in JSON serializer
using System.Text.Json.Nodes; // For simple navigation/parsing of JSON responses

namespace _404_not_founders.Services
{
    /// Thin wrapper around the Google Gemini HTTP API.
    /// Sends plain-text prompts and returns the raw text response from the model.
    public class GeminiAIService
    {
        private readonly string _apiKey;

        // IMPORTANT: HttpClient is static and reused to avoid socket exhaustion.
        private static readonly HttpClient _httpClient = new HttpClient();

        /// Creates a new GeminiAIService using the provided Google API key.
        public GeminiAIService(string googleApiKey)
        {
            _apiKey = googleApiKey;
        }

        /// Sends a prompt to the Gemini 2.5 Flash model and returns the text reply.
        /// Returns null if the request fails or the response cannot be parsed.
        public async Task<string?> GenerateAsync(string prompt)
        {
            // Use the 'gemini-2.5-flash' model: fast and cost-effective.
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            // Minimal request body for text-only prompts.
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
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
                    // Basic error logging to console for debugging
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"Gemini API Error: {response.StatusCode} - {(string.IsNullOrWhiteSpace(responseText) ? "<empty body>" : responseText)}");
                    Console.ResetColor();
                    return null;
                }

                // Parse JSON response and extract the model's text
                var jsonNode = JsonNode.Parse(responseText);

                string? aiReply = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                // Return trimmed text or null if missing
                return aiReply?.Trim();
            }
            catch (Exception ex)
            {
                // Connection / network / parsing errors
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Gemini Connection Error: {ex.Message}");
                Console.ResetColor();
                return null;
            }
        }
    }
}
