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

            var index = -1;
            var hasIndex = typeName.EndsWith("]") && typeName.Contains("[");
            var hasValidIndex = hasIndex && int.TryParse(typeName.Split('[')[1].Split(']')[0], out index);
            var hasWildcardIndex = hasIndex && typeName.Split('[')[1].Split(']')[0] == "*";
            if (hasWildcardIndex)
            {
                var typeNameWithoutIndex = typeName.Split('[')[0];
                var projectAssets = UnityEditor.AssetDatabase.FindAssets($"t:{typeNameWithoutIndex}")
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                    .OrderBy(path => path)
                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                    .Where(x => string.Equals(x.GetType().Name, typeNameWithoutIndex,
                        StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();
                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = projectAssets;
                UnityEditor.Selection.objects = _selectedObjects;
            }
            else if (hasValidIndex)
            {
                var typeNameWithoutIndex = typeName.Split('[')[0];
                var projectAssets = UnityEditor.AssetDatabase.FindAssets($"t:{typeNameWithoutIndex}")
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                    .OrderBy(path => path)
                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                    .Where(x => string.Equals(x.GetType().Name, typeNameWithoutIndex,
                        StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();

                if (index < 0 || index >= projectAssets.Length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range for {typeNameWithoutIndex}!");

                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = new[] { projectAssets[index] };
                UnityEditor.Selection.objects = _selectedObjects;
            }
            else
            {
                var projectAsset = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}")
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                    .OrderBy(path => path)
                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                    .FirstOrDefault(x => string.Equals(x.GetType().Name, typeName,
                        StringComparison.CurrentCultureIgnoreCase));
                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = new[] { projectAsset };
                UnityEditor.Selection.objects = _selectedObjects;
            }
        }
    }
}