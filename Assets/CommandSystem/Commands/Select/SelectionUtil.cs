﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    public static class SelectionUtil
    {
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
    }
}