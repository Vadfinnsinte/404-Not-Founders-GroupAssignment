using _404_not_founders.Models;


namespace _404_not_founders.Services
{
    public class ProjectService
    {
        private readonly UserService _userService;
        public ProjectService(UserService userService) => _userService = userService;

        public List<Project> GetAll(User user)
            => user.Projects.OrderByDescending(p => p.DateOfCreation).ToList();

        // Search projects by term in title or description
        public List<Project> Search(User user, string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return GetAll(user);
            term = term.Trim();

            return user.Projects
                .Where(p =>
                    p.title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    p.description.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.DateOfCreation)
                .ToList();
        }

        // Set the last selected project for the user
        public void SetLastSelected(User user, Guid projectId)
        {
            user.LastSelectedProjectId = projectId;
            _userService.SaveUserService();
        }
        // Get the last selected project for the user
        public Project? GetLastSelected(User user)
        {
            if (user.LastSelectedProjectId == Guid.Empty)
                return null;

            return user.Projects
                .FirstOrDefault(p => p.Id == user.LastSelectedProjectId);
        }
    }
}