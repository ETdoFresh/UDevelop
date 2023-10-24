using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateQuadCommand : Command
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateQuadCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Quad {_gameObjectName}";

        public override string[] CommandAliases => new[] { "create-quad", "createquad", "c-q" };
        public override string CommandUsage => $"{CommandAliases[0]} [QUAD_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 3 ? "Quad" : string.Join("_", args[2..]);
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
    }
}