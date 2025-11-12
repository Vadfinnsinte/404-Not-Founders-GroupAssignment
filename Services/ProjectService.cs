using _404_not_founders.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _404_not_founders.Services
{
    public class ProjectService
    {
        private readonly UserService _userService;
        public ProjectService(UserService userService) => _userService = userService;

        public List<Project> GetAll(User user)
            => user.Projects.OrderByDescending(p => p.dateOfCreation).ToList();

        // söker projekt baserat på sökterm i titel eller beskrivning
        public List<Project> Search(User user, string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return GetAll(user);
            term = term.Trim();

            return user.Projects
                .Where(p =>
                    p.title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    p.description.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.dateOfCreation)
                .ToList();
        }

        //sätter senaste valda projekt för användaren
        public void SetLastSelected(User user, Guid projectId)
        {
            user.LastSelectedProjectId = projectId;
            _userService.SaveUserService();
        }
    }
}