using CommandSystem.Commands.Select;

namespace CommandSystem.Commands.EditorOnly
{
    public class ReloadCommand : Command
    {
        public ReloadCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            CommandJsonData.Reload();
            CommandTypes.Reload();
            SelectionUtil.Reload();
        }
    }
}