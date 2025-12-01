
using System.Text.Json;
using _404_not_founders.Models;


namespace _404_not_founders.Services
{
    public class UserService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public List<User> Users { get; set; } = new List<User>();


        public UserService()
        {
            //  Find the base folder the user's AppData
            string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create a subfolder for the application
            _folderPath = Path.Combine(baseFolder, "Adventurer’s Journal");
            Directory.CreateDirectory(_folderPath);

            // File path for storing JSON data
            _filePath = Path.Combine(_folderPath, "user.json");

        }

        public void SaveUserService()
        {
            try
            {
                // Serialize the entire UserService to JSON and save to file
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            // Handle any exceptions that may occur
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sparande av fil: {ex.Message}");
            }
        }

        public void LoadUserService()
        {
            // Load and deserialize the UserService from JSON file
            if (!File.Exists(_filePath)) return;
            try
            {

                string json = File.ReadAllText(_filePath);

                var loaded = JsonSerializer.Deserialize<UserService>(json);
                if (loaded != null)
                {
                    Users = loaded.Users;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Något gick fel när filen laddades, error meddelande:  {ex.Message} ");
            }
        }

        // Removes a user by username and saves the updated user list
        public bool RemoveUser(string username)
        {
            int countBefore = Users.Count;
            Users.RemoveAll(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            bool removed = Users.Count < countBefore;
            if (removed) SaveUserService();
            return removed;
        }
        
    }
}
