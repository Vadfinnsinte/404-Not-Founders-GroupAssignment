using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using _404_not_founders.Models;
using _404_not_founders.Menus;

namespace _404_not_founders.Services
{
    public class UserService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public List<User> Users { get; set; } = new List<User>();

        // Lägg till dessa eftersom:
        //public List<Character> Characters { get; set; } = new List<Character>();
        //public List<Storyline> Storylines { get; set; } = new List<Storyline>();
        //public List<World> Worlds { get; set; } = new List<World>();

        // !!Håll koll på strukturen: 
        //      "Users": [
        //{
        //  "Email": "Testing",
        //  "Username": "testing",
        //  "Password": "testing",
        //  "CreationDate": "2025-11-10T14:16:43.0761063+01:00",
        //  "Projects": [],
        //  "Characters": [],
        //  "Storyline": [],
        //  "Worlds": []
        ////},
        //]

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
                    //Characters = loaded.Characters;
                    //Storylines = loaded.Storylines;
                    //Worlds = loaded.Worlds;
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