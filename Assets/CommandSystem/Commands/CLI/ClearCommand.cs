using System;

namespace CommandSystem.Commands
{
    [Serializable]
    public class ClearCommand : Command
    {
        public ClearCommand(string commandInput) : base(commandInput) { }

        public override string CommandOutput => null;

        public override void OnRun(params string[] args)
        {
            CommandData.Clear();
        }
    }
}