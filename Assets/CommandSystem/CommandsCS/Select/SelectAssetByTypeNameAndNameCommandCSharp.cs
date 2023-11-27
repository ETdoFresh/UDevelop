using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectAssetByTypeNameAndNameCommandCSharp : CommandCSharp
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectAssetByTypeNameAndNameCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 3) throw new ArgumentException("Not enough arguments!");
            var typeName = string.Join(" ", args[1]);
            var assetName = string.Join(" ", args[2..]);
            var assetNameWithoutIndex = SelectionUtil.RemoveIndexFromName(assetName);

#if UNITY_EDITOR
            var projectAssets = UnityEditor.AssetDatabase.FindAssets($"t:{typeName} {assetNameWithoutIndex}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                .Where(x => string.Equals(x.GetType().Name, typeName,
                    StringComparison.CurrentCultureIgnoreCase));
#else
            var projectAssets = Enumerable.Empty<Object>();
#endif
            _previousSelectedObjects = Selection.objects;
            _selectedObjects = SelectionUtil.ParseAndSelectIndex(projectAssets, assetName);
            Selection.objects = _selectedObjects;
        }

        public override void OnUndo()
        {
            Selection.objects = _previousSelectedObjects;
        }

        public override void OnRedo()
        {
            Selection.objects = _selectedObjects;
        }
    }
}