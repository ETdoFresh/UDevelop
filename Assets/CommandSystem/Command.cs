using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommandSystem
{
    [Serializable]
    public class Command
    {
        private static readonly List<string> hasRun = new();
        
        [SerializeField] private string commandInput;
        [SerializeField] protected List<string> args = new();
        
        public string CommandInput => commandInput;

        public Command(string commandInput)
        {
            this.commandInput = commandInput;
            var args = commandInput.Split(' ', ',');
            var commandTypeName = GetType().Name;
            this.args.AddRange(args);
            if (!hasRun.Contains(commandTypeName))
            {
                FirstRun();
                hasRun.Add(commandTypeName);
            }
            Run(args);
        }

        public virtual void FirstRun() { }

        public virtual void Run(params string[] args) { }

        public virtual void Undo() { }

        public virtual void Redo() { }
        
        protected void GetObjectNameAndIndex(string str, out string objectName, out int index)
        {
            objectName = str;
            index = 0;
            var indexStart = objectName.IndexOf('[');
            var indexEnd = objectName.IndexOf(']');
            if (indexStart == -1 || indexEnd == -1) return;
            var indexString = objectName.Substring(indexStart + 1, indexEnd - indexStart - 1);
            objectName = objectName.Substring(0, indexStart);
            index = int.Parse(indexString);
        }
    }
}