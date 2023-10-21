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

        public virtual string CommandOutput => $"{commandInput} Complete!";
        
        public virtual bool AddToHistory => true;

        public virtual string[] CommandNames => new[]
        {
            GetType().Name.EndsWith("Command")
                ? GetType().Name.Substring(0, GetType().Name.Length - 7).ToLower()
                : GetType().Name.ToLower()
        };

        internal Command() { }

        public Command(string commandInput)
        {
            this.commandInput = commandInput;
        }

        public void Run()
        {
            var args = commandInput.Split(' ', ',');
            var commandTypeName = GetType().Name;
            this.args.AddRange(args);
            if (!hasRun.Contains(commandTypeName))
            {
                OnFirstRun();
                hasRun.Add(commandTypeName);
            }

            OnRun(args);
        }

        public virtual void OnFirstRun() { }

        public virtual void OnRun(params string[] args) { }

        public virtual void OnUndo() { }

        public virtual void OnRedo() { }

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