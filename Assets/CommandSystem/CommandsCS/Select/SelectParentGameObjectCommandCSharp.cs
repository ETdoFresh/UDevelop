using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectParentGameObjectCommandCSharp : CommandCSharp
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;
        
        public SelectParentGameObjectCommandCSharp(string commandInput) : base(commandInput) { }
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 1) throw new ArgumentException("Not enough arguments!");
            var currentSelection = UnityEditor.Selection.objects;
            var gameObjects = currentSelection.OfType<GameObject>().ToArray();
            if (gameObjects.Length == 0) throw new ArgumentException("No GameObjects selected!");
            var parentObjects = gameObjects.Select(x => x.transform.parent).Where(x => x != null).Select(x => x.gameObject).ToArray();
            if (parentObjects.Length == 0) throw new ArgumentException("No parent GameObjects found!");
            _previousSelectedObjects = currentSelection;
            _selectedObjects = parentObjects.Cast<Object>().ToArray();
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