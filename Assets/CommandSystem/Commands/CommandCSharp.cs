using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommandSystem
{
    [Serializable]
    public class CommandCSharp
    {
        private static readonly List<string> hasRun = new();

        [SerializeField] private string commandInput;
        [SerializeField] protected List<string> args = new();

        public string CommandInput => commandInput;

        // public virtual string CommandOutput =>
        //     CommandJsonData.Get<string>($"{TypeName}.Output") ?? $"{CommandInput} completed!";
        //
        // public virtual bool AddToHistory =>
        //     CommandJsonData.Get<bool?>($"{TypeName}.AddToHistory") ?? true;

        public virtual string Name => TypeName.EndsWith("Command")
            ? TypeName.Substring(0, TypeName.Length - 7)
            : TypeName;

        // public virtual string[] CommandAliases =>
        //     CommandJsonData.Get<string[]>($"{TypeName}.Aliases") ?? new[] { Name.ToLower() };
        //
        // public virtual string CommandUsage =>
        //     CommandJsonData.Get<string>($"{TypeName}.Usage") != null
        //         ? Name + " " + CommandJsonData.Get<string>($"{TypeName}.Usage")
        //         : Name;

        // public virtual string CommandDescription => CommandJsonData.Get<string>($"{TypeName}.Description");
        //
        // public virtual Dictionary<string, string> CommandArg1Descriptions => CommandJsonData.GetKeyAndValue<string>($"{TypeName}.Arg1.PossibleValues", "Description");
        // public virtual Dictionary<string, string> CommandArg2Descriptions => CommandJsonData.GetKeyAndValue<string>($"{TypeName}.Arg2.PossibleValues", "Description");
        
        protected string TypeName => GetType().Name;

        internal CommandCSharp() { }

        public CommandCSharp(string commandInput)
        {
            this.commandInput = commandInput;
        }

        public void Run()
        {
            var args = commandInput.Split(' ');
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