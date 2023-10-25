using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectNoneCommand : Command
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectNoneCommand(string commandInput) : base(commandInput) { }
        
        public override void OnRun(params string[] args)
        {
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = Array.Empty<Object>();
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