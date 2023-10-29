using System;
using System.Linq;
using UnityEngine;

namespace CommandSystem.Commands.Update
{
    [Serializable]
    public class ScaleSelectedTransformsCommand : Command
    {
        [SerializeField] private Transform[] transforms;
        [SerializeField] private Vector3[] initialScales;
        [SerializeField] private Vector3 inputScale;
        
        public ScaleSelectedTransformsCommand(string command) : base(command) { }
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            if (UnityEditor.Selection.transforms.Length == 0) throw new ArgumentException("No Transforms selected!");
            transforms = UnityEditor.Selection.transforms;
            initialScales = transforms.Select(x => x.localScale).ToArray();
            var x = float.Parse(args[1]);
            var y = args.Length > 2 ? float.Parse(args[2]) : x;
            var z = args.Length > 3 ? float.Parse(args[3]) : x;
            inputScale = new Vector3(x, y, z);
            foreach (var transform in transforms) 
                transform.localScale = inputScale;
        }

        public override void OnUndo()
        {
            for (var i = 0; i < transforms.Length; i++) 
                transforms[i].localScale = initialScales[i];
        }
        
        public override void OnRedo()
        {
            foreach (var transform in transforms) 
                transform.localScale = inputScale;
        }
    }
}