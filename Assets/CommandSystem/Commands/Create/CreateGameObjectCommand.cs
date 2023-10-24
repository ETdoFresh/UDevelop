using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateGameObjectCommand : Command
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateGameObjectCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created GameObject {_gameObjectName}";

        public override string[] CommandAliases => new[] { "create-gameobject", "creategameobject", "c-go" };
        public override string CommandUsage => $"{CommandAliases[0]} [GAMEOBJECT_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 3 ? "Empty" : string.Join("_", args[2..]);
            _gameObject = new GameObject(_gameObjectName);
            UnityEditor.Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = new GameObject(_gameObjectName);
            UnityEditor.Selection.activeObject = _gameObject;
        }
    }
}