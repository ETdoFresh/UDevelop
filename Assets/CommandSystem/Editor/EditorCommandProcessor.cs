using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem.Editor
{
    public static class EditorCommandProcessor
    {
        public static List<string> CommandInputs = new();
        public static List<string> CommandOutputs = new();
        public static List<Command> CommandHistory = new();
        public static List<string> CommandDisplay = new();
        public static List<CommandEntry> PossibleCommands = new();
        public static int CommandHistoryIndex;

        public static void ExecuteCommand(string commandInput)
        {
            if (string.IsNullOrEmpty(commandInput)) return;
            CommandInputs.Add(commandInput);
            CommandDisplay.Add(commandInput);

            RegisterCommands();

            var commands = commandInput.Split('\n');
            foreach (var command in commands)
            {
                try
                {
                    var trimmedCommand = command.Trim();
                    var commandName = trimmedCommand.Split(' ')[0];

                    if (string.Equals(commandName, "undo", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (CommandHistoryIndex <= 0) continue;
                        CommandHistoryIndex--;
                        CommandHistory[CommandHistoryIndex].OnUndo();
                        CommandOutputs.Add("Undo Complete!");
                        CommandDisplay.Add("Undo Complete!");
                        continue;
                    }

                    if (string.Equals(commandName, "redo", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (CommandHistoryIndex >= CommandHistory.Count) continue;
                        CommandHistory[CommandHistoryIndex].OnRedo();
                        CommandHistoryIndex++;
                        CommandOutputs.Add("Redo Complete!");
                        CommandDisplay.Add("Redo Complete!");
                        continue;
                    }

                    if (string.Equals(commandName, "help", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var helpList = new List<string>();
                        helpList.AddRange(new[] { "undo", "redo", "help" });
                        helpList.AddRange(PossibleCommands.Select(x => $"{x.commandName}"));
                        helpList.Sort();
                        CommandOutputs.Add($"Commands: {string.Join(", ", helpList)}");
                        CommandDisplay.Add($"Commands: {string.Join(", ", helpList)}");
                        continue;
                    }
                    
                    if (string.Equals(commandName, "clear", StringComparison.CurrentCultureIgnoreCase))
                    {
                        CommandOutputs.Clear();
                        CommandDisplay.Clear();
                        continue;
                    }

                    var commandEntry = PossibleCommands.Find(entry =>
                        string.Equals(entry.commandName, commandName, StringComparison.CurrentCultureIgnoreCase));
                    if (commandEntry == null)
                    {
                        CommandOutputs.Add($"Command {commandName} not found!");
                        CommandDisplay.Add($"Command {commandName} not found!");
                        continue;
                    }

                    var commandType = commandEntry.commandType;
                    var commandInstance = (Command)Activator.CreateInstance(commandType, trimmedCommand);
                    commandInstance.Run();
                    CommandHistory.Insert(CommandHistoryIndex, commandInstance);
                    CommandHistoryIndex++;
                    CommandHistory.RemoveRange(CommandHistoryIndex, CommandHistory.Count - CommandHistoryIndex);
                    CommandOutputs.Add(commandInstance.CommandOutput);
                    CommandDisplay.Add(commandInstance.CommandOutput);
                }
                catch (Exception ex)
                {
                    CommandOutputs.Add($"Error: {ex.Message}");
                    CommandDisplay.Add($"Error: {ex.Message}");
                }
            }
            CommandDisplay.Add("");
        }

        public static string GetCommandOutput()
        {
            return string.Join("\n", CommandDisplay);
        }

        public static int SelectPreviousCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return CommandInputs.Count - 1;
            if (selectedCommandIndex == 0) return 0;
            return selectedCommandIndex - 1;
        }

        public static int SelectNextCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex + 1 <= 0 || selectedCommandIndex + 1 >= CommandInputs.Count) return -1;
            if (selectedCommandIndex == CommandInputs.Count - 1) return CommandInputs.Count - 1;
            return selectedCommandIndex + 1;
        }

        public static string GetCommandHistory(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return "";
            return CommandInputs[selectedCommandIndex];
        }

        public static string AutoCompleteCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return "";
            var args = command.Split(' ');
            if (args.Length == 1)
            {
                RegisterCommands();
                var possibleCommandName = PossibleCommands.Find(x => x.commandName.StartsWith(args[0].ToLower()));
                return possibleCommandName == null ? command : possibleCommandName.commandName + " ";
            }

            return command;
        }

        private static void RegisterCommands()
        {
            if (PossibleCommands.Count > 0) return;
            foreach (var commandType in AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetTypes())
                         .SelectMany(x => x).Where(x => typeof(Command).IsAssignableFrom(x)))
            {
                if (commandType == typeof(Command)) continue;
                var command = (Command)Activator.CreateInstance(commandType, "");
                foreach (var commandName in command.CommandName)
                    PossibleCommands.Add(new CommandEntry { commandName = commandName, commandType = commandType });
            }
        }
    }
}