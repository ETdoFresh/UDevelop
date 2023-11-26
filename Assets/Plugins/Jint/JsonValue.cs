using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Jint
{
    [Serializable]
    public class JsonValue
    {
        public string jsonString;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(JsonValue))]
    public class JsonValuePropertyDrawer : UnityEditor.PropertyDrawer
    {
        private string _prevJsonString;
        private JToken _jsonToken;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var jsonStringProperty = property.FindPropertyRelative("jsonString");
            
            if (jsonStringProperty.stringValue != _prevJsonString)
            {
                try
                {
                    _jsonToken = JsonConvert.DeserializeObject<JToken>(jsonStringProperty.stringValue) ??
                                 JValue.CreateUndefined();
                    _prevJsonString = jsonStringProperty.stringValue;
                }
                catch (Exception e)
                {
                    // Create undefined JValue
                    _jsonToken = JValue.CreateUndefined();
                }
            }

            if (_jsonToken != null)
            {
                position = ShowJsonToken(_jsonToken, position);
                if (_jsonToken.Type != JTokenType.Undefined)
                    jsonStringProperty.stringValue = JsonConvert.SerializeObject(_jsonToken, Formatting.Indented);
            }
            
            var content = new GUIContent(jsonStringProperty.stringValue);
            var textAreaHeight = EditorStyles.textArea.CalcHeight(content, 9999);
            var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            position.y += EditorGUIUtility.singleLineHeight;
            var textAreaRect = new Rect(position.x, position.y, position.width, textAreaHeight);
            
            EditorGUI.LabelField(labelRect, "JSON", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            jsonStringProperty.stringValue = EditorGUI.TextArea(textAreaRect, jsonStringProperty.stringValue);
            EditorGUI.EndDisabledGroup();
            position.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var jsonStringProperty = property.FindPropertyRelative("jsonString");
            var jsonString = jsonStringProperty.stringValue;
            var content = new GUIContent(jsonString);
            var textAreaHeight = EditorStyles.textArea.CalcHeight(content, 9999);

            if (_jsonToken == null) return textAreaHeight;

            return textAreaHeight + GetJsonTokenHeight(_jsonToken);
        }

        private Rect ShowJsonToken(JToken jsonToken, Rect position)
        {
            var valueRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            position.y += EditorGUIUtility.singleLineHeight;
            if (jsonToken is JObject jObject)
            {
                EditorGUI.LabelField(valueRect, $"JObject ({jObject.Count})");
                for (var i = 0; i < jObject.Count; i++)
                {
                    var property = jObject.Properties().ElementAt(i);
                    var key = property.Name;
                    var value = property.Value;
                    var keyWidth = Mathf.Min(position.width * 0.33f, 100);
                    var valueWidth = position.width - keyWidth;
                    var keyRect = new Rect(position.x, position.y, keyWidth, EditorGUIUtility.singleLineHeight);
                    valueRect = new Rect(position.x + keyWidth, position.y, valueWidth, EditorGUIUtility.singleLineHeight);
                    var newKey = EditorGUI.TextField(keyRect, key);
                    if (newKey != key) property.Replace(new JProperty(newKey, value));
                    ShowJsonToken(value, valueRect);
                    position.y += GetJsonTokenHeight(value);
                }
            }
            else if (jsonToken is JArray jArray)
            {
                EditorGUI.LabelField(valueRect, $"JArray ({jArray.Count})");
                for (var i = 0; i < jArray.Count; i++)
                {
                    var value = jArray[i];
                    valueRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    ShowJsonToken(value, valueRect);
                    position.y += GetJsonTokenHeight(value);

                    if (value is not JValue jValue) continue;
                    if (jValue.Type != JTokenType.Float) continue;
                    if ((float)jValue.Value == Mathf.FloorToInt((float)jValue.Value))
                        jArray[i] = new JValue(Mathf.FloorToInt((float)jValue.Value));
                }
            }
            else if (jsonToken is JValue jValue)
            {
                if (jValue.Type == JTokenType.Boolean)
                {
                    jValue.Value = EditorGUI.Toggle(valueRect, jValue.Value<bool>());
                }
                else if (jValue.Type == JTokenType.Integer)
                {
                    jValue.Value = EditorGUI.FloatField(valueRect, jValue.Value<int>());
                }
                else if (jValue.Type == JTokenType.Float)
                {
                    jValue.Value = EditorGUI.FloatField(valueRect, jValue.Value<float>());
                }
                else if (jValue.Type == JTokenType.String)
                {
                    jValue.Value = EditorGUI.TextField(valueRect, jValue.Value<string>());
                }
                else if (jValue.Type == JTokenType.Null)
                {
                    EditorGUI.LabelField(valueRect, "null");
                }
                else
                {
                    EditorGUI.LabelField(valueRect, "Undefined JValue type");
                }
            }
            else
            {
                EditorGUI.LabelField(position, "Undefined JToken type");
            }
            return position;
        }

        private float GetJsonTokenHeight(JToken token)
        {
            var height = 0f;
            if (token is JObject jObject)
            {
                height += EditorGUIUtility.singleLineHeight;
                foreach (var property in jObject.Properties())
                    height += GetJsonTokenHeight(property.Value);
            }
            else if (token is JArray jArray)
            {
                height += EditorGUIUtility.singleLineHeight;
                foreach (var value in jArray.Values<JToken>())
                    height += GetJsonTokenHeight(value);
            }
            else if (token is JValue jValue)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            return height;
        }
    }
#endif
}