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
            var commandName = args[0].ToLower();
            this.args.AddRange(args);
            if (!hasRun.Contains(commandName))
            {
                FirstRun();
                hasRun.Add(commandName);
            }
            Run(args);
        }

        public virtual void FirstRun() { }

        public virtual void Run(params string[] args) { }

        public virtual void Undo() { }

        public virtual void Redo() { }
    }
}