using System.Collections.Generic;
using System.Linq;

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
            if (args.Length > 0) commandStringWithArgs += " " + string.Join(" ", args.Select(x => x.Name));
            var localArgsWithArgs = new Dictionary<string, ArgData>(_localArgs);
            foreach (var arg in args) localArgsWithArgs[arg.Name] = arg;
            return CommandJsonRunner.AttemptToRunCommand(commandStringWithArgs, localArgsWithArgs);
        }
    }
}