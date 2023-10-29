using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    public static class SelectionUtil
    {
        private static List<Type> _componentTypeCache;
        
        public static int GetGameObjectOrder(GameObject gameObject)
        {
            return gameObject.transform.root.GetSiblingIndex() * 1000000
                   + CountParents(gameObject.transform) * 1000
                   + gameObject.transform.GetSiblingIndex();
        }

        public static Object[] ParseAndSelectIndex(IEnumerable<Object> objects, string objectName)
        {
            var index = -1;
            var hasIndex = objectName.EndsWith("]") && objectName.Contains("[");
            var hasValidIndex = hasIndex && int.TryParse(objectName.Split('[')[1].Split(']')[0], out index);
            var hasWildcardIndex = hasIndex && objectName.Split('[')[1].Split(']')[0] == "*";

            if (hasWildcardIndex)
            {
                return objects.ToArray();
            }

            if (hasValidIndex)
            {
                var objectNameWithoutIndex = objectName.Split('[')[0];

                if (index < 0 || index >= objects.Count())
                    throw new IndexOutOfRangeException($"Index {index} is out of range for {objectNameWithoutIndex}!");

                return new[] { objects.ElementAt(index) };
            }

            else
            {
                return new[] { objects.FirstOrDefault() };
            }
        }

        private static int CountParents(Transform transform)
        {
            var count = 0;
            while (transform.parent != null)
            {
                count++;
                transform = transform.parent;
            }

            return count;
        }

        public static string RemoveIndexFromName(string objectName)
        {
            var hasIndex = objectName.EndsWith("]") && objectName.Contains("[");
            var hasValidIndex = hasIndex && int.TryParse(objectName.Split('[')[1].Split(']')[0], out _);
            var hasWildcardIndex = hasIndex && objectName.Split('[')[1].Split(']')[0] == "*";
            return hasValidIndex || hasWildcardIndex ? objectName.Split('[')[0] : objectName;
        }

        public static Type GetTypeByName(string typeName)
        {
            _componentTypeCache ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(Component)))
                .ToList();
            return _componentTypeCache.FirstOrDefault(x => x.FullName == typeName) ??
                   _componentTypeCache.FirstOrDefault(x =>
                       string.Equals(x.FullName, typeName, StringComparison.CurrentCultureIgnoreCase)) ??
                   _componentTypeCache.FirstOrDefault(x => x.Name == typeName) ??
                   _componentTypeCache.FirstOrDefault(x =>
                       string.Equals(x.Name, typeName, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Object[] GetGameObjectsByPath(string path)
        {
            var objectNameArray = path.Split('/');
            var foundPath = "";
            var remainingPath = path;
            List<Transform> currentParents = null;

            foreach (var objectName in objectNameArray)
            {
                var objectNameWithoutIndex = RemoveIndexFromName(objectName);
                var nextParents = new List<Transform>();

                if (currentParents == null)
                {
                    var objectsByName = Object
                        .FindObjectsOfType<GameObject>(true)
                        .Where(x => !x.transform.parent)
                        .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                            StringComparison.CurrentCultureIgnoreCase))
                        .OrderBy(GetGameObjectOrder)
                        .Cast<Object>();
                    if (!objectsByName.Any()) throw new ArgumentException($"No GameObjects found! {remainingPath}");
                    nextParents.AddRange(ParseAndSelectIndex(objectsByName, objectName)
                        .Select(x => ((GameObject)x).transform));
                }
                else
                {
                    foreach (var currentParent in currentParents)
                    {
                        var objectsByName = currentParent
                            .Cast<Transform>()
                            .Select(x => x.gameObject)
                            .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                                StringComparison.CurrentCultureIgnoreCase))
                            .OrderBy(GetGameObjectOrder)
                            .Cast<Object>();
                        if (!objectsByName.Any())
                            throw new ArgumentException($"No GameObjects found! {foundPath} -- {remainingPath}");
                        nextParents.AddRange(ParseAndSelectIndex(objectsByName, objectName)
                            .Select(x => ((GameObject)x).transform).ToArray());
                    }
                }

                currentParents = nextParents;
                foundPath += $"{objectName}";
                remainingPath = path[foundPath.Length..];
            }
            
            if (currentParents == null) throw new ArgumentException($"No GameObjects found! {foundPath}");
            return currentParents.Select(x => x.gameObject).Cast<Object>().ToArray();
        }
        

        public static void Reload()
        {
            _componentTypeCache = null;
        }
    }
}