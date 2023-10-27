using System;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectCommand : Command
    {
        private Object[] _previousSelectedObjects;
        private Object[] _selectedObjects;
        
        public SelectCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");

            var selectType = args[1].ToLower();

            switch ()
            {
                case "all":
                    
            }
        }
}