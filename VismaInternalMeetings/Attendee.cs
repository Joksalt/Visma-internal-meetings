namespace VismaInternalMeetings
{
    public class Attendee
    {
        public string Name { get; set; }
        public DateTime TimeAdded { get; set; }

        public Attendee(string name, DateTime timeAdded)
        {
            Name = name;
            TimeAdded = timeAdded;
        }
    }
}
