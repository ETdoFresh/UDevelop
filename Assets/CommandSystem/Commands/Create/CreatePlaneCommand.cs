using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreatePlaneCommand : Command
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreatePlaneCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Plane {_gameObjectName}";

        public override string[] CommandNames => new[] { "create-plane", "createplane", "c-p" };
        public override string CommandUsage => $"{CommandNames[0]} [PLANE_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 3 ? "Plane" : string.Join("_", args[2..]);
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
    }
}