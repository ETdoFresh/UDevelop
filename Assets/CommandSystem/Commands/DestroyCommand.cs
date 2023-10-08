using System;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class DestroyCommand : Command
    {
        [SerializeField] private string objectName;
        [SerializeField] private int index;
        
        public DestroyCommand(string commandInput) : base(commandInput) { }
        
        public override void Run(params string[] args)
        {
            GetObjectNameAndIndex(args[1], out objectName, out index);
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.SetActive(false);
        }
        
        public override void Undo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.SetActive(true);
        }
        
        public override void Redo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.SetActive(false);
        }
    }
}