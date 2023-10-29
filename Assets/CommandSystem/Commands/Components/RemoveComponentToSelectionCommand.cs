using System;
using CommandSystem.Commands.Select;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Components
{
    [Serializable]
    public class RemoveComponentToSelectionCommand : Command
    {
        private GameObject[] _selectedGameObjects;
        private Component[] _removedComponents;
        private Type _componentType;

        public RemoveComponentToSelectionCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            _selectedGameObjects = UnityEditor.Selection.gameObjects;
            _removedComponents = new Component[_selectedGameObjects.Length];
            _componentType = SelectionUtil.GetTypeByName(args[1]);
            if (_componentType == null) throw new ArgumentException($"Component {args[1]} not found!");
            
            for (var i = 0; i < _selectedGameObjects.Length; i++)
                _removedComponents[i] = _selectedGameObjects[i].GetComponent(_componentType);
            
            foreach (var component in _removedComponents)
                Object.DestroyImmediate(component);
        }

        public override void OnUndo()
        {
            // TODO: Add component back to the same index with the same data as before
            for (var i = 0; i < _selectedGameObjects.Length; i++)
                _removedComponents[i] = _selectedGameObjects[i].AddComponent(_componentType);
        }
        
        public override void OnRedo()
        {
            foreach (var component in _removedComponents)
                Object.DestroyImmediate(component);
        }
    }
}