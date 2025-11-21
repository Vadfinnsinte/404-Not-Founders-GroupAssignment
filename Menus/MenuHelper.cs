using _404_not_founders.Models;
using _404_not_founders.Services;
using _404_not_founders.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace _404_not_founders.Menus
{
    public class MenuHelper
    {
  
        public const string MainTitleColor = "#FFA500";
        private readonly UserService _userService;
        private User? _currentUser;
        private readonly ProjectService _projectService;

        public MenuHelper(UserService userService, ProjectService projectService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        public void SetCurrentUser(User user) => _currentUser = user;



        // Add this getter so other classes can read the currently logged-in user
        public User? CurrentUser => _currentUser;

        // ----- UI-HELPERS OCH GEMENSAM LOGIK -----

        /// Meny med Orange highlight (aktivt) och vita val (inaktivt)
        public static string Menu(string title, params string[] choices) =>
             AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#{MainTitleColor}]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(choices)
                    .UseConverter(choice => $"[white]{choice}[/]")
            );

       
        // Add this method to the MenuHelper class
      

        // ----- HUVUDMENY (login/reg/avsluta) -----
        public void ShowLoginRegisterMenu(List<User> users, out bool loggedIn, out string currentUser, ref bool running)
        {
            loggedIn = false; currentUser = null;
            while (running)
            {
                Console.Clear();
                var choice = Menu("Choose an option", "Log in", "Sign up", "Exit");
                if (choice == "Exit") { running = false; return; }
                if (choice == "Log in" && LoginMenu(users, out currentUser))
                {
                    loggedIn = true;
                    Console.Clear();
                    break;
                }
                string newUser = null;
                if (choice == "Sign up" && User.RegisterUser(users, out newUser, _userService))
                {
                    loggedIn = true;
                    currentUser = newUser;
                    _currentUser = users.FirstOrDefault(u => u.Username == newUser);
                    Console.Clear();
                    break;
                }
            }
        }


        // ----- INLOGGNING (med stegbaserad backa och återanvändbar vy) -----
        public bool LoginMenu(List<User> users, out string loggedInUser)
        {
            loggedInUser = null;
            string username = "", password = ""; int step = 0;
            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("Log in");
                ConsoleHelpers.InputInstruction(true);
                if (step >= 1)
                    AnsiConsole.MarkupLine($"[grey]Username:[/] [#FFA500]{username}[/]");

                string value = step == 0
                    ? ConsoleHelpers.AskInput("[#FFA500]Username:[/]")
                    : ConsoleHelpers.AskInput("[#FFA500]Password:[/]", true);

                if (value == null) return false;
                if (value.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    if (step > 0) { if (step == 1) username = ""; step--; }
                    continue;
                }
                if (step == 0) { username = value; step++; }
                else if (step == 1) { password = value; step++; }

                if (step == 2)
                {
                    var user = users.Find(u => u.Username == username);
                    if (user != null && user.Password == password)
                    {
                        ConsoleHelpers.Result(true, "Logging in…");
                        ConsoleHelpers.DelayAndClear();
                        loggedInUser = username;
                        _currentUser = user;
                        return true;
                    }
                    ConsoleHelpers.Result(false, "Wrong username or password!");
                    ConsoleHelpers.DelayAndClear(1200);
                    password = ""; step = 1;
                }
            }
        }

        //public void StorylineMenu(Project project)
        //{
        //    while (true)
        //    {
        //        var choice = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("[bold]Storylines[/]")
        //                .AddChoices("Add Storyline", "Show Storylines", "Edit Storyline", "Remove Storyline", "Back")
        //                .HighlightStyle(Color.Orange1));

        //        switch (choice)
        //        {
        //            case "Add Storyline":
        //                AddStorylineToProject(project);
        //                break;
        //            case "Show Storylines":
        //                project.ShowAllStorylines();
        //                AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
        //                AnsiClearHelper.WaitForKeyAndClear();
        //                break;

        //            case "Edit Storyline":
        //                EditStoryline(project);
        //                break;
        //            case "Remove Storyline":
        //                if (project.Storylines == null || project.Storylines.Count == 0)
        //                {
        //                    AnsiConsole.MarkupLine("[grey]No Storylines to remove.[/]");
        //                    ConsoleHelpers.DelayAndClear();
        //                    break;
        //                }

        //                var storylineChoices = project.Storylines.Select(w => w.Title).ToList();

        //                storylineChoices.Add("Back to Menu");

        //                var selectedStoryline = AnsiConsole.Prompt(
        //                    new SelectionPrompt<string>()
        //                        .Title("[#FFA500]Choose storyline to Remove[/]")
        //                        .HighlightStyle(new Style(Color.Orange1))
        //                        .AddChoices(storylineChoices));

        //                if (selectedStoryline == "Back to Menu")
        //                {
        //                    break;
        //                }

        //                var storylineToDelete = project.Storylines.First(w => w.Title == selectedStoryline);

        //                storylineToDelete.DeleteStoryline(project, _userService);
        //                break;

        //            case "Back":
        //                //ProjectEditMenu(project);
        //                break;
        //        }
        //    }
        //}
        //private void AddStorylineToProject(Project project)
        //{
        //    Console.Clear();
        //    ConsoleHelpers.Info("Create new storyline");


        //    var title = AnsiConsole.Ask<string>("[#FFA500]Title:[/]");
        //    var synopsis = AnsiConsole.Ask<string>("[#FFA500]Synopsis (short description):[/]");
        //    var theme = AnsiConsole.Ask<string>("[#FFA500]Theme:[/]");
        //    var genre = AnsiConsole.Ask<string>("[#FFA500]Genre:[/]");
        //    var story = AnsiConsole.Ask<string>("[#FFA500]Story content:[/]");
        //    var ideaNotes = AnsiConsole.Ask<string>("[#FFA500]Idea notes:[/]");
        //    var otherInfo = AnsiConsole.Ask<string>("[#FFA500]Other information:[/]");

        //    project.Storylines ??= new List<Storyline>();
        //    int maxOrder = project.Storylines.Count + 1;
        //    int orderInProject;

        //    if (maxOrder == 1)
        //    {

        //        orderInProject = 1;
        //    }
        //    else
        //    {
        //        while (true)
        //        {
        //            var input = AnsiConsole.Ask<string>(
        //                $"[#FFA500]Order in project (1 - {maxOrder}):[/]");

        //            if (int.TryParse(input, out orderInProject) &&
        //                orderInProject >= 1 && orderInProject <= maxOrder)
        //                break;

        //            AnsiConsole.MarkupLine("[red]Please enter a number within the range.[/]");
        //        }
        //    }

        //    Console.WriteLine();
        //    ConsoleHelpers.Info("Storyline summary:");
        //    AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{title}[/]");
        //    AnsiConsole.MarkupLine($"[grey]Synopsis:[/] {synopsis}");
        //    AnsiConsole.MarkupLine($"[grey]Theme:[/] {theme}");
        //    AnsiConsole.MarkupLine($"[grey]Genre:[/] {genre}");
        //    AnsiConsole.MarkupLine($"[grey]Story:[/] {story}");
        //    AnsiConsole.MarkupLine($"[grey]Idea notes:[/] {ideaNotes}");
        //    AnsiConsole.MarkupLine($"[grey]Other info:[/] {otherInfo}");

        //    Console.WriteLine();
        //    var confirm = AnsiConsole.Prompt(
        //        new SelectionPrompt<string>()
        //            .Title("[#FFA500]Are you happy with this storyline?[/]")
        //            .HighlightStyle(new Style(Color.Orange1))
        //            .AddChoices("Yes", "No (Start over)", "Exit"));

        //    if (confirm == "Exit")
        //    {

        //        ConsoleHelpers.DelayAndClear();
        //        return;
        //    }

        //    if (confirm == "No (Start over)")
        //    {

        //        AddStorylineToProject(project);
        //        return;
        //    }

        //    foreach (var sl in project.Storylines
        //        .Where(sl => sl.orderInProject >= orderInProject))
        //    {
        //        sl.orderInProject++;
        //    }
        //    var s = new Storyline
        //    {
        //        Title = title,
        //        Synopsis = synopsis,
        //        Theme = theme,
        //        Genre = genre,
        //        Story = story,
        //        IdeaNotes = ideaNotes,
        //        OtherInfo = otherInfo,
        //        orderInProject = orderInProject,
        //        dateOfLastEdit = DateTime.Now
        //    };

        //    project.Storylines.Add(s);
        //    _userService.SaveUserService();

        //    ConsoleHelpers.Info("Storyline created!");
        //    ConsoleHelpers.DelayAndClear();
        //}
        //private void EditStoryline(Project project)
        //{
        //    var original = SelectStoryline(project, "Choose storyline to edit");
        //    if (original == null) return;


        //    var temp = new Storyline
        //    {
        //        Title = original.Title,
        //        Synopsis = original.Synopsis,
        //        Theme = original.Theme,
        //        Genre = original.Genre,
        //        Story = original.Story,
        //        IdeaNotes = original.IdeaNotes,
        //        OtherInfo = original.OtherInfo,
        //        orderInProject = original.orderInProject,
        //        dateOfLastEdit = original.dateOfLastEdit
        //    };

        //    while (true)
        //    {
        //        Console.Clear();
        //        ConsoleHelpers.Info($"Edit storyline: [#FFA500]{temp.Title}[/]");

        //        var choice = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("What do you want to change?")
        //                .HighlightStyle(new Style(Color.Orange1))
        //                .AddChoices(
        //                    "Title",
        //                    "Synopsis",
        //                    "Theme",
        //                    "Genre",
        //                    "Story",
        //                    "Idea notes",
        //                    "Other info",
        //                    "Order in project",
        //                    "Done")
        //                .HighlightStyle(Color.Orange1));


        //        string PromptNonEmpty(string prompt)
        //        {
        //            while (true)
        //            {
        //                var value = AnsiConsole.Ask<string>(prompt);
        //                if (!string.IsNullOrWhiteSpace(value))
        //                    return value;

        //                AnsiConsole.MarkupLine("[red]Value cannot be empty.[/]");
        //            }
        //        }

        //        if (choice == "Done")
        //        {
        //            Console.Clear();
        //            ConsoleHelpers.Info("Storyline summary:");
        //            AnsiConsole.MarkupLine($"[grey]Title:[/] [#FFA500]{temp.Title}[/]");
        //            AnsiConsole.MarkupLine($"[grey]Synopsis:[/] {temp.Synopsis}");
        //            AnsiConsole.MarkupLine($"[grey]Theme:[/] {temp.Theme}");
        //            AnsiConsole.MarkupLine($"[grey]Genre:[/] {temp.Genre}");
        //            AnsiConsole.MarkupLine($"[grey]Story:[/] {temp.Story}");
        //            AnsiConsole.MarkupLine($"[grey]Idea notes:[/] {temp.IdeaNotes}");
        //            AnsiConsole.MarkupLine($"[grey]Other info:[/] {temp.OtherInfo}");
        //            AnsiConsole.MarkupLine($"[grey]Order in project:[/] {temp.orderInProject}");

        //            Console.WriteLine();
        //            var confirm = AnsiConsole.Prompt(
        //                new SelectionPrompt<string>()
        //                    .Title("[#FFA500]Are you happy with this storyline?[/]")
        //                    .HighlightStyle(new Style(Color.Orange1))
        //                    .AddChoices("Yes", "No (Start over)", "Exit"));

        //            if (confirm == "Exit")
        //            {
        //                ConsoleHelpers.DelayAndClear();
        //                return;
        //            }

        //            if (confirm == "No (Start over)")
        //            {
        //                temp.Title = original.Title;
        //                temp.Synopsis = original.Synopsis;
        //                temp.Theme = original.Theme;
        //                temp.Genre = original.Genre;
        //                temp.Story = original.Story;
        //                temp.IdeaNotes = original.IdeaNotes;
        //                temp.OtherInfo = original.OtherInfo;
        //                temp.orderInProject = original.orderInProject;
        //                continue;
        //            }

        //            if (confirm == "Yes")
        //            {
        //                original.Title = temp.Title;
        //                original.Synopsis = temp.Synopsis;
        //                original.Theme = temp.Theme;
        //                original.Genre = temp.Genre;
        //                original.Story = temp.Story;
        //                original.IdeaNotes = temp.IdeaNotes;
        //                original.OtherInfo = temp.OtherInfo;
        //                original.dateOfLastEdit = DateTime.Now;

        //                int oldOrder = original.orderInProject;
        //                int newOrder = temp.orderInProject;

        //                if (newOrder != oldOrder)
        //                {
        //                    if (newOrder < oldOrder)
        //                    {
        //                        foreach (var sl in project.Storylines.Where(s =>
        //                                     s != original &&
        //                                     s.orderInProject >= newOrder &&
        //                                     s.orderInProject < oldOrder))
        //                        {
        //                            sl.orderInProject++;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        foreach (var sl in project.Storylines.Where(s =>
        //                                     s != original &&
        //                                     s.orderInProject <= newOrder &&
        //                                     s.orderInProject > oldOrder))
        //                        {
        //                            sl.orderInProject--;
        //                        }
        //                    }

        //                    original.orderInProject = newOrder;
        //                }

        //                _userService.SaveUserService();
        //                ConsoleHelpers.Info("Storyline updated!");
        //                ConsoleHelpers.DelayAndClear();
        //                return;
        //            }
        //        }


        //        switch (choice)
        //        {
        //            case "Title":
        //                temp.Title = PromptNonEmpty("[#FFA500]New title:[/]");
        //                break;
        //            case "Synopsis":
        //                temp.Synopsis = PromptNonEmpty("[#FFA500]New synopsis:[/]");
        //                break;
        //            case "Theme":
        //                temp.Theme = PromptNonEmpty("[#FFA500]New theme:[/]");
        //                break;
        //            case "Genre":
        //                temp.Genre = PromptNonEmpty("[#FFA500]New genre:[/]");
        //                break;
        //            case "Story":
        //                temp.Story = PromptNonEmpty("[#FFA500]New story content:[/]");
        //                break;
        //            case "Idea notes":
        //                temp.IdeaNotes = PromptNonEmpty("[#FFA500]New idea notes:[/]");
        //                break;
        //            case "Other info":
        //                temp.OtherInfo = PromptNonEmpty("[#FFA500]New other information:[/]");
        //                break;
        //            case "Order in project":
        //                int maxOrder = project.Storylines.Count;
        //                while (true)
        //                {
        //                    var input = AnsiConsole.Ask<string>(
        //                        $"[#FFA500]New order in project (1 - {maxOrder}):[/]");
        //                    if (int.TryParse(input, out var newOrder) &&
        //                        newOrder >= 1 && newOrder <= maxOrder)
        //                    {
        //                        temp.orderInProject = newOrder;
        //                        break;
        //                    }

        //                    AnsiConsole.MarkupLine("[red]Please enter a number within the range.[/]");
        //                }
        //                break;
        //        }
        //    }
        //}
        //private Storyline? SelectStoryline(Project project, string title)
        //{
        //    if (project.Storylines == null || project.Storylines.Count == 0)
        //    {
        //        AnsiConsole.MarkupLine("[grey]No storylines yet.[/]");
        //        Console.ReadKey(true);
        //        return null;
        //    }

        //    var sorted = project.Storylines
        //        .OrderBy(s => s.orderInProject)
        //        .ToList();

        //    return AnsiConsole.Prompt(
        //        new SelectionPrompt<Storyline>()
        //            .Title($"[#FFA500]{title}[/]")
        //            .HighlightStyle(new Style(Color.Orange1))
        //            .AddChoices(sorted)
        //            .UseConverter(s => $"{s.orderInProject}. {s.Title}"));
        //}
        private World? SelectWorld(Project project, string title)
        {
            if (project.Worlds == null || project.Worlds.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No worlds yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            var sorted = project.Worlds.ToList();

            return AnsiConsole.Prompt(
                new SelectionPrompt<World>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(sorted)
                    .UseConverter(w => w.Name));
        }





        // ----- WORLD MENU -----

        public void WorldMenu(User loggedInUser, Project currentProject)
        {

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info("World Menu");
                var choice = Menu("",
                    "Add World",
                    "Show Worlds",
                    "Edit World",
                    "Remove World",
                    "Back");

                switch (choice)
                {
                    case "Add World":
                        World newWorld = new World();
                        newWorld.Add(loggedInUser, currentProject, _userService);
                        break;

                    case "Show Worlds":
                        currentProject.ShowAllWorlds();
                        AnsiClearHelper.WaitForKeyAndClear();
                        break;
                    case "Edit World":
                        if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No worlds in this project yet.[/]");
                            Console.ReadKey(true);
                            break;
                        }

                        var worldToEdit = SelectWorld(currentProject, "Choose world to edit");
                        if (worldToEdit != null)
                            worldToEdit.EditWorld(_userService);
                        break;
                    case "Remove World":
                        // Check if there are any worlds to remove
                        if (currentProject.Worlds == null || currentProject.Worlds.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[grey]No worlds to remove.[/]");
                            ConsoleHelpers.DelayAndClear();
                            break;
                        }

                        // Create a list of world names for selection
                        var worldChoices = currentProject.Worlds.Select(w => w.Name).ToList();

                        // Add a "Back to Menu" option
                        worldChoices.Add("Back to Menu");

                        // Show the selection prompt
                        var selectedWorld = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[#FFA500]Choose World to Remove[/]")
                                .HighlightStyle(new Style(Color.Orange1))
                                .AddChoices(worldChoices));

                        // If the user selected "Back to Menu", go back to the previous menu
                        if (selectedWorld == "Back to Menu")
                        {
                            break;
                        }

                        // Find the world object based on the selected name
                        var worldToDelete = currentProject.Worlds.First(w => w.Name == selectedWorld);

                        // Delete the selected world
                        worldToDelete.DeleteWorld(currentProject, _userService);
                        break;

                    case "Back":
                        //ProjectEditMenu(currentProject);
                        break;
                }
            }
        }
    }
}

