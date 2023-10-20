using System.Collections.Generic;

namespace CommandSystem.Editor
{
    public static class EditorCommandProcessor
    {
        public static List<string> CommandHistory = new();

        public static void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return;
            CommandHistory.Add(command);
            // TODO: Execute command.
        }

        public static string GetCommandOutput()
        {
            return string.Join("\n", CommandHistory);
        }

        public static int SelectPreviousCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return CommandHistory.Count - 1;
            if (selectedCommandIndex == 0) return 0;
            return selectedCommandIndex - 1;
        }
        
        public static int SelectNextCommand(int selectedCommandIndex)
        {
            if (selectedCommandIndex + 1 <= 0 || selectedCommandIndex + 1 >= CommandHistory.Count) return -1;
            if (selectedCommandIndex == CommandHistory.Count - 1) return CommandHistory.Count - 1;
            return selectedCommandIndex + 1;
        }
        
        public static string GetCommandHistory(int selectedCommandIndex)
        {
            if (selectedCommandIndex <= -1) return "";
            return CommandHistory[selectedCommandIndex];
        }

        public static string AutoCompleteCommand(string command)
        {
            return command;
        }
    }
}