using System;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class ScaleCommand : Command
    {
        [SerializeField] private string objectName;
        [SerializeField] private int index;
        [SerializeField] private Vector3 initialScale;
        [SerializeField] private Vector3 inputScale;
        
        public ScaleCommand(string command) : base(command) { }
        
        public override void OnRun(params string[] args)
        {
            GetObjectNameAndIndex(args[1], out objectName, out index);
            var instance = ObjectDBBehaviour.Get(objectName, index);
            initialScale = instance.transform.localScale;
            var x = float.Parse(args[2]);
            var y = args.Length > 3 ? float.Parse(args[3]) : x;
            var z = args.Length > 4 ? float.Parse(args[4]) : x;
            inputScale = new Vector3(x, y, z);
            instance.transform.localScale = inputScale;
        }

        public override void OnUndo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.localScale = initialScale;
        }
        
        public override void OnRedo()
        {
            var instance = ObjectDBBehaviour.Get(objectName, index);
            instance.transform.localScale = inputScale;
        }
    }
}