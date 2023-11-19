using System.Collections.Generic;

namespace CommandSystem
{
    public class CommandReference
    {
        private string _commandString;

        public CommandReference(string commandString)
        {
            _commandString = commandString;
        }

        public Dictionary<string, ArgData> Run()
        {
            return CommandObject.RunCommandReference(_commandString);
        }

        public Dictionary<string, ArgData> Run(params ArgData[] args)
        {
            return CommandObject.RunCommandReference(_commandString, args);
        }
    }
}