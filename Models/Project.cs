using _404_not_founders.Menus;
using _404_not_founders.Services;
using Spectre.Console;

namespace _404_not_founders.Models
{
    public class Project
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime dateOfCreation;

        public List<Storyline> Storylines;


        public void Add(User currentUser, UserService userService)
        {
            // Prompt user for project details
            string addProjectName = AnsiConsole.Ask<string>("[#FFA500]Enter project title: [/]");
            string addProjectDescription = AnsiConsole.Ask<string>("[#FFA500]Enter project description: [/]");

            // Create new project
            var newProject = new Project
            {
                title = addProjectName,
                description = addProjectDescription,
                dateOfCreation = DateTime.Now,
                Storylines = new List<Storyline>()
            };

            // Add project to user's project list and save
            currentUser.Projects.Add(newProject);
            userService.SaveUserService();

        }
        public void Show()
        {
            Console.WriteLine("Coming soon");
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
        public void Filter()
        {
            Console.WriteLine("Coming soon");
        }
        public void Search()
        {
            Console.WriteLine("Coming soon");
        }

    }
}
