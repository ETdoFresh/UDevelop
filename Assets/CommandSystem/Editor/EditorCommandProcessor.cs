using System;
using UnityEngine;

namespace CommandSystem.Editor
{
    public static class EditorCommandProcessor
    {
        public static void ExecuteCommand(string commandInput)
        {
            if (string.IsNullOrEmpty(commandInput)) return;
            CommandHandlerScriptableObject.Inputs.Add(commandInput);

            var commandInputWithDecorators = $"> {commandInput}".Replace("\n", "\n> ");
            CommandHandlerScriptableObject.Display.Add(commandInputWithDecorators);
            
            var commands = commandInput.Split('\n');
            foreach (var command in commands)
            {
                try
                {
                    var output = CommandRunner.Run(command);
                    CommandHandlerScriptableObject.Outputs.Add(output.CommandLineOutput);
                    CommandHandlerScriptableObject.Display.Add(output.CommandLineOutput);

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
                    CommandHandlerScriptableObject.Outputs.Add($"Error: {ex.Message}");
                    CommandHandlerScriptableObject.Display.Add($"Error: {ex.Message}");
                    Debug.LogException(ex);
                }
            }
            CommandHandlerScriptableObject.Display.Add("");
        }

        public static string GetCommandOutput()
        {
            return string.Join("\n", CommandHandlerScriptableObject.Display);
        }

        public static int SelectPreviousCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return CommandHandlerScriptableObject.Inputs.Count - 1;
            if (selectedCommandIndex == 0) return 0;
            return selectedCommandIndex - 1;
        }

        public static int SelectNextCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex + 1 <= 0 || selectedCommandIndex + 1 >= CommandHandlerScriptableObject.Inputs.Count) return -1;
            if (selectedCommandIndex == CommandHandlerScriptableObject.Inputs.Count - 1) return CommandHandlerScriptableObject.Inputs.Count - 1;
            return selectedCommandIndex + 1;
        }

        public static string GetCommandHistory(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return "";
            return CommandHandlerScriptableObject.Inputs[selectedCommandIndex];
        }
    }
}