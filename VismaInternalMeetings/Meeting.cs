namespace VismaInternalMeetings
{
    [Serializable]
    public class Meeting
    {
        public string Name { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Attendee> Attendees { get; set; }

        public Meeting(string name, string responsiblePerson, string description, string category, string type, DateTime startDate, DateTime endDate)
        {
            Name = name;
            ResponsiblePerson = responsiblePerson;
            Description = description;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Attendees = new List<Attendee>
            {
                new Attendee(responsiblePerson, startDate)
            };
        }

        public bool HasAttendee(Attendee attendee)
        {
            return Attendees.Exists(person => person.Name == attendee.Name);
        }

        public void AddAttendee(string name, DateTime date)
        {
            Attendees.Add(new Attendee(name, date));
        }

        public override string ToString()
        {
            return new string($"Name: {Name}\nResponsible person: {ResponsiblePerson}\nDescription:{Description}\nCategory: {Category}\nType: {Type}\nStart date: {StartDate}\n  End date: {EndDate}\nAttendees: {Attendees.Count}");
        }
    }
}
