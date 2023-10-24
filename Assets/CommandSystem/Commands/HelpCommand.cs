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
                var possibleCommands = CommandJsonData.GetKeyAndValue<string>("", "Description");
                var commandName = args.Count > 1 ? args[1] : null;
                if (commandName != null)
                {
                    if (possibleCommands.TryGetValue(commandName, out var commandDescription))
                        return $"{commandName}\n{commandDescription}\n";
                    
                    return $"Command {commandName} not found!\n";
                }

                var output = "";
                foreach(var command in possibleCommands.OrderBy(x => x.Key))
                    output += $"{command.Key,-20}{command.Value}\n";
                return output;
            }
        }
    }
}