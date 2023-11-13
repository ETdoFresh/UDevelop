namespace CommandSystem
{
    public static class CommandletRunner
    {
        public static bool TryRun(string commandString, out OutputData outputData)
        {
            AttemptUpdateAliasMap();
            var firstSpaceIndex = commandString.IndexOf(' ');
            var commandAlias = firstSpaceIndex > 0 ? commandString[..firstSpaceIndex] : commandString;
            var commandArgString = firstSpaceIndex > 0 ? commandString[(firstSpaceIndex + 1)..] : "";
            var commandArgs = Parser.GetArgData(commandArgString);
            if (CommandRunner.CommandMap.TryGetValue(commandAlias.ToLower(), out var commandObjects))
            {
                outputData = null;
            }
            outputData = null;
            return false;
        }

        private static void AttemptUpdateAliasMap()
        {
            CommandRunner.Update();
        }
    }
}