using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCubeCommand : Command
    {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private string gameObjectName;

        public CreateCubeCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Cube {gameObjectName}";

        public override string[] CommandAliases => new[] { "create-cube", "createcube", "c-c" };
        public override string CommandUsage => $"{CommandAliases[0]} [CUBE_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            gameObjectName = args.Length < 3 ? "Cube" : string.Join("_", args[2..]);
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = gameObjectName;
            UnityEditor.Selection.activeObject = gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(gameObject);
        }
        
        public override void OnRedo()
        {
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = gameObjectName;
            UnityEditor.Selection.activeObject = gameObject;
        }
    }
}