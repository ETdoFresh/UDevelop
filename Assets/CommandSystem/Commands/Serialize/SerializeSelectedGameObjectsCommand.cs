using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Serialize
{
    [Serializable]
    public class SerializeSelectedGameObjectsCommand : Command
    {
        [SerializeField] private string _serializedCommand;
        
        public SerializeSelectedGameObjectsCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            // Serializes the selected game object to commands
            var gameObjects = UnityEditor.Selection.gameObjects;
            foreach(var gameObject in gameObjects)
            {
                _serializedCommand += _serializedCommand == null ? "" : "\n";
                _serializedCommand += $"CreateGameObject {gameObject.name}";
                _serializedCommand += $"\nSetValue Transform Position {{x: {gameObject.transform.position.x}, y: {gameObject.transform.position.y}, z: {gameObject.transform.position.z}}}";
                _serializedCommand += $"\nSetValue Transform Rotation {{x: {gameObject.transform.rotation.x}, y: {gameObject.transform.rotation.y}, z: {gameObject.transform.rotation.z}, w: {gameObject.transform.rotation.w}}}";
                _serializedCommand += $"\nSetValue Transform LocalScale {{x: {gameObject.transform.localScale.x}, y: {gameObject.transform.localScale.y}, z: {gameObject.transform.localScale.z}}}";
                foreach(var component in gameObject.GetComponents<Component>().OrderBy(GetComponentSerializeOrder))
                {
                    if(!component) continue;
                    if (component is Transform) continue;
                    var componentName = component.GetType().Name;
                    _serializedCommand += $"\nAddComponent {componentName}";
                    foreach(var field in component.GetType().GetFields())
                    {
                        if (field.CustomAttributes.Any(x => x.AttributeType == typeof(ObsoleteAttribute))) continue;
                        if (field.DeclaringType == typeof(Component)) continue;
                        if (field.DeclaringType == typeof(Object)) continue;
                        var fieldValue = field.GetValue(component);
                        _serializedCommand += $"\nSetValue {componentName} {field.Name} {fieldValue}";
                    }
                    foreach(var property in component.GetType().GetProperties())
                    {
                        if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ObsoleteAttribute))) continue;
                        if (property.DeclaringType == typeof(Component)) continue;
                        if (property.DeclaringType == typeof(Object)) continue;
                        if (!property.CanWrite) continue;
                        if (!property.CanRead) continue;
                        var propertyValue = property.GetValue(component);
                        _serializedCommand += $"\nSetValue {componentName} {property.Name} {propertyValue}";
                    }
                }
            }
            Debug.Log(_serializedCommand);
        }

        private int GetComponentSerializeOrder(Component component)
        {
            if (component is Transform) return 0;
            if (component is MeshFilter) return 1;
            if (component is MeshRenderer) return 2;
            if (component is Collider) return 3;
            if (component is Rigidbody) return 4;
            if (component is MonoBehaviour) return 5;
            return 6;
        }
    }
}