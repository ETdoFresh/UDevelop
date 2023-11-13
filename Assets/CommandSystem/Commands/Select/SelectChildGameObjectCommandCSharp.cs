using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectChildGameObjectCommandCSharp : CommandCSharp
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;
        
        public SelectChildGameObjectCommandCSharp(string commandInput) : base(commandInput) { }
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 1) throw new ArgumentException("Not enough arguments!");
            var indexString = "*";
            if (args.Length > 1) indexString = $"{args[1]}";
            if (indexString != "*" && !int.TryParse(indexString, out _)) throw new ArgumentException("Invalid index!");
            indexString = $"[{indexString}]";
            var currentSelection = UnityEditor.Selection.objects;
            var gameObjects = currentSelection.OfType<GameObject>().ToArray();
            if (gameObjects.Length == 0) throw new ArgumentException("No GameObjects selected!");
            var selection = new List<Object>();
            foreach (var go in gameObjects)
            {
                var childObjects = go.transform.Cast<Transform>().Select(x => x.gameObject).ToArray();
                selection.AddRange(SelectionUtil.ParseAndSelectIndex(childObjects, indexString));
            }
            _previousSelectedObjects = currentSelection;
            _selectedObjects = selection.Distinct().ToArray();
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