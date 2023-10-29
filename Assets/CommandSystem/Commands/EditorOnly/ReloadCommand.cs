namespace CommandSystem.Commands
{
    public class ReloadCommand : Command
    {
        public ReloadCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            CommandJsonData.Reload();
            CommandTypes.Reload();
        }
    }
}