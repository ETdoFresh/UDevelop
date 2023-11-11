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

            var commandInputWithDecorators = $"> {commandInput}".Replace("\n", "\n> ");
            CommandData.Display.Add(commandInputWithDecorators);
            
            var commands = commandInput.Split('\n');
            foreach (var command in commands)
            {
                try
                {
                    var output = CommandJsonRunner.ProcessCommandInputString(command);
                    CommandData.Outputs.Add(output);
                    CommandData.Display.Add(output);

                    // TODO: Will have to figure out how to reverse each command in the future
                    // if (commandInstance.AddToHistory)
                    // {
                    //     CommandData.History.Insert(CommandData.HistoryIndex, commandInstance);
                    //     CommandData.HistoryIndex++;
                    //     CommandData.History.RemoveRange(CommandData.HistoryIndex, CommandData.History.Count - CommandData.HistoryIndex);
                    // }
                    //
                    // if (commandInstance.CommandOutput != null)
                    // {
                    //     CommandData.Outputs.Add(commandInstance.CommandOutput);
                    //     CommandData.Display.Add(commandInstance.CommandOutput);
                    // }
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