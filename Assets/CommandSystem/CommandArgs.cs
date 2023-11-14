using System;
using System.Collections.Generic;

namespace CommandSystem.Commands
{
    public static class CommandArgs
    {
        public static Dictionary<string, string[]> GetArgAliases(Type commandType, int arg)
        {
            return CommandJsonData.GetKeyAndValue<string[]>($"{commandType.Name}.Arg{arg}.PossibleValues", "Aliases");
        }
    }
}