using System.Linq;

namespace CommandSystem.Commands
{
    public static class CommandAutoComplete
    {
        private static string lastCommand = "";
        private static string lastAutoComplete = "";
        
        public static string Get(string command)
        {
            return "";
            if (string.IsNullOrEmpty(command)) return "";
            if (command == lastCommand) return lastAutoComplete;
            
            var autoComplete = "";
            var args = command.Split(' ', ',');
            if (args.Length == 1)
            {
                var arg = args[0].ToLower();
                var aliases = CommandJsonData.GetKeyAndValue<string[]>("", "Aliases");
                var keys = aliases.Keys;
                var maxAliasLength = aliases.Values.Where(x => x != null).Max(x => x.Length);
                for(var i = 0; i < maxAliasLength; i++)
                {
                    foreach(var key in keys)
                    {
                        if (aliases[key] == null) continue;
                        if (aliases[key].Length <= i) continue;
                        if (!aliases[key][i].ToLower().StartsWith(arg)) continue;
                        autoComplete = aliases[key][i];
                        break;
                    }
                    if (!string.IsNullOrEmpty(autoComplete)) break;
                }
            }

            if (string.IsNullOrEmpty(autoComplete)) autoComplete = command;
            else autoComplete += " ";
            lastCommand = command;
            lastAutoComplete = autoComplete;
            return autoComplete;
        }
    }
}