using System;
using System.Linq;
using System.Reflection;
using CommandSystem.Commands.Select;
using Newtonsoft.Json;
using UnityEngine;

namespace CommandSystem.Commands.Update
{
    [Serializable]
    public class SetValueOfSelectedGameObjectComponentCommand : Command
    {
        [SerializeField] private GameObject[] gameObjects;
        [SerializeField] private string componentName;
        [SerializeField] private string fieldName;
        [SerializeField] private string fieldValue;
        
        public SetValueOfSelectedGameObjectComponentCommand(string command) : base(command) { }
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 4) throw new ArgumentException("Not enough arguments!");
            if (UnityEditor.Selection.gameObjects.Length == 0) throw new ArgumentException("No GameObjects selected!");
            gameObjects = UnityEditor.Selection.gameObjects;
            componentName = args[1];
            fieldName = args[2];
            fieldValue = string.Join(" ", args.Skip(3));
            foreach (var gameObject in gameObjects)
            {
                var componentType = SelectionUtil.GetTypeByName(componentName);
                var component = gameObject.GetComponent(componentType);
                if (component == null) throw new ArgumentException($"Component {componentName} not found on GameObject {gameObject.name}!");
                var fields = component.GetType().GetFields();
                var field = fields.FirstOrDefault(x => string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (field != null)
                {
                    SetFieldValue(component, field, fieldValue);
                    continue;
                }
                fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                field = fields.FirstOrDefault(x => string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (field != null)
                {
                    SetFieldValue(component, field, fieldValue);
                    continue;
                }
                var properties = component.GetType().GetProperties();
                var property = properties.FirstOrDefault(x => string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (property != null)
                {
                    SetPropertyValue(component, property, fieldValue);
                    continue;
                }
                properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
                property = properties.FirstOrDefault(x => string.Equals(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (property != null)
                {
                    SetPropertyValue(component, property, fieldValue);
                    continue;
                }
                throw new ArgumentException($"Field {fieldName} not found on Component {componentName}!");
            }
        }

        private void SetFieldValue(Component component, FieldInfo field, string s)
        {
            field.SetValue(component, JsonConvert.DeserializeObject(s, field.FieldType));
        }
        
        private void SetPropertyValue(Component component, PropertyInfo property, string s)
        {
            property.SetValue(component, JsonConvert.DeserializeObject(s, property.PropertyType));
        }
    }
}