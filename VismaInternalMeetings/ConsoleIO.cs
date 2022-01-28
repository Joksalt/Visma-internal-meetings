namespace VismaInternalMeetings
{
    public class ConsoleIO : IConsoleIO
    {
        private const string Message = "Error reading user input";

        public void Write(string s)
        {
            Console.Write(s);
        }

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }

        public string ReadLine()
        {
            return Console.ReadLine() ?? throw new Exception(Message);
        }
    }
}
