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

        public object Run()
        {
            return CommandObject.RunCommandReference(_commandString);
        }

        public object Run(params ArgData[] args)
        {
            return CommandObject.RunCommandReference(_commandString, args);
        }
    }
}