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
            
            var regex = new System.Text.RegularExpressions.Regex(@"\{.*?\}|'.*?'|"".*?""|\S+");
            var matches = regex.Matches(argString);
            var args = new ArgData[matches.Count];
            for (var i = 0; i < matches.Count; i++)
            {
                var arg = matches[i].Value;
                if (arg == "null")
                    args[i] = new ArgData(arg, typeof(object), null, false);
                else if (bool.TryParse(arg, out var boolValue))
                    args[i] = new ArgData(arg, typeof(bool), boolValue, false);
                else if (int.TryParse(arg, out var intValue))
                    args[i] = new ArgData(arg, typeof(int), intValue, false);
                else if (float.TryParse(arg, out var floatValue))
                    args[i] = new ArgData(arg, typeof(float), floatValue, false);
                else if (double.TryParse(arg, out var doubleValue))
                    args[i] = new ArgData(arg, typeof(double), doubleValue, false);
                else
                    args[i] = new ArgData(arg, typeof(string), arg, false);
            }

            return args;
        }
    }
}