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
            fieldValue = args[3];
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
            if (field.FieldType == typeof(int))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<int>(s));
                return;
            }
            if (field.FieldType == typeof(float))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<float>(s));
                return;
            }
            if (field.FieldType == typeof(double))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<double>(s));
                return;
            }
            if (field.FieldType == typeof(string))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<string>(s));
                return;
            }
            if (field.FieldType == typeof(bool))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<bool>(s));
                return;
            }
            if (field.FieldType == typeof(Vector2))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<Vector2>(s));
                return;
            }
            if (field.FieldType == typeof(Vector3))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<Vector3>(s));
                return;
            }
            if (field.FieldType == typeof(Vector4))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<Vector4>(s));
                return;
            }
            if (field.FieldType == typeof(Quaternion))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<Quaternion>(s));
                return;
            }
            if (field.FieldType == typeof(Color))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<Color>(s));
                return;
            }
            if (field.FieldType == typeof(Color32))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<Color32>(s));
                return;
            }
            if (field.FieldType == typeof(Rect))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<Rect>(s));
                return;
            }
            if (field.FieldType == typeof(RectOffset))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<RectOffset>(s));
                return;
            }
            if (field.FieldType == typeof(AnimationCurve))
            {
                field.SetValue(component, JsonConvert.DeserializeObject<AnimationCurve>(s));
                return;
            }
            throw new ArgumentException($"Field {fieldName} has unsupported type {field.FieldType}!");
        }
        
        private void SetPropertyValue(Component component, PropertyInfo property, string s)
        {
            if (property.PropertyType == typeof(int))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<int>(s));
                return;
            }
            if (property.PropertyType == typeof(float))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<float>(s));
                return;
            }
            if (property.PropertyType == typeof(double))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<double>(s));
                return;
            }
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<string>(s));
                return;
            }
            if (property.PropertyType == typeof(bool))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<bool>(s));
                return;
            }
            if (property.PropertyType == typeof(Vector2))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<Vector2>(s));
                return;
            }
            if (property.PropertyType == typeof(Vector3))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<Vector3>(s));
                return;
            }
            if (property.PropertyType == typeof(Vector4))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<Vector4>(s));
                return;
            }
            if (property.PropertyType == typeof(Quaternion))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<Quaternion>(s));
                return;
            }
            if (property.PropertyType == typeof(Color))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<Color>(s));
                return;
            }
            if (property.PropertyType == typeof(Color32))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<Color32>(s));
                return;
            }
            if (property.PropertyType == typeof(Rect))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<Rect>(s));
                return;
            }
            if (property.PropertyType == typeof(RectOffset))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<RectOffset>(s));
                return;
            }
            if (property.PropertyType == typeof(AnimationCurve))
            {
                property.SetValue(component, JsonConvert.DeserializeObject<AnimationCurve>(s));
                return;
            }
            throw new ArgumentException($"Field {fieldName} has unsupported type {property.PropertyType}!");
        }
    }
}