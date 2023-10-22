using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCapsuleCommand : Command
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateCapsuleCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Capsule {_gameObjectName}";

        public override string[] CommandNames => new[] { "create-capsule", "createcapsule", "c-c" };
        public override string CommandUsage => $"{CommandNames[0]} [CAPSULE_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 3 ? "Capsule" : string.Join("_", args[2..]);
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
    }
}