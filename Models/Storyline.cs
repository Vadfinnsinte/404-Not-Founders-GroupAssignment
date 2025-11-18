

using _404_not_founders.UI;

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
        public void Delete()
        {
            Console.WriteLine("Coming soon");
        }
    }
}
