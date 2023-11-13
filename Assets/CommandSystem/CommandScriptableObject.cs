using UnityEngine;

namespace CommandSystem
{
    public class CommandScriptableObject : ScriptableObject
    {
        public void Run(string commandString)
        {
            CommandJsonRunner.ProcessCommandInputString(commandString);
        }
    }
}