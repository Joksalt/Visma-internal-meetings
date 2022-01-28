using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using VismaInternalMeetings;

namespace VismaInternalMeetingsTests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod()]
        public void TestAddMeeting_AsksInput_AddsMeeting()
        {
            var name = "Jokubas";
            var mockConsoleIO = new Mock<IConsoleIO>();
            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns(name)
                .Returns("Description")
                .Returns("Category")
                .Returns("Type")
                .Returns("2022/02/02")
                .Returns("2022/02/03");
            var commands = new CommandsMoq(mockConsoleIO.Object);
            var meetingList = new List<Meeting>();

            commands.AddMeeting(meetingList, name);

            Assert.AreEqual(1, meetingList.Count, "Meeting list should have one meeting");
            var meeting = meetingList[0];

            Assert.AreEqual(name, meeting.Name, "Meeting's name should be the same");
            Assert.AreEqual("Description", meeting.Description, "Meeting's description should be the same");
            Assert.AreEqual("Type", meeting.Type, "Meeting's type should be the same");
        }

        [TestMethod]
        public void TestListMeeting_InputFilter_ListsMeetings()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var commands = new CommandsMoq(mockConsoleIO.Object);
            var meetingList = new List<Meeting>();


            commands.ListMeetings(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("No meetings, get back to work!"), Times.Once);

            var startDate = System.DateTime.Now;
            var endDate = startDate.AddDays(1);
            meetingList.Add(new Meeting("Jonas meet", "Jonas", "Description", "Hub", "Live", startDate, endDate));

            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("person")
                .Returns("Jonas");

            commands.ListMeetings(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("Filtered data:"), Times.Once);

            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("person")
                .Returns("Jokubas");

            commands.ListMeetings(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("Nothing found..."), Times.Once);
        }

        [TestMethod]
        public void TestDeleteMeeting_InputMeeting_DeletesMeeting()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var commands = new CommandsMoq(mockConsoleIO.Object);
            var meetingList = new List<Meeting>();

            var startDate = System.DateTime.Now;
            var endDate = startDate.AddDays(1);
            meetingList.Add(new Meeting("Jonas meet", "Jonas", "Description", "Hub", "Live", startDate, endDate));

            //check if meeting exists with the given name
            mockConsoleIO.Setup(t => t.ReadLine()).Returns("Petro meet");

            commands.DeleteMeeting(meetingList, "Jonas");

            mockConsoleIO.Verify(t => t.WriteLine("Meeting with name \"Petro meet\" not found."), Times.Once);

            //check if user is responsible for the meeting
            mockConsoleIO.Setup(t => t.ReadLine()).Returns("Jonas meet");

            commands.DeleteMeeting(meetingList, "Petras");

            mockConsoleIO.Verify(t => t.WriteLine("Unauthorized action!"), Times.Once);

            //check if meeting deletion is successful
            mockConsoleIO.Setup(t => t.ReadLine()).Returns("Jonas meet");

            commands.DeleteMeeting(meetingList, "Jonas");

            mockConsoleIO.Verify(t => t.WriteLine("Credentials approved. Deleting meeting..."), Times.Once);
            Assert.AreEqual(0, meetingList.Count, "Meeting list should have no meetings");
            mockConsoleIO.Verify(t => t.WriteLine("Meeting removed"), Times.Once);
        }

        [TestMethod]
        public void TestAddPersonToMeeting_InputPerson_AddsPerson()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var commands = new CommandsMoq(mockConsoleIO.Object);
            var meetingList = new List<Meeting>();

            var startDate = System.DateTime.Now;
            var endDate = startDate.AddDays(1);
            meetingList.Add(new Meeting("Jonas meet", "Jonas", "Description", "Hub", "Live", startDate, endDate));
            
            //check if meeting exists with the given name
            mockConsoleIO.Setup(t => t.ReadLine()).Returns("Petro meet");
            commands.AddPersonToMeeting(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("Meeting with the name \"Petro meet\" does not exist!"), Times.Once);

            //check if person is already added
            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("Jonas meet")
                .Returns("Jonas");

            commands.AddPersonToMeeting(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("The person you wish to add has already been added!"), Times.Once);

            //check if person has meetings during the same time
            meetingList.Add(new Meeting("Petras meet", "Petras", "Description", "Hub", "Live", startDate, endDate));
            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("Petras meet")
                .Returns("Jonas");

            commands.AddPersonToMeeting(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("You have an intersecting meeting with the name of \"Jonas meet\"!"), Times.Once);

            //check if person was added to meeting successfully
            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("Jonas meet")
                .Returns("Juozas");

            commands.AddPersonToMeeting(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("Person Juozas added to meeting Jonas meet.\n"), Times.Once);
            Assert.AreEqual(2, meetingList.Find(x => x.Name == "Jonas meet").Attendees.Count, "Meeting's attendees count should be two");
        }

        [TestMethod]
        public void TestRemovePersonFromMeeting_InputPerson_RemovesPerson()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var commands = new CommandsMoq(mockConsoleIO.Object);
            var meetingList = new List<Meeting>();

            var startDate = System.DateTime.Now;
            var endDate = startDate.AddDays(1);
            meetingList.Add(new Meeting("Jonas meet", "Jonas", "Description", "Hub", "Live", startDate, endDate));

            mockConsoleIO.Setup(t => t.ReadLine()).Returns("Petras meet");

            commands.RemovePersonFromMeeting(meetingList);
            
            //check if meeting with the given name exists
            mockConsoleIO.Verify(t => t.WriteLine("Meeting with the name \"Petras meet\" does not exist!"), Times.Once);

            //check if attendee exist in the given meeting
            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("Jonas meet")
                .Returns("Petras");

            commands.RemovePersonFromMeeting(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("Attendee with the name of \"Petras\" does not exit in this meeting!"), Times.Once);

            //check if user cant remove responsible person from the meeting
            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("Jonas meet")
                .Returns("Jonas");

            commands.RemovePersonFromMeeting(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("Cannot remove person Jonas responsible for the meeting!"), Times.Once);

            //check if person could be removed from the meeting
            Meeting meeting = new Meeting("Petras meet", "Petras", "Description", "Hub", "Live", startDate, endDate);
            meeting.AddAttendee("Jonas", startDate);
            meetingList.Add(meeting);
            
            mockConsoleIO.SetupSequence(t => t.ReadLine())
                .Returns("Petras meet")
                .Returns("Jonas");

            commands.RemovePersonFromMeeting(meetingList);

            mockConsoleIO.Verify(t => t.WriteLine("Person Jonas removed from meeting Petras meet.\n"), Times.Once);
            Assert.AreEqual(1, meetingList.Find(meet => meet.Name == "Petras meet").Attendees.Count, "Meeting should have only one attendee");
        }
    }
}