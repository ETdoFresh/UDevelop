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
            
            var projectAssets = UnityEditor.AssetDatabase.FindAssets($"t:{typeName} {assetNameWithoutIndex}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                .Where(x => string.Equals(x.GetType().Name, typeName,
                    StringComparison.CurrentCultureIgnoreCase));
            
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = SelectionUtil.ParseAndSelectIndex(projectAssets, assetName);
            UnityEditor.Selection.objects = _selectedObjects;
        }
        
        public override void OnUndo()
        {
            UnityEditor.Selection.objects = _previousSelectedObjects;
        }
        
        public override void OnRedo()
        {
            UnityEditor.Selection.objects = _selectedObjects;
        }
    }
}