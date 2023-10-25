using System;
using CommandSystem.Commands;
using UnityEngine;

namespace CommandSystem.Editor
{
    public static class EditorCommandProcessor
    {
        public static void ExecuteCommand(string commandInput)
        {
            if (string.IsNullOrEmpty(commandInput)) return;
            CommandData.Inputs.Add(commandInput);
            CommandData.Display.Add(commandInput);
            
            var commands = commandInput.Split('\n');
            foreach (var command in commands)
            {
                try
                {
                    var trimmedCommand = command.Trim();
                    var commandName = trimmedCommand.Split(' ')[0];

                    var commandType = CommandTypes.GetByAlias(commandName.ToLower());
                    
                    if (commandType == null)
                    {
                        CommandData.Outputs.Add($"Command {commandName} not found!");
                        CommandData.Display.Add($"Command {commandName} not found!");
                        continue;
                    }
                    
                    var commandInstance = (Command)Activator.CreateInstance(commandType, trimmedCommand);
                    commandInstance.Run();
                    
                    if (commandInstance.AddToHistory)
                    {
                        CommandData.History.Insert(CommandData.HistoryIndex, commandInstance);
                        CommandData.HistoryIndex++;
                        CommandData.History.RemoveRange(CommandData.HistoryIndex, CommandData.History.Count - CommandData.HistoryIndex);
                    }

                    if (commandInstance.CommandOutput != null)
                    {
                        CommandData.Outputs.Add(commandInstance.CommandOutput);
                        CommandData.Display.Add(commandInstance.CommandOutput);
                    }
                }
                catch (Exception ex)
                {
                    CommandData.Outputs.Add($"Error: {ex.Message}");
                    CommandData.Display.Add($"Error: {ex.Message}");
                    Debug.LogException(ex);
                }
            }
            CommandData.Display.Add("");
        }

        public static string GetCommandOutput()
        {
            return string.Join("\n", CommandData.Display);
        }

        public static int SelectPreviousCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return CommandData.Inputs.Count - 1;
            if (selectedCommandIndex == 0) return 0;
            return selectedCommandIndex - 1;
        }

        public static int SelectNextCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex + 1 <= 0 || selectedCommandIndex + 1 >= CommandData.Inputs.Count) return -1;
            if (selectedCommandIndex == CommandData.Inputs.Count - 1) return CommandData.Inputs.Count - 1;
            return selectedCommandIndex + 1;
        }

        public static string GetCommandHistory(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return "";
            return CommandData.Inputs[selectedCommandIndex];
        }
    }
}