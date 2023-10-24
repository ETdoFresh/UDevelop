using System.Collections.Generic;

namespace CommandSystem
{
    public static class CommandData
    {
        public static List<string> Inputs = new();
        public static List<string> Outputs = new();
        public static List<Command> History = new();
        public static List<string> Display = new();
        public static int HistoryIndex;
    }
}