namespace VismaInternalMeetings
{
    public class Commands
    {
        private const string Message = "Error reading user input";
        private readonly IConsoleIO ConsoleIO;

        public Commands(IConsoleIO consoleIO)
        {
            ConsoleIO = consoleIO;
        }

        public virtual void SaveMeetingsList(List<Meeting> meetingList)
        {
            string jsonData = JsonSerializer.Serialize(meetingList);
            File.WriteAllText("Meeting.json", jsonData);
        }

        public void AddMeeting(List<Meeting> meetingList, string responsiblePerson)
        {
            try
            {
                ConsoleIO.WriteLine("Input data:\n");

                ConsoleIO.Write("Name:");
                string name = ConsoleIO.ReadLine();

                ConsoleIO.Write("Description:");
                string description = ConsoleIO.ReadLine();

                ConsoleIO.Write("Category:");
                string category = ConsoleIO.ReadLine();

                ConsoleIO.Write("Type:");
                string Type = ConsoleIO.ReadLine();

                ConsoleIO.Write("Start date:");
                DateTime.TryParse(ConsoleIO.ReadLine(), out DateTime startDate);

                ConsoleIO.Write("End date:");
                DateTime.TryParse(ConsoleIO.ReadLine(), out DateTime endDate);

                ConsoleIO.WriteLine("Adding a meeting...");
                Meeting meeting = new Meeting(name, responsiblePerson, description, category, Type, startDate, endDate);
                meetingList.Add(meeting);
                SaveMeetingsList(meetingList);
            }
            catch (Exception ex)
            { 
                ConsoleIO.WriteLine(ex.Message);
            }
        }

        public void ListMeetings(List<Meeting> meetingList)
        {
            try
            {
                if (!meetingList.Any())
                {
                    ConsoleIO.WriteLine("No meetings, get back to work!");
                    return;
                }

                string param = string.Empty;

                ConsoleIO.WriteLine(" Listing meetings:\n");

                foreach (Meeting meeting in meetingList)
                {
                    ConsoleIO.WriteLine(meeting.ToString());
                    ConsoleIO.WriteLine("");
                }

                ConsoleIO.WriteLine(" Filter meetings by:");
                ConsoleIO.WriteLine("  Description - description");
                ConsoleIO.WriteLine("  Responsible person - person");
                ConsoleIO.WriteLine("  Category - category");
                ConsoleIO.WriteLine("  Type - type");
                ConsoleIO.WriteLine("  Dates - date");
                ConsoleIO.WriteLine("  No. of attendees higher than x - attendees");
                ConsoleIO.WriteLine("  Cancel - cancel\n");

                string input = ConsoleIO.ReadLine();
                IEnumerable<Meeting> filteredMeetings = null;
                switch (input)
                {
                    case "description":
                        ConsoleIO.WriteLine("Enter meeting's description:");
                        param = ConsoleIO.ReadLine();
                        filteredMeetings = meetingList.Where(meeting => meeting.Description.Contains(param));
                        break;
                    case "person":
                        ConsoleIO.WriteLine("Enter responsible person's name:");
                        param = ConsoleIO.ReadLine();
                        filteredMeetings = meetingList.Where(meeting => meeting.ResponsiblePerson.Contains(param));
                        break;
                    case "category":
                        ConsoleIO.WriteLine("Enter category:");
                        param = ConsoleIO.ReadLine();
                        filteredMeetings = meetingList.Where(meeting => meeting.Category.Contains(param));
                        break;
                    case "type":
                        ConsoleIO.WriteLine("Enter type:");
                        param = ConsoleIO.ReadLine();
                        filteredMeetings = meetingList.Where(meeting => meeting.Type.Contains(param));
                        break;
                    case "date":
                        ConsoleIO.WriteLine("Enter the date from (leaving the field empty filters up until end date):");
                        string inputStartDate = ConsoleIO.ReadLine();
                        ConsoleIO.WriteLine("Enter the date to (leaving the field empty filters from start date):");
                        string inputEndDate = ConsoleIO.ReadLine();

                        if (inputStartDate != "" && inputEndDate != "")
                        {
                            DateTime.TryParse(inputStartDate, out DateTime startDate);
                            DateTime.TryParse(inputEndDate, out DateTime endDate);
                            filteredMeetings = meetingList.Where(meeting => meeting.StartDate >= startDate && meeting.EndDate <= endDate);
                        }
                        else if (inputStartDate != "" && inputEndDate == "")
                        {
                            DateTime.TryParse(inputStartDate, out DateTime startDate);
                            filteredMeetings = meetingList.Where(meeting => meeting.StartDate >= startDate);
                        }
                        else if (inputStartDate == "" && inputEndDate != "")
                        {
                            DateTime.TryParse(inputEndDate, out DateTime endDate);
                            filteredMeetings = meetingList.Where(meeting => meeting.EndDate <= endDate);
                        }
                        break;
                    case "attendees":
                        ConsoleIO.WriteLine("Enter the number of attendees (filters meetings with attendee count bigger than entered):");
                        param = ConsoleIO.ReadLine();
                        filteredMeetings = meetingList.Where(meeting => meeting.Attendees.Count > Int32.Parse(param));
                        break;
                    case "cancel":
                        return;
                }

                if (filteredMeetings.Count() > 0)
                {
                    ConsoleIO.WriteLine("Filtered data:");
                    foreach (Meeting meeting in filteredMeetings)
                    {
                        ConsoleIO.WriteLine(meeting.ToString());
                        ConsoleIO.WriteLine("");
                    }
                }
                else
                {
                    ConsoleIO.WriteLine("Nothing found...");
                }
                ConsoleIO.WriteLine("");
            }
            catch (Exception ex)
            {
                ConsoleIO.WriteLine(ex.Message);
            }
        }

