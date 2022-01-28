namespace VismaInternalMeetings
{
    public interface IConsoleIO
    {
        void Write(string s);
        void WriteLine(string s);
        string ReadLine();
    }
}
