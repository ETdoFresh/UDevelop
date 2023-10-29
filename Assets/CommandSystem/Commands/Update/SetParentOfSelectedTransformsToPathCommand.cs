using System;
using System.Linq;
using CommandSystem.Commands.Select;
using UnityEngine;

namespace CommandSystem.Commands.Update
{
    [Serializable]
    public class SetParentOfSelectedTransformsToPathCommand : Command
    {
        [SerializeField] private Transform[] transforms;
        [SerializeField] private Transform[] initialParents;
        [SerializeField] private Transform newParent;
        
        public SetParentOfSelectedTransformsToPathCommand(string command) : base(command) { }
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            if (UnityEditor.Selection.transforms.Length == 0) throw new ArgumentException("No Transforms selected!");
            transforms = UnityEditor.Selection.transforms;
            initialParents = transforms.Select(x => x.parent).ToArray();
            var objectPath = string.Join(" ", args[1..]);
            var newParents = SelectionUtil.GetGameObjectsByPath(objectPath);
            if (newParents.Length == 0) throw new ArgumentException("No GameObjects found!");
            newParent = ((GameObject)newParents[0]).transform;
            foreach (var transform in transforms) 
                transform.SetParent(newParent, true);
        }
        
        public override void OnUndo()
        {
            for (var i = 0; i < transforms.Length; i++) 
                transforms[i].SetParent(initialParents[i], true);
        }
        
        public override void OnRedo()
        {
            foreach (var transform in transforms) 
                transform.SetParent(newParent, true);
        }
    }
}