using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCameraCommandCSharp : CommandCSharp
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateCameraCommandCSharp(string commandInput) : base(commandInput) { }

        // public override bool AddToHistory => true;
        // public override string CommandOutput => $"Created Camera {_gameObjectName}";
        //
        // public override string[] CommandAliases => new[] { "create-camera", "createcamera", "c-c" };
        // public override string CommandUsage => $"{CommandAliases[0]} [CAMERA_NAME/PATH]";
        // public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 2 ? "Camera" : string.Join("_", args[1..]);
            _gameObject = new GameObject(_gameObjectName, typeof(Camera));
            Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = new GameObject(_gameObjectName, typeof(Camera));
            Selection.activeObject = _gameObject;
        }
    }
}