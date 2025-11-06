
using _404_not_founders.Models;

namespace _404_not_founders.Menus
{
    public class MenuHelper
    {

        public void StartMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public static void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("Välkommen till HM!");
            Console.WriteLine("1. Logga in");
            Console.WriteLine("2. Registrera dig");
            Console.WriteLine("0. Avsluta");
        }
        public static int ShowLoginRegisterMenu()
        {
            ShowWelcome();
            Console.Write("\nDitt val: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
                return choice;
            return -1;
        }
        public static User RegisterMenu()
        {
            Console.Clear();
            Console.WriteLine("Registrera ny användare");
            Console.Write("E-post: ");
            string email = Console.ReadLine();

            Console.Write("Användarnamn: ");
            string username = Console.ReadLine();

            Console.Write("Lösenord: ");
            string password = Console.ReadLine();

            // Här kan du lägga till kontroll för att kolla så att epost/username inte redan finns
            // och ev. password hashing!

            return new User
            {
                Email = email,
                Username = username,
                Password=password,
                CreationDate = DateTime.Now,
                Projects = new List<Project>()
            };
        }
        public static int ShowLoggedInMenu(string username)
        {
            Console.Clear();
            Console.WriteLine($"Inloggad som {username}");
            Console.WriteLine("1. Lägg till projekt");
            Console.WriteLine("2. Visa projekt");
            Console.WriteLine("3. Senaste projekt");
            Console.WriteLine("4. Redigera konto");
            Console.WriteLine("0. Logga ut");
            Console.Write("\nDitt val: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
                return choice;
            return -1;
        }
        public void ShowProjectMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void ProjectMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void UserMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void WorldMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void CharacterMenu()
        {
            Console.WriteLine("Coming Soon");
        }
        public void StorylineMenu()
        {
            Console.WriteLine("Coming Soon");
        }
    }
}
