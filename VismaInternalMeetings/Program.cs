global using System.Text.Json;
using VismaInternalMeetings;

string path = @"Meeting.json";
bool endApp = false;
List<Meeting> meetingList = new List<Meeting>();
IConsoleIO consoleIO = new ConsoleIO();
Commands commands = new Commands(consoleIO);

Initialize();
Console.WriteLine("Visma Internal Meetings");
Console.WriteLine("Login in as:");
string username = Console.ReadLine() ?? "Guest";

while (!endApp)
{
    Console.WriteLine($"\nWhat would you like to do, {username}?\n");
    Console.WriteLine("\tCreate a new meeting - new_meeting");
    Console.WriteLine("\tDelete meeting - delete_meeting");
    Console.WriteLine("\tAdd person to the meeting - add_person");
    Console.WriteLine("\tRemove person from the meeting - remove_person");
    Console.WriteLine("\tList all meetings - list_meetings");
    Console.WriteLine("\tExit application - exit\n");

    switch (Console.ReadLine())
    {
        case "new_meeting":
            commands.AddMeeting(meetingList, username);
            break;
        case "delete_meeting":
            commands.DeleteMeeting(meetingList, username);
            break;
        case "add_person":
            commands.AddPersonToMeeting(meetingList);
            break;
        case "remove_person":
            commands.RemovePersonFromMeeting(meetingList);
            break;
        case "list_meetings":
            commands.ListMeetings(meetingList);
            break;
        case "exit":
            endApp = true;
            break;
    }
}

void Initialize()
{
    try
    {
        if (File.Exists("Meeting.json"))
        {
            string fileData = File.ReadAllText(path);
            meetingList = JsonSerializer.Deserialize<List<Meeting>>(fileData) ?? new List<Meeting>();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading data file: {ex.Message}");
    }
}