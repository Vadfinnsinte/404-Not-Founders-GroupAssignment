using System;
using _404_not_founders.Menus;
using _404_not_founders.Services;
using Spectre.Console;
using _404_not_founders.UI;


namespace _404_not_founders.Models
{
    public class Character
    {
        public string Names { get; set; }
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

        public void menh()
        {

            MenuHelper.Info("[grey italic]Skriv E för att gå tillbaka eller B för att backa till föregående steg[/]");
            if (MenuHelper.ReadBackOrExit() == "E")
            {
                Environment.Exit(0);

            }
                return;
            if (MenuHelper.ReadBackOrExit() == "B")
            {
                ChracterMenu2();
                return;
            }
        }



        public void ChracterMenu2()
        {
            UserService userService = new UserService();
            MenuHelper menuHelper = new MenuHelper(new UserService());
            var choice = ChracterMenu1("Character Menu", "Add Character", "Show Character", "Change Character", "Delete Character", "Back to Main Menu");
            switch (choice)
            {
               
                case "Add Character":

                    Add();
                    ChracterMenu2();
                    break;
                case "Show Character":
                    Show();
                    ChracterMenu2();
                    break;
                case "Change Character":
                    Change();
                    ChracterMenu2();
                    break;
                case "Delete Character":
                    Delete();
                    ChracterMenu2();
                    break;
                case "Back to Main Menu":
                    bool loggedIn = true;
                    string currentUser = Names; 
                    bool running = true;
                    menuHelper.ShowLoggedInMenu(currentUser, ref loggedIn, ref currentUser, ref running);
                    break;
            }
        }



        public void Add()
        {
            string name = "", race = "", description = "", gender = "", @class = "", otherInfo = "";
            int age = 0, level = 0;
            int step = 0; // 0 = name, 1 = race, 2 = description, 3 = gender, 4 = age, 5 = level, 6 = class, 7 = otherInfo, 8 = confirm

            while (true)
            {
                Console.Clear();
                MenuHelper.Info("Create New Character");
                MenuHelper.InputInstruction(true);

                // Show already filled values for context
                if (step >= 1) AnsiConsole.MarkupLine($"[grey]Name:[/] [#FFA500]{name}[/]");
                if (step >= 2) AnsiConsole.MarkupLine($"[grey]Race:[/] [#FFA500]{race}[/]");
                if (step >= 3) AnsiConsole.MarkupLine($"[grey]Description:[/] [#FFA500]{description}[/]");
                if (step >= 4) AnsiConsole.MarkupLine($"[grey]Gender:[/] [#FFA500]{gender}[/]");
                if (step >= 5) AnsiConsole.MarkupLine($"[grey]Age:[/] [#FFA500]{age}[/]");
                if (step >= 6) AnsiConsole.MarkupLine($"[grey]Level:[/] [#FFA500]{level}[/]");
                if (step >= 7) AnsiConsole.MarkupLine($"[grey]Class:[/] [#FFA500]{@class}[/]");
                if (step >= 8) AnsiConsole.MarkupLine($"[grey]Other info:[/] [#FFA500]{otherInfo}[/]");

                string input;

                switch (step)
                {
                    case 0:
                        Console.Write("Name: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 1:
                        Console.Write("Race: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 2:
                        Console.Write("Description: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 3:
                        Console.Write("Gender: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 4:
                        // Age — allow empty for 0, validate integer
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
                        Console.Write("Class: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 7:
                        Console.Write("Other info: ");
                        input = MenuHelper.ReadBackOrExit();
                        break;
                    case 8:
                        // Confirm
                        var confirm = ChracterMenu1("Confirm character creation", "Ja", "Nej", "Avsluta");
                        if (confirm == "Avsluta") Environment.Exit(0);
                        if (confirm == "Nej") { step = 0; continue; }
                        if (confirm == "Ja")
                        {
                            // assign to properties and finish
                            Names = name;
                            Race = race;
                            Description = description;
                            Gender = gender;
                            Age = age;
                            Level = level;
                            Class = @class;
                            OtherInfo = otherInfo;

                            Console.WriteLine();
                            Console.WriteLine($"Character '{Names}' created.");
                            Console.WriteLine("Press any key to continue...");
                            MenuHelper.DelayAndClear();



                            return;
                        }
                        // if somehow other result, loop
                        
                        continue;
                    default:
                        return;
                }

                // Handle Back/Exit inputs for non-age/level steps
                if (input == "E")
                {
                    Console.Clear();
                    ChracterMenu2();
                }
                
                if (input == "B")
                {
                    if (step > 0) step--;
                    continue;
                }

                // Store input to the right variable for steps that use string input
                switch (step)
                {
                    case 0: name = input?.Trim() ?? ""; break;
                    case 1: race = input?.Trim() ?? ""; break;
                    case 2: description = input?.Trim() ?? ""; break;
                    case 3: gender = input?.Trim() ?? ""; break;
                    case 6: @class = input?.Trim() ?? ""; break;
                    case 7: otherInfo = input?.Trim() ?? ""; break;
                }

                step++;
            }
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
};
