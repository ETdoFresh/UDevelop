using System;
using System.Linq;
using UnityEngine;

namespace CommandSystem.Commands.Update
{
    [Serializable]
    public class RotateSelectedTransformsCommand : Command
    {
        [SerializeField] private Transform[] transforms;
        [SerializeField] private Vector3[] initialRotations;
        [SerializeField] private Vector3 inputRotation;

        public RotateSelectedTransformsCommand(string command) : base(command) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            if (UnityEditor.Selection.transforms.Length == 0) throw new ArgumentException("No Transforms selected!");
            transforms = UnityEditor.Selection.transforms;
            initialRotations = transforms.Select(x => x.eulerAngles).ToArray();
            if (args.Length < 5)
            {
                var x = args.Length > 3 ? float.Parse(args[1]) : 0;
                var y = args.Length == 3 ? float.Parse(args[2]) : args.Length > 3 ? float.Parse(args[2]) : 0;
                var z = args.Length == 2 ? float.Parse(args[1]) : args.Length > 3 ? float.Parse(args[3]) : 0;
                inputRotation = new Vector3(x, y, z);
                foreach (var transform in transforms)
                    transform.eulerAngles = inputRotation;
            }
            else
            {
                var x = float.Parse(args[1]);
                var y = float.Parse(args[2]);
                var z = float.Parse(args[3]);
                var w = float.Parse(args[4]);
                var quaternion = new Quaternion(x, y, z, w);
                inputRotation = quaternion.eulerAngles;
                foreach (var transform in transforms)
                    transform.rotation = quaternion;
            }
        }

        public override void OnUndo()
        {
            for (var i = 0; i < transforms.Length; i++)
                transforms[i].eulerAngles = initialRotations[i];
        }

        public override void OnRedo()
        {
            foreach (var transform in transforms)
                transform.eulerAngles = inputRotation;
        }
    }
}