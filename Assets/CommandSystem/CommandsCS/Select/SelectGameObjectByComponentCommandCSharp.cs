using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectGameObjectByComponentCommandCSharp : CommandCSharp
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectGameObjectByComponentCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var component = string.Join(" ", args[1..]);
            var componentWithoutIndex = SelectionUtil.RemoveIndexFromName(component);
            var componentType = SelectionUtil.GetTypeByName(componentWithoutIndex);
            if (componentType == null) throw new ArgumentException($"Component {componentWithoutIndex} not found!");

            var objectsByComponent = Object
                .FindObjectsOfType<GameObject>(true)
                .Where(x => x.GetComponent(componentType) != null)
                .OrderBy(SelectionUtil.GetGameObjectOrder)
                .Cast<Object>();

            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = SelectionUtil.ParseAndSelectIndex(objectsByComponent, component);
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