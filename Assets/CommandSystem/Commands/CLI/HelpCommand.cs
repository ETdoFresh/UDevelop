using System;

namespace CommandSystem.Commands.CLI
{
    [Serializable]
    public class HelpCommand : Command
    {
        public HelpCommand(string commandInput) : base(commandInput) { }

        public override string CommandOutput
        {
            get
            {
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
                    if (commandArg1Description == null)
                    {
                        var commandArg1Subcommand = CommandJsonData.Get<string>($"{commandTypeName}.Arg1.PossibleValues.{commandArg1Name}.Subcommand");
                        if (commandArg1Subcommand != null)
                            commandArg1Description = CommandJsonData.Get<string>($"{commandArg1Subcommand}.Description");   
                    }
                    var commandArg1Aliases = CommandArgs.GetArgAliases(commandType, 1)[commandArg1Name];
                    return $"{commandTypeName} - {commandDescription}\n{commandArg1Name} - {commandArg1Description}\nAliases: {string.Join(",", commandArg1Aliases)}\nUsage: {commandAlias} {commandArg1}";
                }
                if (commandAlias != null)
                {
                    var commandType = CommandTypes.GetByAlias(commandAlias.ToLower());
                    if (commandType == null) return $"Command {commandAlias} not found!\n";
                    var commandTypeName = commandType.Name;
                    var commandDescription = CommandJsonData.Get<string>($"{commandTypeName}.Description");
                    var commandAliases = CommandJsonData.Get<string[]>($"{commandTypeName}.Aliases");
                    var commandUsage = CommandJsonData.Get<string>($"{commandTypeName}.Usage");
                    var hasArg1PossibleValues = CommandJsonData.HasKey($"{commandTypeName}.Arg1.PossibleValues");
                    string commandArg1PossibleValues = null;
                    if (hasArg1PossibleValues)
                    {
                        commandArg1PossibleValues += "\n";
                        var arg1Descriptions = CommandJsonData.GetKeyAndValue<string>($"{commandTypeName}.Arg1.PossibleValues", "Description");
                        foreach (var arg1NameAndDescription in arg1Descriptions)
                        {
                            var arg1Name = arg1NameAndDescription.Key;
                            var arg1Description = arg1NameAndDescription.Value;
                            if (arg1Description == null)
                            {
                                var commandArg1Subcommand = CommandJsonData.Get<string>($"{commandTypeName}.Arg1.PossibleValues.{arg1Name}.Subcommand");
                                if (commandArg1Subcommand != null)
                                    arg1Description = CommandJsonData.Get<string>($"{commandArg1Subcommand}.Description");   
                            }
                            commandArg1PossibleValues += $"\n{commandAlias} {arg1Name} - {arg1Description}";
                        }
                    }
                    return $"{commandTypeName} - {commandDescription}\nAliases: {string.Join(",", commandAliases)}\nUsage: {commandAlias} {commandUsage}{commandArg1PossibleValues}";
                }

                var possibleCommands = CommandJsonData.GetKeys("");
                possibleCommands.Sort();
                var possibleDescriptions = CommandJsonData.GetKeyAndValue<string>("", "Description");
                var output = "help - Displays this help message\nUsage: help [command]\n\n";
                var excludeFromHelp = CommandJsonData.GetKeyAndValue<bool>("", "ExcludeFromHelp");
                foreach(var command in possibleCommands)
                    if (excludeFromHelp == null || !excludeFromHelp.TryGetValue(command, out var value) || !value)
                    {
                        possibleDescriptions.TryGetValue(command, out var description);
                        output += $"{command} - {description}\n";
                    }

                return output;
            }
        }
    }
}