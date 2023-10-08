using System;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class RotateCommand : Command
    {
        [SerializeField] private string objectName;
        [SerializeField] private int index;
        [SerializeField] private Vector3 initialRotation;
        [SerializeField] private Vector3 inputRotation;
        
        public RotateCommand(string command) : base(command) { }
        
        public override void Run(params string[] args)
        {
            GetObjectNameAndIndex(args[1], out objectName, out index);
            var instance = ObjectDBBehaviour.Get(objectName, index);
            initialRotation = instance.transform.eulerAngles;
            var x = args.Length > 3 ? float.Parse(args[2]) : 0;
            var y = args.Length > 3 ? float.Parse(args[3]) : 0;
            var z = args.Length == 3 ? float.Parse(args[2]) : args.Length > 4 ? float.Parse(args[4]) : 0;
            inputRotation = new Vector3(x, y, z);
            instance.transform.eulerAngles = inputRotation;
        }

        public override void Undo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.eulerAngles = initialRotation;
        }
        
        public override void Redo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.eulerAngles = inputRotation;
        }
    }
}