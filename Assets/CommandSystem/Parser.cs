using System.Collections.Generic;

namespace CommandSystem
{
    public static class Parser
    {
        public static string[] CommandString(string commandLine)
        {
            // Will read everything in { } as a single argument and include the braces
            // Will read everything in " " as a single argument and not include the quotes
            // Will read everything in ' ' as a single argument and not include the quotes

            var regex = new System.Text.RegularExpressions.Regex(@"\{.*?\}|'.*?'|"".*?""|\S+");
            var matches = regex.Matches(commandLine);
            var args = new string[matches.Count];
            for (var i = 0; i < matches.Count; i++)
            {
                args[i] = matches[i].Value;
            }

            return args;
        }

        public static ArgData[] GetArgData(string argString)
        {
            // Will read everything in { } as a single argument and include the braces
            // Will read everything in " " as a single argument and not include the quotes
            // Will read everything in ' ' as a single argument and not include the quotes

            var regexFlags = System.Text.RegularExpressions.RegexOptions.Multiline;
            regexFlags |= System.Text.RegularExpressions.RegexOptions.Singleline;
            var pattern = @"(?:\S*\:)\{.*?\}|\{.*?\}|'.*?'|\"".*?\""|\S+";
            var regex = new System.Text.RegularExpressions.Regex(pattern, regexFlags);
            var matches = regex.Matches(argString);
            var args = new ArgData[matches.Count];
            for (var i = 0; i < matches.Count; i++)
            {
                var arg = matches[i].Value;
                if (arg == "null")
                    args[i] = new ArgData(arg, typeof(object), null);
                else if (bool.TryParse(arg, out var boolValue))
                    args[i] = new ArgData(arg, typeof(bool), boolValue);
                else if (int.TryParse(arg, out var intValue))
                    args[i] = new ArgData(arg, typeof(int), intValue);
                else if (float.TryParse(arg, out var floatValue))
                    args[i] = new ArgData(arg, typeof(float), floatValue);
                else if (double.TryParse(arg, out var doubleValue))
                    args[i] = new ArgData(arg, typeof(double), doubleValue);
                else
                {
                    while ((arg.StartsWith("\"") && arg.EndsWith("\"")) || (arg.StartsWith("'") && arg.EndsWith("'")))
                        arg = arg[1..^1];
                    arg = arg.Replace("\\r", "\r");
                    arg = arg.Replace("\\n", "\n");
                    arg = arg.Replace("\\t", "\t");
                    args[i] = new ArgData(arg, typeof(string), arg);
                }
            }

            return args;
        }

        public static string[] MultiCommandString(string commands)
        {
            var split = commands.Split('\n');
            var commandStrings = new string[split.Length];
            for (var i = 0; i < split.Length; i++)
            {
                commandStrings[i] = split[i].Trim();
            }
            return commandStrings;
        }
        
        public static string[] CommandAliasSplit(string commandLine)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"(.*|\"".*\""|\{.*\})(?:[ \t]*>>[ \t]*(.*))|(.+)");
            var match = regex.Match(commandLine);
            var command = match.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(command)) command = match.Groups[3].Value;
            var alias = match.Groups[2].Value;
            return new[] {command, alias};
        }

        public static bool ContainsBracesVariable(string argString)
        {
            var pattern = @"\{.*?\}";
            var regex = new System.Text.RegularExpressions.Regex(pattern);
            return regex.IsMatch(argString);
        }

        public static string ReplaceBracesVariable(string argString, Dictionary<string, ArgData> argMemory)
        {
            var pattern = @"\{.*?\}";
            var regex = new System.Text.RegularExpressions.Regex(pattern);
            var matches = regex.Matches(argString);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var arg = match.Value;
                if (argMemory.TryGetValue(arg, out var argData) && (argData?.Type == typeof(string) || argData?.Value?.GetType() == typeof(string)))
                    argString = argString.Replace(arg, argData.Value.ToString());
            }
            return argString;
        }
    }
}