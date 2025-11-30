using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _404_not_founders.Menus
{
    public class LoggedInMenu
    {
        private readonly MenuHelper _menuHelper;
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;

        // Constructor to initialize necessary services
        public LoggedInMenu(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _menuHelper = new MenuHelper(_userService, _projectService);


        }
        // Method that sets the current user or null if no user is logged in
        public void SetCurrentUser(User? user) => _currentUser = user;

        // Method to display the logged-in menu
        public void ShowLoggedInMenu(ref bool loggedIn, ref string currentUser)
        {
            bool running = true;
            while (running)
            {
                // Check so that there is a logged in user and if not logs out
                if (_currentUser == null)
                {
                    Console.Clear();
                    loggedIn = false;
                    currentUser = null;
                    return;
                }

                // Display the main menu
                Console.Clear();
                ConsoleHelpers.Info($"Main menu");
                AnsiConsole.MarkupLine($"User: {_currentUser.Username}");
                var choice = MenuChoises.Menu("What would you like to do?",
                                  "Add project",
                                  "Handle project",
                                  "Latest project",
                                  "Edit account",
                                  "Log out");

                // Handle the main menu choices
                switch (choice)
                {
                    case "Log out":
                        // Log out the current user
                        ConsoleHelpers.Result(true, "Logging out...");
                        ConsoleHelpers.DelayAndClear();
                        loggedIn = false;
                        currentUser = null;
                        _currentUser = null;
                       
                        break;
                    case "Add project":
                        // Create a new project and add it to the current user
                        ConsoleHelpers.Info("[grey italic]Press E to go back or B to return to the previous step[/]");
                        var newProject = new Project();
                        var addedProject = newProject.Add(_currentUser, _userService);
                        // Informs the user if project creation was cancelled
                        if (addedProject == null)
                        {
                            ConsoleHelpers.Info("Project creation cancelled.");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }
                        ConsoleHelpers.DelayAndClear();

                        // Opens the project edit menu for the newly created project
                        var projectMenu = new ProjectChoisesMenu(_currentUser, _projectService, _userService);
                        projectMenu.ProjectEditMenu(addedProject);
                        break;
                    case "Handle project":
                        // Sends the user to the project handling menu
                        var projectMenu2 = new ProjectChoisesMenu(_currentUser, _projectService, _userService);
                        projectMenu2.ShowProjectMenu();
                        break;
                    case "Latest project":
                        // Creates a new object to handle project choices and finds the current user
                        var projectMenu3 = new ProjectChoisesMenu(_currentUser, _projectService, _userService);
                        var username = currentUser;
                        var user = _userService.Users
                        .FirstOrDefault(u => u.Username == username);

                        // Make sure user is not null before showing last project menu
                        if (user != null)
                            projectMenu3.ShowLastProjectMenu(user);
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Could not find current user.[/]");
                            Console.ReadKey(true);
                        }
                        break;
                    case "Edit account":
                        // Opens the user account edit menu
                        EditUserMenu(ref currentUser);
                        break;
                }
            }

        }
        public void EditUserMenu(ref string currentUser)
        {
            if (_currentUser == null)
            {
                ConsoleHelpers.Result(false, "No user logged in!");
                ConsoleHelpers.DelayAndClear();
                return;
            }

            bool finished = _currentUser.EditUser(_userService, ref currentUser);
            if (finished)
            {
                // Visa feedback endast om du gick via "Tillbaka"
                ConsoleHelpers.Info($"New {_currentUser.Username}.");
                ConsoleHelpers.DelayAndClear();
            }
            // Annars – ingen feedback!
        }


    }
}
