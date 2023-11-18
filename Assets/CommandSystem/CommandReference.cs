using System.Collections.Generic;

namespace CommandSystem
{
    public class CommandReference
    {
        private string _commandString;
        private Dictionary<string, ArgData> _localArgs;

        public CommandReference(string commandString)
        {
            _commandString = commandString;
            _localArgs = new Dictionary<string, ArgData>();
        }
        
        public CommandReference(string commandString, Dictionary<string, ArgData> localArgs)
        {
            _commandString = commandString;
            _localArgs = localArgs;
        }

        public OutputData Run(params ArgData[] args)
        {
            var commandStringWithArgs = _commandString;
            foreach(var arg in args)
                if (!commandStringWithArgs.Contains(arg.Name))
                    commandStringWithArgs += " " + arg.Name;
            var localArgsWithArgs = new Dictionary<string, ArgData>(_localArgs);
            foreach (var arg in args) localArgsWithArgs[arg.Name] = arg;
            return CommandObject.TryRun(commandStringWithArgs, null, localArgsWithArgs, out var outputData)
                ? outputData
                : throw new System.Exception($"Command not found\n{commandStringWithArgs}");
        }
    }
}