using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem.Commands
{
    [Serializable]
    public class HelpCommand : Command
    {
        public HelpCommand(string commandInput) : base(commandInput) { }

        public override string CommandOutput
        {
            get
            {
                var helpList = new List<string>();
                helpList.AddRange(CommandData.PossibleCommands.Select(x => $"{x.commandName}"));
                helpList.Sort();
                return string.Join("\n", helpList);
            }
        }
    }

    [Serializable]
    public class ClearCommand : Command
    {
        public ClearCommand(string commandInput) : base(commandInput) { }

        public override string CommandOutput => null;

        public override void OnRun(params string[] args)
        {
            CommandData.Outputs.Clear();
            CommandData.Display.Clear();
        }
    }
}