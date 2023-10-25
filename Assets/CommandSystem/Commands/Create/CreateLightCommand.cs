using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateLightCommand : Command
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateLightCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Light {_gameObjectName}";

        public override string[] CommandAliases => new[] { "create-light", "createlight", "c-l" };
        public override string CommandUsage => $"{CommandAliases[0]} [LIGHT_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 2 ? "Light" : string.Join("_", args[1..]);
            _gameObject = new GameObject(_gameObjectName, typeof(Light));
            UnityEditor.Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = new GameObject(_gameObjectName, typeof(Light));
            UnityEditor.Selection.activeObject = _gameObject;
        }
    }
}