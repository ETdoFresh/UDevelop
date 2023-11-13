using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem
{
    public class CommandReference
    {
        public string commandString;
        public Dictionary<string, ArgData> localArgs;

        public static CommandReference Create(string commandString, Dictionary<string, ArgData> localArgs)
        {
            return new CommandReference
            {
                commandString = commandString,
                localArgs = localArgs
            };
        }

        public OutputData Run(params ArgData[] args)
        {
            var commandStringWithArgs = commandString;
            if (args.Length > 0) commandStringWithArgs += " " + string.Join(" ", args.Select(x => x.Name));
            var localArgsWithArgs = new Dictionary<string, ArgData>(localArgs);
            foreach (var arg in args) localArgsWithArgs[arg.Name] = arg;
            return CommandJsonRunner.AttemptToRunCommand(commandStringWithArgs, localArgs);
        }
    }
}