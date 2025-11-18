using System;
using System.Collections.Generic;
using System.Linq;
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
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<Storyline> Storylines;
        

        public List<Character> Characters { get; set; } = new List<Character>();

        public Project Add(User currentUser, UserService userService)
        {
            // Prompt user for project details
            string addProjectName = AnsiConsole.Ask<string>("[#FFA500]Enter project title:[/]");
            string addProjectDescription = AnsiConsole.Ask<string>("[#FFA500]Enter project description:[/]");

            // Create new project
            var newProject = new Project
            {
                title = addProjectName,
                description = addProjectDescription,
                dateOfCreation = DateTime.Now,
                Storylines = new List<Storyline>(),
                Characters = new List<Character>()
             };

            // Add project to user's project list and save
            currentUser.Projects.Add(newProject);
            userService.SaveUserService();
            return (newProject);

        }

        public void AddCharacter(Character character, UserService userService)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));

            Characters ??= new List<Character>();

            if (Characters.Any(c => string.Equals(c.Names, character.Names, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A character with the name '{character.Names}' already exists in project '{title}'.");
            }

            Characters.Add(character);

            // Persist entire userstore (caller is expected to supply the same UserService instance used by the app)
            userService?.SaveUserService();
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
