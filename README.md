# Adventurer’s Journal
### 404-Not-Founders-GroupAssignment

An app that helps you to store all that valuable information you want for your adventures in roleplays, writing projects or any ways you see fit.
# Design overview (class diagram / pseudo diagram)

<img src="https://imgur.com/3JhtW88" width="200" alt="class-diagrams" />

At first we decided on these five classes that you see in the Class Diagram. But later on we decided to add a few more classes to ease the build of the program. At first we chose to create a MenuHelper class to put all our menus in but after some time it got too big and difficult to handle, so we had to divide MenuHelper into smaller classes. We also added a few UI classes for our programs UI and two services classes.

### We also made a flowchart that looks like this:
https://imgur.com/zNck1Af
<img src="https://imgur.com/zNck1Af" width="200" alt="A flowchart" />

This was very useful as it helped us plan the tasks needed to create the app. It let us see what menus and functions we needed to help the User get a smooth user experience.

## Early on we decided how we wanted our UI and the colors of our app to look like. 

### Here we decided to use these colors:

Menyval(aktiv): Orange1
Title: Använd hex färgen #FFA500.
Error (error från server eller om du skrivit fel): Red
Success (Något har registrerats korrekt): Green 

<img src="https://imgur.com/ZOHkNbg" width="200" alt="menu in white and orange" />


# HOW TO USE UserService: 

It is initialized via Program.cs (check MenuHelper to see how it is initialized/used from other files, look for _userService). In this way, we ALWAYS use the same file and data so that nothing gets overwritten. This method does not work with methods that are static.

If there are things you don’t understand, ask the person who wrote the code (Linda and Benji) or have AI break it down line by line and ask questions until you understand.

The file structure is:

```
  "Users": [
    {
      "Email": "Testing",
      "Username": "testing",
      "Password": "testing",
      "CreationDate": "2025-11-10T14:16:43.0761063+01:00",
      "Projects": [
              "Characters": [],
              "Storyline": [],
              "Worlds": []
      ],

    },
    {
      "Email": "test",
      "Username": "test",
      "Password": "test",
      "CreationDate": "2025-11-10T14:43:05.1866501+01:00",
      "Projects": [
            "Characters": [],
            "Storyline": [],
            "Worlds": []
      ],

    },
    {
      "Email": "yes",
      "Username": "yes",
      "Password": "yes",
      "CreationDate": "2025-11-10T14:43:22.0977721+01:00",
      "Projects": [
            "Characters": [],
            "Storyline": [],
            "Worlds": [],
      ],

    }
  ],

```
