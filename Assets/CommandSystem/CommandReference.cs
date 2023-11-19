using System;
using System.Collections.Generic;

namespace CommandSystem
{
    public class CommandReference
    {
        private string _commandString;
        private CommandObject[] commands;
        private Dictionary<string, ArgData> _localArgs;

        public CommandReference(string commandString)
        {
            _commandString = commandString;
            _localArgs = new Dictionary<string, ArgData>();
            commands = CommandObject.FromCommandletString(commandString);
        }
        
        public CommandReference(string commandString, Dictionary<string, ArgData> localArgs)
        {
            _commandString = commandString;
            _localArgs = localArgs;
            commands = CommandObject.FromCommandletString(commandString);
        }

        public OutputData Run() => Run(Array.Empty<ArgData>());
        
        public OutputData Run(params ArgData[] args)
        {
            var commandStrings = Parser.MultiCommandString(_commandString);
            var outputData = (OutputData) null;
            foreach (var commandString in commandStrings)
            {
                var commandStringWithArgs = commandString;
                foreach (var arg in args)
                    if (!commandStringWithArgs.Contains(arg.Name))
                        commandStringWithArgs += " " + arg.Name;
                var localArgsWithArgs = new Dictionary<string, ArgData>(_localArgs);
                foreach (var arg in args) localArgsWithArgs[arg.Name] = arg;
                if (!CommandObject.TryRun(commandStringWithArgs, null, localArgsWithArgs, out outputData))
                    throw new Exception($"Command not found\n{commandStringWithArgs}");
            }
            return outputData;
        }
    }
}