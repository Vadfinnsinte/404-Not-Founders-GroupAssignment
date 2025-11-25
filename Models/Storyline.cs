

using _404_not_founders.Services;
using _404_not_founders.UI;
using Spectre.Console;

namespace _404_not_founders.Models
{
    public class Storyline
    {
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Theme { get; set; }
        public string Genre { get; set; }
        public string Story {  get; set; }
        public string IdeaNotes { get; set; }
        public string OtherInfo { get; set; }
        public int orderInProject { get; set; }
        public List<World> worlds;
        public List<Character> chracters;
        public DateTime dateOfLastEdit;



        public void Add()
        {
            Console.WriteLine("Coming soon");
        }
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void DeleteStoryline(Project project, UserService userService)
        {
            Console.Clear();

            // Ask for confirmation
            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{this.Title}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                // Remove the world from the project's worlds list
                if (project.Storylines.Contains(this))
                {
                    project.Storylines.Remove(this);

                    // Save changes
                    userService.SaveUserService();

                    AnsiConsole.MarkupLine($"The storyline '[orange1]{this.Title}[/]' has been deleted!");
                    Thread.Sleep(1200);
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]Error: Storyline not found.[/]");
                    Thread.Sleep(1200);
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
                Thread.Sleep(1200);
            }
        }
    }
}
