namespace CommandSystem
{
    public class Command
    {
        public static OutputData Run(string commandString)
        {
            return CommandJsonRunner.ProcessCommandInputString(commandString);
        }
    }
}