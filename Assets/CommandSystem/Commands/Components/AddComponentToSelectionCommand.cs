using System;
using CommandSystem.Commands.Select;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Components
{
    [Serializable]
    public class AddComponentToSelectionCommand : Command
    {
        private GameObject[] _selectedGameObjects;
        private Component[] _newComponents;
        private Type _componentType;

        public AddComponentToSelectionCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            _selectedGameObjects = UnityEditor.Selection.gameObjects;
            _newComponents = new Component[_selectedGameObjects.Length];
            _componentType = SelectionUtil.GetTypeByName(args[1]);
            if (_componentType == null) throw new ArgumentException($"Component {args[1]} not found!");
            for (var i = 0; i < _selectedGameObjects.Length; i++)
                _newComponents[i] = _selectedGameObjects[i].AddComponent(_componentType);
        }

        public override void OnUndo()
        {
            foreach (var component in _newComponents)
                Object.DestroyImmediate(component);
            
            _newComponents = null;
        }
        
        public override void OnRedo()
        {
            for (var i = 0; i < _selectedGameObjects.Length; i++)
                _newComponents[i] = _selectedGameObjects[i].AddComponent(_componentType);
        }
    }
}