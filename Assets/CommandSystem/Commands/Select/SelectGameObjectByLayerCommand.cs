using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectGameObjectByLayerCommand : Command {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectGameObjectByLayerCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var layer = string.Join(" ", args[1..]);
            var layerWithoutIndex = SelectionUtil.RemoveIndexFromName(layer);
            
            var objectsByLayer = Object
                .FindObjectsOfType<GameObject>(true)
                .Where(x => string.Equals(LayerMask.LayerToName(x.layer), layerWithoutIndex,
                    StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(SelectionUtil.GetGameObjectOrder)
                .Cast<Object>();
            
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = SelectionUtil.ParseAndSelectIndex(objectsByLayer, layer);
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