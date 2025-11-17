

using _404_not_founders.Menus;
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
        public DateTime dateOfLastEdit { get; set; } = DateTime.Now;



        public void Add()
        {
           
        }
        public void Show()
        {
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Synopsis: {Synopsis}");
            Console.WriteLine($"Theme: {Theme}");
            Console.WriteLine($"Genre: {Genre}");
            Console.WriteLine($"Story: {Story}");
            Console.WriteLine($"Idea notes: {IdeaNotes}");
            Console.WriteLine($"Other info: {OtherInfo}");
            Console.WriteLine($"Last edit: {dateOfLastEdit:g}");
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
