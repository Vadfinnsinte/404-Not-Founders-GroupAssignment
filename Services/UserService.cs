
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
            //  Hitta användarens AppData eller motsvarande mapp på alla plattformar
            string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            //  Skapa en undermapp för din app
            _folderPath = Path.Combine(baseFolder, "Adventurer’s Journal");
            Directory.CreateDirectory(_folderPath);

            //  Filväg till JSON-filen
            _filePath = Path.Combine(_folderPath, "user.json");

        }

        public void SaveUserService()
        {
            try
            {
                // Serialisera hela UserService-objektet till JSON och skriv till fil
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sparande av fil: {ex.Message}");
            }
        }

        public void LoadUserService()
        {
            // Ladda och deserialisera hela UserService från filen
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

        // Ta bort användare per användarnamn och spara
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
