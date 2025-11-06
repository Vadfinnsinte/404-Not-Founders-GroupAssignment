using _404_not_founders.Menus;

namespace _404_not_founders
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            bool loggedIn = false;
            string currentUser = null;

            while (running)
            {
                if (!loggedIn)
                {
                    int choice = MenuHelper.ShowLoginRegisterMenu();
                    switch (choice)
                    {
                        case 1:
                            // Logga in-logik
                            // Om login lyckas:
                            Console.WriteLine("Ange användarnamn:");
                            currentUser = Console.ReadLine();
                            loggedIn = true;
                            break;
                        case 2:
                            // Registreringslogik
                            Console.WriteLine("Registrera nytt konto...");
                            break;
                        case 0:
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Ogiltigt val.");
                            break;
                    }
                }
                else
                {
                    int choice = MenuHelper.ShowLoggedInMenu(currentUser);
                    switch (choice)
                    {
                        case 1:
                            // Lägg till projekt
                            Console.WriteLine("Lägger till projekt...");
                            break;
                        case 2:
                            // Visa projekt
                            Console.WriteLine("Visar alla projekt...");
                            break;
                        case 3:
                            // Senaste projekt
                            Console.WriteLine("Visar senaste projektet...");
                            break;
                        case 4:
                            // Redigera konto
                            Console.WriteLine("Redigerar ditt konto...");
                            break;
                        case 0:
                            loggedIn = false;
                            currentUser = null;
                            break;
                        default:
                            Console.WriteLine("Ogiltigt val.");
                            break;
                    }
                }
            }
            Console.WriteLine("Tack för att du använde appen!");
        }
    }
}
