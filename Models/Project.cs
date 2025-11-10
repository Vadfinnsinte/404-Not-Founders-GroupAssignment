using Spectre.Console;

namespace _404_not_founders.Models
{
    public class Project
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime dateOfCreation;

        public List<Storyline> Storylines;

        // Static list to hold all projects, needs JSON to store to later
        public List<Project> AllProjects = new List<Project>();


        public void Add()
        {
            // Prompt user for project details
            string addProjectName = AnsiConsole.Ask<string>("[#FFA500]Enter project title: ");
            string addProjectDescription = AnsiConsole.Ask<string>("[#FFA500]Enter project title: ");

            // Create new project and add to list
            var newProject = new Project
            {
                title = addProjectName,
                description = addProjectDescription,
                dateOfCreation = DateTime.Now,
                Storylines = new List<Storyline>()
            };
            AllProjects.Add(newProject);

            //Sends user to where they can add a storyline to their new project
            Change();

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