        public void DeleteMeeting(List<Meeting> meetingList, string responsiblePerson)
        {
            try
            {
                ConsoleIO.WriteLine("Delete meeting");
                ConsoleIO.WriteLine("Enter meetings name:");
                string meetingName = ConsoleIO.ReadLine();
                Meeting meeting = meetingList.Find(meeting => meeting.Name == meetingName);

                if (meeting == null)
                {
                    ConsoleIO.WriteLine($"Meeting with name \"{meetingName}\" not found.");
                    return;
                }

                ConsoleIO.WriteLine("Checking credentials...");

                if (meeting.ResponsiblePerson != responsiblePerson)
                {
                    ConsoleIO.WriteLine("Unauthorized action!");
                    return;
                }

                ConsoleIO.WriteLine("Credentials approved. Deleting meeting...");
                meetingList.Remove(meeting);
                SaveMeetingsList(meetingList);
                ConsoleIO.WriteLine("Meeting removed");
            }
            catch(Exception ex)
            {
                ConsoleIO.WriteLine(ex.Message);
            }
        }

        public void AddPersonToMeeting(List<Meeting> meetingList)
        {
            try
            {
                ConsoleIO.WriteLine("Enter meeting's name:");
                string meetingName = ConsoleIO.ReadLine();

                Meeting? meeting = meetingList.Find(meetingList => meetingList.Name == meetingName);

                if (meeting == null)
                {
                    ConsoleIO.WriteLine($"Meeting with the name \"{meetingName}\" does not exist!");
                    return;
                }

                ConsoleIO.WriteLine("Enter person's name:");
                string personName = ConsoleIO.ReadLine();
                Attendee attendee = new Attendee(personName, meeting.StartDate);

                if (meeting.Attendees.Find(attendee => attendee.Name == personName) != null)
                {
                    ConsoleIO.WriteLine("The person you wish to add has already been added!");
                    return;
                }

                List<Meeting> attendeeMeetings = new List<Meeting>();

                foreach (Meeting meet in meetingList)
                {
                    if (meet.HasAttendee(attendee))
                    {
                        attendeeMeetings.Add(meet);
                    }
                }

                if (attendeeMeetings.Count != 0)
                {
                    Meeting? concurrentMeet = attendeeMeetings.FirstOrDefault(meet => meet.StartDate <= meeting.StartDate && meet.EndDate >= meeting.StartDate);
                    if (concurrentMeet != null)
                    {
                        ConsoleIO.WriteLine($"You have an intersecting meeting with the name of \"{concurrentMeet.Name}\"!");
                    }
                }

                Meeting temp = meeting;
                temp.Attendees.Add(attendee);
                meetingList.Remove(meeting);
                meetingList.Add(temp);

                SaveMeetingsList(meetingList);
                ConsoleIO.WriteLine($"Person {personName} added to meeting {meetingName}.\n");
            }
            catch (Exception ex)
            {
                ConsoleIO.WriteLine(ex.Message);
            }
        }

        public void RemovePersonFromMeeting(List<Meeting> meetingList)
        {
            try
            {
                ConsoleIO.WriteLine("Removing person from the meeting\n");
                ConsoleIO.WriteLine("Enter meeting's name");
                string meetingName = ConsoleIO.ReadLine();
                Meeting targetMeeting = meetingList.Find(meet => meet.Name == meetingName);
                if (targetMeeting == null)
                {
                    ConsoleIO.WriteLine($"Meeting with the name \"{meetingName}\" does not exist!");
                    return;
                }

                foreach (var person in targetMeeting.Attendees)
                {
                    ConsoleIO.WriteLine($"Attendee: {person.Name}");
                }
                ConsoleIO.WriteLine("\nEnter the person's name which you desire to remove from the meeting");
                string targetPerson = ConsoleIO.ReadLine();

                if (targetMeeting.Attendees.Where(attendee => attendee.Name == targetPerson).Count() == 0)
                {
                    ConsoleIO.WriteLine($"Attendee with the name of \"{targetPerson}\" does not exit in this meeting!");
                    return;
                }

                if (targetMeeting.ResponsiblePerson == targetPerson)
                {
                    ConsoleIO.WriteLine($"Cannot remove person {targetPerson} responsible for the meeting!");
                    return;
                }

                Attendee targetAttendee = targetMeeting.Attendees.FirstOrDefault(person => person.Name == targetPerson);
                if (targetAttendee == null)
                {
                    ConsoleIO.WriteLine("Failed to retrieve desired attendee!");
                    return;
                }

                Meeting tmpMeeting = targetMeeting;
                targetMeeting.Attendees.Remove(targetAttendee);
                meetingList.Remove(tmpMeeting);
                meetingList.Add(targetMeeting);

                SaveMeetingsList(meetingList);
                ConsoleIO.WriteLine($"Person {targetPerson} removed from meeting {meetingName}.\n");
            }
            catch (Exception ex)
            {
                ConsoleIO.WriteLine(ex.Message);
            }
        }
    }
}
