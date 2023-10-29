using System;
using System.Linq;
using UnityEngine;

namespace CommandSystem.Commands.Update
{
    [Serializable]
    public class MoveSelectedTransformsCommand : Command
    {
        [SerializeField] private Transform[] transforms;
        [SerializeField] private Vector3[] initialPositions;
        [SerializeField] private Vector3 inputPosition;
        
        public MoveSelectedTransformsCommand(string command) : base(command) { }
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            if (UnityEditor.Selection.transforms.Length == 0) throw new ArgumentException("No Transforms selected!");
            transforms = UnityEditor.Selection.transforms;
            initialPositions = transforms.Select(x => x.position).ToArray();
            var x = float.Parse(args[1]);
            var y = args.Length > 2 ? float.Parse(args[2]) : 0;
            var z = args.Length > 3 ? float.Parse(args[3]) : 0;
            inputPosition = new Vector3(x, y, z);
            foreach (var transform in transforms) 
                transform.position = inputPosition;
        }

        public override void OnUndo()
        {
            for (var i = 0; i < transforms.Length; i++) 
                transforms[i].position = initialPositions[i];
        }
        
        public override void OnRedo()
        {
            foreach (var transform in transforms) 
                transform.position = inputPosition;
        }
    }
}