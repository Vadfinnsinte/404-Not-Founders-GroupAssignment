using _404_not_founders.Models;         // User, Project, Character, World, Storyline
using _404_not_founders.Services;       // UserService, ProjectService
using _404_not_founders.UI;             // ConsoleHelpers, MenuChoises
using Spectre.Console;                  // Meny, markeringar och prompts
using System;                           // Console, ArgumentNullException
using System.Collections.Generic;       // List
using System.Linq;                      // FirstOrDefault
using System.Text;                      // StringBuilder
using System.Threading.Tasks;           // Async/await

namespace _404_not_founders.Menus
{
    public class LoggedInMenu
    {
        private readonly MenuHelper _menuHelper;
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;

        public LoggedInMenu(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _menuHelper = new MenuHelper(_userService, _projectService);
        }

        // Sätter inloggad användare i menyn
        public void SetCurrentUser(User? user) => _currentUser = user;

        // Async meny för när man är inloggad, och kan hantera/projekt/redigera m.m.
        public async Task<(bool loggedIn, string currentUser)> ShowLoggedInMenu(bool loggedIn, string currentUser)
        {
            bool running = true;
            while (running)
            {
                if (_currentUser == null)
                {
                    Console.Clear();
                    loggedIn = false;
                    currentUser = null;
                    return (loggedIn, currentUser);
                }

                Console.Clear();
                ConsoleHelpers.Info("Main menu");
                AnsiConsole.MarkupLine($"User: {_currentUser.Username}");

                var choice = MenuChoises.Menu("What would you like to do?",
                                      "Add project",
                                      "Handle project",
                                      "Latest project",
                                      "Edit account",
                                      "Log out");

                switch (choice)
                {
                    case "Log out":
                        ConsoleHelpers.Result(true, "Logging out...");
                        ConsoleHelpers.DelayAndClear();
                        loggedIn = false;
                        currentUser = null;
                        _currentUser = null;
                        running = false;
                        break;

                    case "Add project":
                        ConsoleHelpers.Info("[grey italic]Press E to go back or B to return to the previous step[/]");

                        var newProject = new Project();
                        var addedProject = await newProject.Add(_currentUser, _userService);
                        if (addedProject == null)
                        {
                            ConsoleHelpers.Info("Project creation cancelled.");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }
                        ConsoleHelpers.DelayAndClear();

                        var projectMenu = new ProjectChoisesMenu(_currentUser, addedProject, _userService, _projectService);
                        await projectMenu.ProjectEditMenu(addedProject, _userService);
                        break;

                    case "Handle project":
                        var projectMenu2 = new ProjectChoisesMenu(_currentUser, null, _userService, _projectService);
                        await projectMenu2.ShowProjectMenu();
                        break;

                    case "Latest project":
                        var projectMenu3 = new ProjectChoisesMenu(_currentUser, null, _userService, _projectService);
                        var username = currentUser;
                        var user = _userService.Users.FirstOrDefault(u => u.Username == username);

                        if (user != null)
                            await projectMenu3.ShowLastProjectMenu(user);
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Could not find current user.[/]");
                            Console.ReadKey(true);
                        }
                        break;

                    case "Edit account":
                        var editResult = await EditUserMenu(currentUser);
                        currentUser = editResult.currentUser;
                        break;
                }
            }
            return (loggedIn, currentUser);
        }

        public async Task<(bool finished, string currentUser)> EditUserMenu(string currentUser)
        {
            if (_currentUser == null)
            {
                ConsoleHelpers.Result(false, "No user logged in!");
                ConsoleHelpers.DelayAndClear();
                return (false, currentUser);
            }

            // Call user edit directly. Replace with async logic if needed.
            var (finished, updatedUser) = _currentUser.EditUser(_userService, currentUser);

            if (finished)
            {
                ConsoleHelpers.Info($"New {updatedUser}.");
                ConsoleHelpers.DelayAndClear();
            }

            return (finished, updatedUser);
        }
    }
}
