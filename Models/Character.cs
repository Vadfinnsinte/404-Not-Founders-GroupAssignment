
using _404_not_founders.Services;
using Spectre.Console;
using _404_not_founders.UI.Helpers;
using _404_not_founders.UI.Display;

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


        public void Add(User currentUser, ProjectService projectService, UserService userService)
        {
            // Lokala variabler för fältet som byggs upp stegvis
            string name = "", race = "", description = "", gender = "", characterClass = "", otherInfo = "";
            int age = 0, level = 0;
            // step styr vilken input som efterfrågas; 0..8 där 8 = bekräftelse
            int step = 0;
            bool addingCharacter = true;
            while (addingCharacter)
            {
                Console.Clear();
                ConsoleHelpers.Info("Create New Character");
                ConsoleHelpers.InputInstruction(true);

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
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 1:
                        // Race: enkel textinput
                        Console.Write("Race: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 2:
                        // Description: längre text, inga särskilda valideringar här
                        Console.Write("Description: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 3:
                        // Gender: fri text; överväg enum eller valmeny om du vill begränsa alternativ
                        Console.Write("Gender: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 4:
                        // Age: tillåter tomt = 0, validerar heltal
                        Console.Write("Age (leave empty for 0): ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        if (input != "E" && input != "B")
                        {
                            if (string.IsNullOrWhiteSpace(input)) { age = 0; step++; continue; }
                            if (!int.TryParse(input.Trim(), out age))
                            {
                                Console.WriteLine("Invalid number — please enter an integer or leave empty.");
                     
                                continue;
                            }
                        }
                        break;
                    case 5:
                        // Level: samma logik som age
                        Console.Write("Level (leave empty for 0): ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        if (input != "E" && input != "B")
                        {
                            if (string.IsNullOrWhiteSpace(input)) { level = 0; step++; continue; }
                            if (!int.TryParse(input.Trim(), out level))
                            {
                                Console.WriteLine("Invalid number — please enter an integer or leave empty.");
                         ;
                                continue;
                            }
                        }
                        break;
                    case 6:
                        // Class: fri text, överväg att kalla detta "characterClass" för tydlighet
                        Console.Write("Class: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 7:
                        // Other info: valfri extra text
                        Console.Write("Other info: ");
                        input = ConsoleHelpers.ReadBackOrExit();
                        break;
                    case 8:
                        // Bekräftelsesteg: ja/nej/avsluta
                        Project project = null;
                        var confirm = MenuChoises.Menu("Confirm character creation", "Yes", "No");
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
                           
                            ConsoleHelpers.DelayAndClear();

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
                    addingCharacter = false;
                    Console.Clear();
                    return;
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
                ConsoleHelpers.DelayAndClear();
                return;
            }

            // Säkra att projektet har en lista med karaktärer och att den inte är tom
            if (project.Characters == null || project.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No characters in this project.[/]");
                ConsoleHelpers.DelayAndClear();
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title($"[#FFA500]Select character to show[/]")
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
            ConsoleHelpers.DelayAndClear();
        }
        public void Show()
        {
            ShowInfoCard.ShowObject(this);
        }

        public void EditCharacter(Project project, UserService userService)
        {
            var original = SelectCharacter(project, "Choose character to edit");
            if (original == null) return;

            var temp = new Character
            {
                Name = original.Name,
                Race = original.Race,
                Description = original.Description,
                Gender = original.Gender,
                Age = original.Age,
                Level = original.Level,
                Class = original.Class,
                OtherInfo = original.OtherInfo
            };

            while (true)
            {
                Console.Clear();
                ConsoleHelpers.Info($"Edit character: [#FFA500]{temp.Name}[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to change?")
                        .HighlightStyle(new Style(Color.Orange1))
                        .AddChoices(
                            "Name",
                            "Race",
                            "Description",
                            "Gender",
                            "Age",
                            "Level",
                            "Class",
                            "Other info",
                            "Done")
                        .HighlightStyle(Color.Orange1));

                string PromptNonEmpty(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (!string.IsNullOrWhiteSpace(value))
                            return value;

                        AnsiConsole.MarkupLine("[red]Value cannot be empty.[/]");
                    }
                }

                int PromptInt(string prompt)
                {
                    while (true)
                    {
                        var value = AnsiConsole.Ask<string>(prompt);
                        if (int.TryParse(value, out int number))
                            return number;

                        AnsiConsole.MarkupLine("[red]You must enter a number.[/]");
                    }
                }

                if (choice == "Done")
                {
                    Console.Clear();
                    ConsoleHelpers.Info("Character summary:");
                    AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{temp.Name}[/]");
                    AnsiConsole.MarkupLine($"[grey]Race:[/] {temp.Race}");
                    AnsiConsole.MarkupLine($"[grey]Description:[/] {temp.Description}");
                    AnsiConsole.MarkupLine($"[grey]Gender:[/] {temp.Gender}");
                    AnsiConsole.MarkupLine($"[grey]Age:[/] {temp.Age}");
                    AnsiConsole.MarkupLine($"[grey]Level:[/] {temp.Level}");
                    AnsiConsole.MarkupLine($"[grey]Class:[/] {temp.Class}");
                    AnsiConsole.MarkupLine($"[grey]Other info:[/] {temp.OtherInfo}");

                    Console.WriteLine();
                    var confirm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[#FFA500]Are you happy with these changes?[/]")
                            .HighlightStyle(new Style(Color.Orange1))
                            .AddChoices("Yes", "No (Start over)", "Exit"));

                    if (confirm == "Exit")
                    {
                        ConsoleHelpers.DelayAndClear();
                        return;
                    }

                    if (confirm == "No (Start over)")
                    {
                        temp.Name = original.Name;
                        temp.Race = original.Race;
                        temp.Description = original.Description;
                        temp.Gender = original.Gender;
                        temp.Age = original.Age;
                        temp.Level = original.Level;
                        temp.Class = original.Class;
                        temp.OtherInfo = original.OtherInfo;
                        continue;
                    }

                    if (confirm == "Yes")
                    {
                        original.Name = temp.Name;
                        original.Race = temp.Race;
                        original.Description = temp.Description;
                        original.Gender = temp.Gender;
                        original.Age = temp.Age;
                        original.Level = temp.Level;
                        original.Class = temp.Class;
                        original.OtherInfo = temp.OtherInfo;

                        userService.SaveUserService();
                        ConsoleHelpers.Info("Character updated!");
                        ConsoleHelpers.DelayAndClear();
                        return;
                    }
                }

                switch (choice)
                {
                    case "Name":
                        temp.Name = PromptNonEmpty("[#FFA500]New name:[/]");
                        break;
                    case "Race":
                        temp.Race = PromptNonEmpty("[#FFA500]New race:[/]");
                        break;
                    case "Description":
                        temp.Description = PromptNonEmpty("[#FFA500]New description:[/]");
                        break;
                    case "Gender":
                        temp.Gender = PromptNonEmpty("[#FFA500]New gender:[/]");
                        break;
                    case "Age":
                        temp.Age = PromptInt("[#FFA500]New age:[/]");
                        break;
                    case "Level":
                        temp.Level = PromptInt("[#FFA500]New level:[/]");
                        break;
                    case "Class":
                        temp.Class = PromptNonEmpty("[#FFA500]New class:[/]");
                        break;
                    case "Other info":
                        temp.OtherInfo = PromptNonEmpty("[#FFA500]New other info:[/]");
                        break;
                }
            }
        }
        private Character? SelectCharacter(Project project, string title)
        {
            if (project.Characters == null || project.Characters.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No characters yet.[/]");
                Console.ReadKey(true);
                return null;
            }

            return AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title($"[#FFA500]{title}[/]")
                    .HighlightStyle(new Style(Color.Orange1))
                    .AddChoices(project.Characters)
                    .UseConverter(c => $"{c.Name} ({c.Race})"));
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
                    Console.Clear();
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
