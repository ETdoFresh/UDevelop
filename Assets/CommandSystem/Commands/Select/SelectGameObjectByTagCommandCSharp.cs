using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectGameObjectByTagCommandCSharp : CommandCSharp {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectGameObjectByTagCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var tag = string.Join(" ", args[1..]);
            var tagWithoutIndex = SelectionUtil.RemoveIndexFromName(tag);
            
            var objectsByTag = Object
                .FindObjectsOfType<GameObject>(true)
                .Where(x => string.Equals(x.tag, tagWithoutIndex,
                    StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(SelectionUtil.GetGameObjectOrder)
                .Cast<Object>();
            
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = SelectionUtil.ParseAndSelectIndex(objectsByTag, tag);
            UnityEditor.Selection.objects = _selectedObjects;
        }

        public override void OnUndo()
        {
            UnityEditor.Selection.objects = _previousSelectedObjects;
        }

        public override void OnRedo()
        {
            UnityEditor.Selection.objects = _selectedObjects;
        }
    }
}