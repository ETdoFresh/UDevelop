using System;
using UnityEngine;

namespace CommandSystem.Commands.Update
{
    [Serializable]
    public class MoveCommand : Command
    {
        [SerializeField] private string objectName;
        [SerializeField] private int index;
        [SerializeField] private Vector3 initialPosition;
        [SerializeField] private Vector3 inputPosition;
        
        public MoveCommand(string command) : base(command) { }
        
        public override void OnRun(params string[] args)
        {
            GetObjectNameAndIndex(args[1], out objectName, out index);
            var instance = ObjectDBBehaviour.Get(objectName, index);
            initialPosition = instance.transform.position;
            var x = float.Parse(args[2]);
            var y = float.Parse(args[3]);
            var z = args.Length > 4 ? float.Parse(args[4]) : 0;
            inputPosition = new Vector3(x, y, z);
            instance.transform.position = inputPosition;
        }

        public override void OnUndo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.position = initialPosition;
        }
        
        public override void OnRedo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.position = inputPosition;
        }
    }
}