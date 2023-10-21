using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem
{
    public static class CommandData
    {
        public static List<string> Inputs = new();
        public static List<string> Outputs = new();
        public static List<Command> History = new();
        public static List<string> Display = new();
        public static List<CommandEntry> PossibleCommands = new();
        public static int HistoryIndex;

        static CommandData()
        {
            RegisterCommands();
        }
            
        private static void RegisterCommands()
        {
            if (PossibleCommands.Count > 0) return;
            foreach (var commandType in AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetTypes())
                         .SelectMany(x => x).Where(x => typeof(Command).IsAssignableFrom(x)))
            {
                if (commandType == typeof(Command)) continue;
                var command = (Command)Activator.CreateInstance(commandType, "");
                foreach (var commandName in command.CommandNames)
                    PossibleCommands.Add(new CommandEntry { commandName = commandName, commandType = commandType });
            }
        }
    }
}