using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectRootGameObjectsCommand : Command
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectRootGameObjectsCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            var sceneGameObjects = Object.FindObjectsOfType<GameObject>(true).Where(x => x.transform.parent == null).Cast<Object>().ToArray();
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = sceneGameObjects;
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