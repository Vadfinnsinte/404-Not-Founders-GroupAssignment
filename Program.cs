using _404_not_founders.Menus;
using _404_not_founders.Models;

namespace _404_not_founders
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            bool loggedIn = false;
            string currentUser = null;

            List<User> users = new List<User>();

            while (running)
            {
                if (!loggedIn)
                {
                    int choice = MenuHelper.ShowLoginRegisterMenu();
                    switch (choice)
                    {
                        case 1:
                            // Login-logik (enkel, ingen hashing!)
                            Console.Write("Användarnamn: ");
                            string loginUsername = Console.ReadLine();
                            Console.Write("Lösenord: ");
                            string loginPassword = Console.ReadLine();

                            var user = users.Find(u => u.Username == loginUsername);

                            if (user != null && user.Password == loginPassword)
                            {
                                Console.WriteLine("Inloggning lyckades!");
                                currentUser = loginUsername;
                                loggedIn = true;
                                Console.ReadKey(); // Vänta på knapptryck!
                            }
                            else
                            {
                                Console.WriteLine("Fel användarnamn eller lösenord.");
                                Console.ReadKey(); // Vänta på knapptryck!
                            }
                            break;
                        case 2:
                            // Register
                            User newUser = MenuHelper.RegisterMenu();

                            // Kontroll: Finns användaren redan?
                            if (users.Exists(u => u.Username == newUser.Username))
                            {
                                Console.WriteLine("Användarnamnet är redan taget. Försök igen.");
                                Console.ReadKey(); // Vänta på knapptryck!
                            }
                            else
                            {
                                users.Add(newUser);
                                Console.WriteLine("Registrering lyckades!");
                                Console.ReadKey(); // Vänta på knapptryck!
                            }
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
