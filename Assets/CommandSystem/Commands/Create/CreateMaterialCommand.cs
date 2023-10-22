﻿using System;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateMaterialCommand : Command
    {
        private string _materialName;
        private string _materialPath;

        public CreateMaterialCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Material {_materialName}";

        public override string[] CommandNames => new[] { "create-material", "creatematerial", "c-m" };
        public override string CommandUsage => $"{CommandNames[0]} [MATERIAL_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .mat object in project.";

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            var materialNameOrPath = args.Length < 3 ? null : string.Join("_", args[2..]);
            _materialPath = CommandAsset.ResolvePath(materialNameOrPath, "Material", ".mat");
            _materialName = CommandAsset.GetNameFromPath(_materialPath);
            var material = new UnityEngine.Material(UnityEngine.Shader.Find("Standard"));
            UnityEditor.AssetDatabase.CreateAsset(material, _materialPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Selection.activeObject = material;
#endif
        }

        public override void OnUndo()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.DeleteAsset(_materialPath);
#endif
        }

        public override void OnRedo()
        {
#if UNITY_EDITOR
            var material = new UnityEngine.Material(UnityEngine.Shader.Find("Standard"));
            UnityEditor.AssetDatabase.CreateAsset(material, _materialPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Selection.activeObject = material;
#endif
        }
    }
}