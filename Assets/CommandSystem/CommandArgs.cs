using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem.Commands
{
    public static class CommandArgs
    {
        public static string GetArgByAlias(Type commandType, int arg, string alias)
        {
            var aliases =
                CommandJsonData.GetKeyAndValue<string[]>($"{commandType.Name}.Arg{arg}.PossibleValues", "Aliases");
            
            if (aliases == null) return null;
            
            foreach (var entry in aliases)
            {
                if (entry.Value == null) continue;
                if (entry.Value.Contains(alias))
                    return entry.Key;
            }
            
            return null;
        }
        
        public static Dictionary<string, string[]> GetArgAliases(Type commandType, int arg)
        {
            return CommandJsonData.GetKeyAndValue<string[]>($"{commandType.Name}.Arg{arg}.PossibleValues", "Aliases");
        }
    }
}