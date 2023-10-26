using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectAssetByNameCommand : Command
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;
        
        public SelectAssetByNameCommand(string commandInput) : base(commandInput) { }
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var objectName = string.Join(" ", args[1..]);
            
            var index = -1;
            var hasIndex = objectName.EndsWith("]") && objectName.Contains("[");
            var hasValidIndex = hasIndex && int.TryParse(objectName.Split('[')[1].Split(']')[0], out index);
            var hasWildcardIndex = hasIndex && objectName.Split('[')[1].Split(']')[0] == "*";
            if (hasWildcardIndex)
            {
                var objectNameWithoutIndex = objectName.Split('[')[0];
                var projectAssets = UnityEditor.AssetDatabase.FindAssets(objectNameWithoutIndex)
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                    .OrderBy(path => path)
                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                    .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                        StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();
                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = projectAssets;
                UnityEditor.Selection.objects = _selectedObjects;
            }
            else if (hasValidIndex)
            {
                var objectNameWithoutIndex = objectName.Split('[')[0];
                var projectAssets = UnityEditor.AssetDatabase.FindAssets(objectNameWithoutIndex)
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                    .OrderBy(path => path)
                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                    .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                        StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();

                if (index < 0 || index >= projectAssets.Length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range for {objectNameWithoutIndex}!");

                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = new[] { projectAssets[index] };
                UnityEditor.Selection.objects = _selectedObjects;
            }
            else
            {
                var projectAsset = UnityEditor.AssetDatabase.FindAssets(objectName)
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                    .OrderBy(path => path)
                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                    .FirstOrDefault(x => string.Equals(x.name, objectName,
                        StringComparison.CurrentCultureIgnoreCase));
                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = new[] { projectAsset };
                UnityEditor.Selection.objects = _selectedObjects;
            }
        }
    }
}