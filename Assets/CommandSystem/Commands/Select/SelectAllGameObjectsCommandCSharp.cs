using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectAllGameObjectsCommandCSharp : CommandCSharp
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectAllGameObjectsCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            var sceneGameObjects = Object
                .FindObjectsOfType<GameObject>(true)
                .Cast<Object>().ToArray();
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = sceneGameObjects;
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