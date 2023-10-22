﻿using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCubeCommand : Command
    {
        private GameObject _gameObject;
        private string _gameObjectName;

        public CreateCubeCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Cube {_gameObjectName}";

        public override string[] CommandNames => new[] { "create-cube", "createcube", "c-c" };
        public override string CommandUsage => $"{CommandNames[0]} [CUBE_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            _gameObjectName = args.Length < 3 ? "Cube" : string.Join("_", args[2..]);
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }
        
        public override void OnRedo()
        {
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _gameObject.name = _gameObjectName;
            UnityEditor.Selection.activeObject = _gameObject;
        }
    }
}