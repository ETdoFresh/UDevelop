using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectAssetByTypeNameCommand : Command
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectAssetByTypeNameCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var typeName = string.Join(" ", args[1..]);
            var typeNameWithoutIndex = SelectionUtil.RemoveIndexFromName(typeName);

            var projectAssets = UnityEditor.AssetDatabase.FindAssets($"t:{typeNameWithoutIndex}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                .Where(x => string.Equals(x.GetType().Name, typeNameWithoutIndex,
                    StringComparison.CurrentCultureIgnoreCase));
            
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = SelectionUtil.ParseAndSelectIndex(projectAssets, typeName);
            UnityEditor.Selection.objects = _selectedObjects;
        }
    }
}