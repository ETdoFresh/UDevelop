using System;
using System.Linq;
using CommandSystem.Commands.Select;
using UnityEngine;
using Object = UnityEngine.Object;
using static System.Reflection.BindingFlags;

namespace CommandSystem.Commands
{
    public static class Utility
    {
        public static string GetGameObjectScenePath(GameObject gameObject)
        {
            var path = gameObject.name;
            var parent = gameObject.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path; 
        }
        
        public static GameObject FindGameObjectByName(string gameObjectName)
        {
            var objectNameWithoutIndex = SelectionUtil.RemoveIndexFromName(gameObjectName);
            const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;
            var objectsByName = Object
                .FindObjectsOfType<GameObject>(true)
                .Where(x => string.Equals(x.name, objectNameWithoutIndex, ignoreCase))
                .OrderBy(SelectionUtil.GetGameObjectOrder)
                .Cast<Object>();
            var selectedObjects = SelectionUtil.ParseAndSelectIndex(objectsByName, gameObjectName);
            return selectedObjects.FirstOrDefault() as GameObject;
        }
        
        public static void SetParent(GameObject child, GameObject parent)
        {
            var childTransform = child.transform;
            var parentTransform = parent.transform;
            childTransform.SetParent(parentTransform);
        }
        
        public static object GetValue(object obj, string name)
        {
            var type = obj.GetType();
            var field = type.GetField(name, Public | NonPublic | Instance);
            if (field != null)
            {
                return field.GetValue(obj);
            }

            var property = type.GetProperty(name, Public | NonPublic | Instance);
            if (property != null)
            {
                return property.GetValue(obj);
            }

            return null;
        }
        
        public static Type FindSystemTypeByName(string typeNameString)
        {
            return StringToTypeUtility.Get(typeNameString);
        }
    }
}