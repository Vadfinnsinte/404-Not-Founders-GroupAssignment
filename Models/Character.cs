using System;
using _404_not_founders.Menus;
using _404_not_founders.Services;
using Spectre.Console;
using _404_not_founders.UI;
using System.Collections.Generic;
using System.Linq;

namespace _404_not_founders.Models
{
    public class Character
    {
        public string Name { get; set; }
        public string Race { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public string OtherInfo { get; set; }

        private const string MainTitleColor = "#FFA500";

        public string ChracterMenu1(string title, params string[] choices) =>
            AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[#{MainTitleColor}]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(choices)
                    .UseConverter(choice => $"[white]{choice}[/]")
            );

       
        public void ChracterMenu2(UserService userService, ProjectService projectService, MenuHelper menuHelper, Project currentProject)
        {
            
            User? currentUser = menuHelper.CurrentUser;
            var choice = ChracterMenu1("Character Menu", "Add Character", "Show Character", "Change Character", "Delete Character", "Back to Main Menu");
            switch (choice)
            {
                case "Add Character":
                    Add(currentUser, projectService, userService, menuHelper);
                    ChracterMenu2(userService, projectService, menuHelper, currentProject);
                    break;
                case "Show Character":
                    // Show characters from the actual project
                    ShowCharacters(currentProject);
                    ChracterMenu2(userService, projectService, menuHelper, currentProject);
                    break;
                case "Change Character":
                    Change();
                    ChracterMenu2(userService, projectService, menuHelper, currentProject);
                    break;
                case "Delete Character":
                    if (currentProject.Characters == null || currentProject.Characters.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[grey]No Characters to remove.[/]");
                        MenuHelper.DelayAndClear();
                        break;
                    }

                    var characterChoices = currentProject.Characters.Select(w => w.Name).ToList();

                    characterChoices.Add("Back to Menu");

                    var selectedCharacter = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Choose character to remove[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices(characterChoices));

                    if (selectedCharacter == "Back to Menu")
                    {
                        break;
                    }

                    var characterToDelete = currentProject.Characters.First(w => w.Name == selectedCharacter);

                    characterToDelete.DeleteCharacter(currentProject, userService);

                    ChracterMenu2(userService, projectService, menuHelper, currentProject);
                    break;
                case "Back to Main Menu": 
                    bool loggedIn = true;
                    bool running = true;
                    string username = currentUser?.Username;
                    menuHelper.ShowLoggedInMenu(ref loggedIn, ref username, ref running);  // should go to project menu
                    if (currentUser != null) currentUser.Username = username;
                    break;
            }
        }

       

        public void Add(User currentUser, ProjectService projectService, UserService userService, MenuHelper menuHelper)
        {
            // Lokala variabler för fältet som byggs upp stegvis
            string name = "", race = "", description = "", gender = "", characterClass = "", otherInfo = "";
            int age = 0, level = 0;
            // step styr vilken input som efterfrågas; 0..8 där 8 = bekräftelse
            int step = 0;

            while (true)
            {
                Console.Clear();
                MenuHelper.Info("Create New Character");
                MenuHelper.InputInstruction(true);

                // Visa redan ifyllda fält som kontext för användaren
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{name}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Race:[/] [#FFA500]{race}[/]");
                if (step >= 3) AnsiConsole.MarkupLine($"[grey]Description:[/] [#FFA500]{description}[/]");
                if (step >= 4) AnsiConsole.MarkupLine($"[grey]Gender:[/] [#FFA500]{gender}[/]");
                if (step >= 5) AnsiConsole.MarkupLine($"[grey]Age:[/] [#FFA500]{age}[/]");
                if (step >= 6) AnsiConsole.MarkupLine($"[grey]Level:[/] [#FFA500]{level}[/]");
                if (step >= 7) AnsiConsole.MarkupLine($"[grey]Class:[/] [#FFA500]{characterClass}[/]");
                if (step >= 8) AnsiConsole.MarkupLine($"[grey]Other info:[/] [#FFA500]{otherInfo}[/]");

                string input;

                switch (step)
                {
                    case 0:
                        // Namn: enkel textinput
                        Console.Write("Name: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 1:
                        // Race: enkel textinput
                        Console.Write("Race: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 2:
                        // Description: längre text, inga särskilda valideringar här
                        Console.Write("Description: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 3:
                        // Gender: fri text; överväg enum eller valmeny om du vill begränsa alternativ
                        Console.Write("Gender: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 4:
                        // Age: tillåter tomt = 0, validerar heltal
                        Console.Write("Age (leave empty for 0): ");
                        input = MenuHelper.ReadBackOrExit();
                        if (input != "E" && input != "B")
                        {
                            if (string.IsNullOrWhiteSpace(input)) { age = 0; step++; continue; }
                            if (!int.TryParse(input.Trim(), out age))
                            {
                                Console.WriteLine("Invalid number — please enter an integer or leave empty.");
                                Console.WriteLine("Press any key to try again...");
                                Console.ReadKey(true);
                                continue;
                            }
                        }
                        break;
                    case 5:
                        // Level: samma logik som age
                        Console.Write("Level (leave empty for 0): ");
                        input = MenuHelper.ReadBackOrExit();
                        if (input != "E" && input != "B")
                        {
                            if (string.IsNullOrWhiteSpace(input)) { level = 0; step++; continue; }
                            if (!int.TryParse(input.Trim(), out level))
                            {
                                Console.WriteLine("Invalid number — please enter an integer or leave empty.");
                                Console.WriteLine("Press any key to try again...");
                                Console.ReadKey(true);
                                continue;
                            }
                        }
                        break;
                    case 6:
                        // Class: fri text, överväg att kalla detta "characterClass" för tydlighet
                        Console.Write("Class: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 7:
                        // Other info: valfri extra text
                        Console.Write("Other info: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 8:
                        // Bekräftelsesteg: ja/nej/avsluta
                        Project project = null;
                        var confirm = ChracterMenu1("Confirm character creation", "Yes", "No");
                        if (confirm == "No") { step = 0; continue; }  // Börjar om från början
                        if (confirm == "Yes")
                        {
                            // Hitta målet för karaktären: använder currentUser.LastSelectedProjectId eller första projektet
                            if (currentUser != null)
                            {
                                if (currentUser.Projects != null && currentUser.LastSelectedProjectId.HasValue)
                                {
                                    project = currentUser.Projects.FirstOrDefault(p => p.Id == currentUser.LastSelectedProjectId.Value);
                                }
                                if (project == null && currentUser.Projects != null)
                                {
                                    project = currentUser.Projects.FirstOrDefault();
                                }
                            }

                            if (project == null)
                            {
                                // HÄR: Informerar användaren om att inget projekt är valt
                                Console.WriteLine("No project found. Create or select a project before adding characters.");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey(true);
                                return;
                            }

                            // Bygg ny Character-instans
                            var newCharacter = new Character
                            {
                                Name = name,
                                Race = race,
                                Description = description,
                                Gender = gender,
                                Age = age,
                                Level = level,
                                Class = characterClass,
                                OtherInfo = otherInfo,
                            };

                            try
                            {
                                // Central logik för att lägga till karaktär (validering/duplicering/spara hanteras i Project.AddCharacter)
                                project.AddCharacter(newCharacter, userService);
                            }
                            catch (InvalidOperationException ex)
                            {
                                // Visa fel från Project.AddCharacter och återställ till börja om
                                Console.WriteLine(ex.Message);
                                Console.WriteLine("Press any key to try again...");
                                Console.ReadKey(true);
                                step = 0;
                                continue;
                            }

                            Console.WriteLine();
                            Console.WriteLine($"Character '{name}' created.");
                            Console.WriteLine("Press any key to continue...");
                            MenuHelper.DelayAndClear();

                            return; // Klart
                        }
                        // Om annat resultat, fortsätt loopen
                        continue;
                    default:
                        return;
                }

                // Hantera specialkommandon "E" = Exit, "B" = Back från MenuHelper.ReadBackOrExit
                if (input == "E")
                {
                    // HÄR: Anropar menymetod och skickar null som currentProject — se till att metoden accepterar null
                    Console.Clear();
                    ChracterMenu2(userService, projectService, menuHelper, null);
                }

                if (input == "B")
                {
                    // Gå ett steg bakåt i formuläret (om möjligt)
                    if (step > 0) step--;
                    continue;
                }

                // Spara input i rätt variabel för textstegen
                switch (step)
                {
                    case 0: name = input?.Trim() ?? ""; break;
                    case 1: race = input?.Trim() ?? ""; break;
                    case 2: description = input?.Trim() ?? ""; break;
                    case 3: gender = input?.Trim() ?? ""; break;
                    case 6: characterClass = input?.Trim() ?? ""; break;
                    case 7: otherInfo = input?.Trim() ?? ""; break;
                }

                // Gå till nästa steg
                step++;
            }
        }



        public void ShowCharacters(Project project)
        {
            // Kontrollera att ett projekt faktiskt skickats in
            if (project == null)
            {
                AnsiConsole.MarkupLine("[red]No project provided.[/]");
                MenuHelper.DelayAndClear();
                return;
            }

            // Säkra att projektet har en lista med karaktärer och att den inte är tom
            if (project.Characters == null || project.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No characters in this project.[/]");
                MenuHelper.DelayAndClear();
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title($"[#{MainTitleColor}]Select character to show[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .PageSize(10)
                    .AddChoices(project.Characters)
                    .UseConverter(c => string.IsNullOrWhiteSpace(c.Name) ? "(unnamed)" : c.Name)
            );

            Console.Clear();
            ShowInfoCard.ShowObject(selected);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            MenuHelper.DelayAndClear();
        }
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }
        public void Change()
        {
            Console.WriteLine("Coming soon");
        }
        public void DeleteCharacter(Project project, UserService userService)
        {
            Console.Clear();

            // Ask for confirmation
            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Are you sure you want to delete '[orange1]{this.Name}[/]'?")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices("Yes", "No"));

            if (confirm == "Yes")
            {
                // Remove the character from the project's character list
                if (project.Characters.Contains(this))
                {
                    project.Characters.Remove(this);

                    // Save changes
                    userService.SaveUserService();

                    AnsiConsole.MarkupLine($"The character '[orange1]{this.Name}[/]' has been deleted!");
                    Thread.Sleep(1200);
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]Error: Character not found.[/]");
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
};
