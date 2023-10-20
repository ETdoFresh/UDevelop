using System;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class DestroyCommand : Command
    {
        [SerializeField] private string objectName;
        [SerializeField] private int index;
        private GameObject instance;
        
        public DestroyCommand(string commandInput) : base(commandInput) { }
        
        public override void OnRun(params string[] args)
        {
            GetObjectNameAndIndex(args[1], out objectName, out index);
            instance = ObjectDBBehaviour.Get(objectName, index);
            instance.SetActive(false);
            ObjectDBBehaviour.Remove(objectName, index);
        }
        
        public override void OnUndo()
        {
            instance.SetActive(true);
            index = ObjectDBBehaviour.Add(objectName, instance);
        }
        
        public override void OnRedo()
        {
            instance.SetActive(false);
            ObjectDBBehaviour.Remove(objectName, index);
        }
    }
}