using System;
using System.Collections.Generic;

namespace CommandSystem
{
    public partial class CommandObject
    {
        private static Dictionary<string, ArgData> currentArgMemory;
        
        public static object RunCommandReference(string commandString)
        {
            return RunCommandReference(commandString, Array.Empty<ArgData>());
        }
        
        public static object RunCommandReference(string commandString, params ArgData[] moreArgs)
        {
            var argMemory = currentArgMemory;
            argMemory ??= new Dictionary<string, ArgData>();
            argMemory = new Dictionary<string, ArgData>(argMemory);
            
            foreach (var arg in moreArgs) 
                argMemory[arg.Name] = arg;

            var newArgMemory = CommandObject.RunCommandString(commandString, argMemory);
            return newArgMemory.TryGetValue("{Output1}", out var argData) ? argData.Value : null;
        }
    }
}