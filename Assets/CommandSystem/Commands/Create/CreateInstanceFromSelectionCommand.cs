using System;
using System.Linq;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateInstanceFromSelectionCommand : Command
    {
        private GameObject[] _prefabs;
        private GameObject[] _instances;

        public CreateInstanceFromSelectionCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            var selectedGameObjects = UnityEditor.Selection.gameObjects;
            _prefabs = selectedGameObjects
                .Where(UnityEditor.PrefabUtility.IsAnyPrefabInstanceRoot).ToArray();

            _instances = new GameObject[_prefabs.Length];
            for (var i = 0; i < _prefabs.Length; i++)
            {
                var prefab = _prefabs[i];
                var instance = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                instance.name = prefab.name;
                _instances[i] = instance;
            }
            UnityEditor.Selection.objects = _instances;
#else
            throw new InvalidOperationException("Create Object in Project Commands only work in Unity Editor!");
#endif
        }

        public override void OnUndo()
        {
#if UNITY_EDITOR
            foreach (var instance in _instances) 
                UnityEngine.Object.DestroyImmediate(instance);
            _instances = null;
            UnityEditor.Selection.objects = _prefabs;
#endif
        }

        public override void OnRedo()
        {
#if UNITY_EDITOR
            _instances = new GameObject[_prefabs.Length];
            for (var i = 0; i < _prefabs.Length; i++)
            {
                var prefab = _prefabs[i];
                var instance = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                instance.name = prefab.name;
                _instances[i] = instance;
            }
            UnityEditor.Selection.objects = _instances;
#endif
        }
    }
}