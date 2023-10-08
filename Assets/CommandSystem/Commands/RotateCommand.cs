using System;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class RotateCommand : Command
    {
        [SerializeField] private string objectName;
        [SerializeField] private int index;
        [SerializeField] private Quaternion initialRotation;
        [SerializeField] private Quaternion inputRotation;
        
        public RotateCommand(string command) : base(command) { }
        
        public override void Run(params string[] args)
        {
            GetObjectNameAndIndex(args[1], out objectName, out index);
            var instance = ObjectDBBehaviour.Get(objectName, index);
            initialRotation = instance.transform.rotation;
            var x = float.Parse(args[2]);
            var y = float.Parse(args[3]);
            var z = float.Parse(args[4]);
            var w = float.Parse(args[5]);
            inputRotation = new Quaternion(x, y, z, w);
            instance.transform.rotation = inputRotation;
        }

        public override void Undo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.rotation = initialRotation;
        }
        
        public override void Redo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.rotation = inputRotation;
        }
    }
}