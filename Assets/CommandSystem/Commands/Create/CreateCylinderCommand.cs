using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCylinderCommand : Command
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateCylinderCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Cylinder {_gameObjectName}";

        public override string[] CommandAliases => new[] { "create-cylinder", "createcylinder", "c-c" };
        public override string CommandUsage => $"{CommandAliases[0]} [CYLINDER_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 2 ? "Cylinder" : string.Join("_", args[1..]);
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
    }
}