using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandSystem.Commands.Select;
using CommandSystem.Json;
using Newtonsoft.Json;
using UnityEngine;

namespace CommandSystem.Commands.Update
{
    [Serializable]
    public class GetValueOfSelectedGameObjectComponentCommand : Command
    {
        [SerializeField] private GameObject[] selectedGameObjects;
        [SerializeField] private string componentTypeName;
        [SerializeField] private string fieldName;

        private JsonSerializerSettings jsonSerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            Converters =
            {
                new Vector2JsonConverter(), new Vector3JsonConverter(), new Vector4JsonConverter(),
                new MaterialJsonConverter(), new ColorJsonConverter()
            }
        };

        public GetValueOfSelectedGameObjectComponentCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 3) throw new ArgumentException("Not enough arguments!");
            if (UnityEditor.Selection.gameObjects.Length == 0) throw new ArgumentException("No GameObjects selected!");
            selectedGameObjects = UnityEditor.Selection.gameObjects;
            componentTypeName = args[1];
            fieldName = args[2];
            var componentType = SelectionUtil.GetTypeByName(componentTypeName);
            var values = new List<string>();
            foreach (var gameObject in selectedGameObjects)
            {
                var component = gameObject.GetComponent(componentType);
                if (component == null)
                    throw new ArgumentException(
                        $"Component {componentTypeName} not found on GameObject {gameObject.name}!");

                var fields = component.GetType().GetFields();
                var field = fields.FirstOrDefault(x =>
                    string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (field != null)
                {
                    values.Add(GetFieldValue(component, field));
                    continue;
                }

                fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                field = fields.FirstOrDefault(x =>
                    string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (field != null)
                {
                    values.Add(GetFieldValue(component, field));
                    continue;
                }

                var properties = component.GetType().GetProperties();
                var property = properties.FirstOrDefault(x =>
                    string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (property != null)
                {
                    values.Add(GetPropertyValue(component, property));
                    continue;
                }

                properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
                property = properties.FirstOrDefault(x =>
                    string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (property != null)
                {
                    values.Add(GetPropertyValue(component, property));
                    continue;
                }

                throw new ArgumentException($"Field {fieldName} not found on Component {componentTypeName}!");
            }

            Debug.Log(string.Join(", ", values));
        }

        public override void OnUndo()
        {
            throw new NotImplementedException();
        }

        public override void OnRedo()
        {
            throw new NotImplementedException();
        }

        private string GetFieldValue(Component component, FieldInfo field)
        {
            return JsonConvert.SerializeObject(field.GetValue(component), jsonSerializerSettings);
        }

        private string GetPropertyValue(Component component, PropertyInfo property)
        {
            return JsonConvert.SerializeObject(property.GetValue(component), jsonSerializerSettings);
        }
    }
}