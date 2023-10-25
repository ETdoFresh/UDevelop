using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectGameObjectByNameCommand : Command
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectGameObjectByNameCommand(string commandInput) : base(commandInput) { }

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
                var sceneGameObjects = Object.FindObjectsOfType<GameObject>(true)
                    .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                        StringComparison.CurrentCultureIgnoreCase))
                    .Cast<Object>()
                    .ToArray();
                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = sceneGameObjects;
                UnityEditor.Selection.objects = _selectedObjects;
            }
            else if (hasValidIndex)
            {
                var objectNameWithoutIndex = objectName.Split('[')[0];
                var sceneGameObjects = Object.FindObjectsOfType<GameObject>(true)
                    .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                        StringComparison.CurrentCultureIgnoreCase))
                    .OrderBy(x => x.transform.root.GetSiblingIndex() * 1000000 + CountParents(x.transform) * 1000 +
                                  x.transform.GetSiblingIndex())
                    .Cast<Object>()
                    .ToArray();

                if (index < 0 || index >= sceneGameObjects.Length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range for {objectNameWithoutIndex}!");

                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = new[] { sceneGameObjects[index] };
                UnityEditor.Selection.objects = _selectedObjects;
            }
            else
            {
                var sceneGameObject = Object.FindObjectsOfType<GameObject>(true)
                    .Where(x => string.Equals(x.name, objectName,
                        StringComparison.CurrentCultureIgnoreCase))
                    .OrderBy(x => x.transform.root.GetSiblingIndex() * 1000000 + CountParents(x.transform) * 1000 +
                                  x.transform.GetSiblingIndex())
                    .Cast<Object>()
                    .FirstOrDefault();
                _previousSelectedObjects = UnityEditor.Selection.objects;
                _selectedObjects = new[] { sceneGameObject };
                UnityEditor.Selection.objects = _selectedObjects;
            }
        }

        public override void OnUndo()
        {
            UnityEditor.Selection.objects = _previousSelectedObjects;
        }

        public override void OnRedo()
        {
            UnityEditor.Selection.objects = _selectedObjects;
        }

        private int CountParents(Transform transform)
        {
            var count = 0;
            while (transform.parent != null)
            {
                count++;
                transform = transform.parent;
            }

            return count;
        }
    }
}