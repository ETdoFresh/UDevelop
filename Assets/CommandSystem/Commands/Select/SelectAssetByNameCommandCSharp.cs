using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectAssetByNameCommandCSharp : CommandCSharp
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectAssetByNameCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var objectName = string.Join(" ", args[1..]);
            var objectNameWithoutIndex = SelectionUtil.RemoveIndexFromName(objectName);

            var projectAssets = UnityEditor.AssetDatabase.FindAssets(objectNameWithoutIndex)
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                    StringComparison.CurrentCultureIgnoreCase));

            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = SelectionUtil.ParseAndSelectIndex(projectAssets, objectName);
            UnityEditor.Selection.objects = _selectedObjects;
        }
    }
}