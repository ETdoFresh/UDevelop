using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateSphereCommand : Command
    {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private string gameObjectName;

        public CreateSphereCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Sphere {gameObjectName}";

        public override string[] CommandAliases => new[] { "create-sphere", "createsphere", "c-s" };
        public override string CommandUsage => $"{CommandAliases[0]} [SPHERE_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .prefab object in project.";

        public override void OnRun(params string[] args)
        {
            gameObjectName = args.Length < 2 ? "Sphere" : string.Join("_", args[1..]);
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.name = gameObjectName;
            UnityEditor.Selection.activeObject = gameObject;
        }
        
        public override void OnUndo()
        {
            UnityEngine.Object.DestroyImmediate(gameObject);
        }
        
        public override void OnRedo()
        {
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.name = gameObjectName;
            UnityEditor.Selection.activeObject = gameObject;
        }
    }
}