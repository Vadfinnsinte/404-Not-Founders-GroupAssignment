

namespace _404_not_founders.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Project> Projects { get; set; }
        public Guid? LastSelectedProjectId { get; set; }

        public void Add()
        {
            Console.WriteLine("Coming soon");
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
    }
}
