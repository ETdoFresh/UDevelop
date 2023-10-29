using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Destroy
{
    [Serializable]
    public class DestroySelectedGameObjectsCommand : Command
    {
        private GameObject[] _destroyedGameObjects;

        public DestroySelectedGameObjectsCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            _destroyedGameObjects = UnityEditor.Selection.gameObjects;
            if (_destroyedGameObjects.Length == 0) throw new ArgumentException("No GameObjects selected!");

            foreach (var destroyedGameObject in _destroyedGameObjects)
                Object.DestroyImmediate(destroyedGameObject);
        }

        public override void OnUndo()
        {
            // TODO: Add component back to the same index with the same data as before
            for (var i = 0; i < _destroyedGameObjects.Length; i++)
                _destroyedGameObjects[i] = Object.Instantiate(_destroyedGameObjects[i]);
        }

        public override void OnRedo()
        {
            foreach (var destroyedGameObject in _destroyedGameObjects)
                Object.DestroyImmediate(destroyedGameObject);
        }
    }
}