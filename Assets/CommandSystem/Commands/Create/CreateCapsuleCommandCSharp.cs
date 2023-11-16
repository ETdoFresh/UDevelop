using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCapsuleCommandCSharp : CommandCSharp
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateCapsuleCommandCSharp(string commandInput) : base(commandInput) { }

        // public override bool AddToHistory => true;
        // public override string CommandOutput => $"Created Capsule {_gameObjectName}";
        //
        // public override string[] CommandAliases => new[] { "create-capsule", "createcapsule", "c-c" };
        // public override string CommandUsage => $"{CommandAliases[0]} [CAPSULE_NAME/PATH]";
        // public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 2 ? "Capsule" : string.Join("_", args[1..]);
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