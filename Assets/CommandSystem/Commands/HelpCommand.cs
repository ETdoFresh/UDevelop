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
                var commandAlias = args.Count > 1 ? args[1] : null;
                var commandArg1 = args.Count > 2 ? args[2] : null;
                if (commandAlias != null && commandArg1 != null)
                {
                    var commandType = CommandTypes.GetByAlias(commandAlias.ToLower());
                    if (commandType == null) return $"Command {commandAlias} not found!";
                    var commandTypeName = commandType.Name;
                    var commandDescription = CommandJsonData.Get<string>($"{commandTypeName}.Description");
                    var commandArg1Name = CommandArgs.GetArgByAlias(commandType, 1, commandArg1.ToLower());
                    if (commandArg1Name == null) return $"Command {commandAlias} does not have an argument {commandArg1}!";
                    var commandArg1Description = CommandJsonData.Get<string>($"{commandTypeName}.Arg1.PossibleValues.{commandArg1Name}.Description");
                    return $"{commandAlias} - {commandDescription}\n{commandArg1} - {commandArg1Description}\nUsage: {commandAlias} {commandArg1}";
                }
                if (commandAlias != null)
                {
                    var commandType = CommandTypes.GetByAlias(commandAlias.ToLower());
                    if (commandType == null) return $"Command {commandAlias} not found!\n";
                    var commandTypeName = commandType.Name;
                    var commandDescription = CommandJsonData.Get<string>($"{commandTypeName}.Description");
                    var commandUsage = CommandJsonData.Get<string>($"{commandTypeName}.Usage");
                    return $"{commandAlias} - {commandDescription}\nUsage: {commandAlias} {commandUsage}";
                }

                var output = "help - Displays this help message\nUsage: help [command]\n\n";
                foreach(var command in possibleCommands.OrderBy(x => x.Key))
                    output += $"{command.Key,-20}{command.Value}\n";
                return output;
            }
        }
    }
}