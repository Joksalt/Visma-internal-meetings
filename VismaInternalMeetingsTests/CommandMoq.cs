using System.Collections.Generic;

using VismaInternalMeetings;

namespace VismaInternalMeetingsTests
{
    public class CommandsMoq : Commands
    {
        public CommandsMoq(IConsoleIO consoleIO) 
            : base(consoleIO)   
        {
        }
        public override void SaveMeetingsList(List<Meeting> meetingList)
        { 
        }

    }
}
