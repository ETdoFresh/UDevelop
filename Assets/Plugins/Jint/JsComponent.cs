using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Native;
using Newtonsoft.Json;
using UnityEngine;
using Types = Jint.Runtime.Types;

namespace Jint
{
    public class JsComponent : MonoBehaviour
    {
        [SerializeField] private JsScriptableObject js;
        [SerializeField] private string overrideInitialValues;
        private JsValue _componentJs;
        
        public string ScriptName => js ? js.name : null;

        private void Awake()
        {
            if (js) js.ScriptChanged.AddListener(HotReload);
            HotReload();
            _componentJs?.AsObject()?.GetProperty("awake")?.Value?.Invoke();
        }

        private void Start()
        {
            _componentJs?.AsObject()?.GetProperty("start")?.Value?.Invoke();
        }

        private void OnDestroy()
        {
            _componentJs?.AsObject()?.GetProperty("onDestroy")?.Value?.Invoke();
            if (js) js.ScriptChanged.RemoveListener(HotReload);
        }

        private void OnEnable()
        {
            _componentJs?.AsObject()?.GetProperty("onEnable")?.Value?.Invoke();
        }

        private void OnDisable()
        {
            _componentJs?.AsObject()?.GetProperty("onDisable")?.Value?.Invoke();
        }

        private void FixedUpdate()
        {
            _componentJs?.AsObject()?.GetProperty("fixedUpdate")?.Value?.Invoke();
        }

        private void LateUpdate()
        {
            _componentJs?.AsObject()?.GetProperty("lateUpdate")?.Value?.Invoke();
        }

        private void Update()
        {
            _componentJs?.AsObject()?.GetProperty("update")?.Value?.Invoke();
        }

        private void OnCollisionEnter(Collision other)
        {
            var collisionJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onCollisionEnter")?.Value?.Invoke(collisionJs);
        }

        private void OnCollisionExit(Collision other)
        {
            var collisionJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onCollisionExit")?.Value?.Invoke(collisionJs);
        }

        private void OnCollisionStay(Collision other)
        {
            var collisionJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onCollisionStay")?.Value?.Invoke(collisionJs);
        }

        private void OnTriggerEnter(Collider other)
        {
            var colliderJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onTriggerEnter")?.Value?.Invoke(colliderJs);
        }

        private void OnTriggerExit(Collider other)
        {
            var colliderJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onTriggerExit")?.Value?.Invoke(colliderJs);
        }

        private void OnTriggerStay(Collider other)
        {
            var colliderJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onTriggerStay")?.Value?.Invoke(colliderJs);
        }

        private void OnParticleCollision(GameObject other)
        {
            var gameObjectJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onParticleCollision")?.Value?.Invoke(gameObjectJs);
        }

        [ContextMenu("Hot Reload")]
        private void HotReload()
        {
            var selfJs = JavaScript.GetJsValue(this);
            JavaScript.Engine.SetValue("self", selfJs);
            _componentJs = JavaScript.Evaluate(js.JavaScript);
            JavaScript.Evaluate($"delete self;");
            RemoveValuesNotInJs();
            OverrideInitialValues();
        }

        private void RemoveValuesNotInJs()
        {
            if (overrideInitialValues == null) return;
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(overrideInitialValues);
            if (values == null) return;
            var jsKeys = _componentJs.AsObject().GetOwnProperties().Select(x => x.Key).ToArray();
            var valuesKeys = values.Keys.ToArray();
            foreach (var valuesKey in valuesKeys)
                if (!jsKeys.Contains(valuesKey))
                    values.Remove(valuesKey);
            overrideInitialValues = JsonConvert.SerializeObject(values);
        }

        private void OverrideInitialValues()
        {
            if (overrideInitialValues == null) return;
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(overrideInitialValues);
            if (values == null) return;
            foreach (var (key, value) in values)
            {
                if (value is bool boolValue)
                    _componentJs.AsObject().GetProperty(key).Value = boolValue;
                else if (value is int intValue)
                    _componentJs.AsObject().GetProperty(key).Value = intValue;
                else if (value is float floatValue)
                    _componentJs.AsObject().GetProperty(key).Value = floatValue;
                else if (value is double doubleValue)
                    _componentJs.AsObject().GetProperty(key).Value = doubleValue;
                else if (value is string stringValue)
                    _componentJs.AsObject().GetProperty(key).Value = stringValue;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(JsComponent))]
        public class JsComponentEditor : UnityEditor.Editor
        {
            private Dictionary<string, object> values = new();
            private Dictionary<string, object> jsValues = new();
            private string previousJavaScript;

            public override void OnInspectorGUI()
            {
                var target = (JsComponent)this.target;
                var serializedObject = new UnityEditor.SerializedObject(target);
                var script = serializedObject.FindProperty("m_Script");
                var js = serializedObject.FindProperty("js");
                var overrideInitialValues = serializedObject.FindProperty("overrideInitialValues");
                UnityEditor.EditorGUI.BeginDisabledGroup(true);
                UnityEditor.EditorGUILayout.PropertyField(script, true, Array.Empty<GUILayoutOption>());
                UnityEditor.EditorGUI.EndDisabledGroup();
                UnityEditor.EditorGUILayout.PropertyField(js, true, Array.Empty<GUILayoutOption>());
                serializedObject.ApplyModifiedProperties();
                
                //UnityEditor.EditorGUILayout.TextArea(overrideInitialValues.stringValue, UnityEditor.EditorStyles.textArea);

                if (overrideInitialValues.stringValue != null && values.Count == 0)
                    values = JsonConvert.DeserializeObject<Dictionary<string, object>>(overrideInitialValues.stringValue) ?? new Dictionary<string, object>();
                
                if (js != null && js.objectReferenceValue != null)
                {
                    var jsScriptableObject = (JsScriptableObject)js.objectReferenceValue;
                    var jsText = jsScriptableObject.JavaScript;
                    var jsObject = target._componentJs;
                    var jsKeys = jsObject == null
                        ? Array.Empty<string>()
                        : jsObject.AsObject().GetOwnProperties().Select(x => x.Key).ToArray();
                    if (previousJavaScript != jsText)
                    {
                        previousJavaScript = jsText;
                        target.HotReload();
                        jsObject = target._componentJs;
                        jsKeys = jsObject == null
                            ? Array.Empty<string>()
                            : jsObject.AsObject().GetOwnProperties().Select(x => x.Key).ToArray();
                        var ignoreKeys = new[]
                        {
                            "awake", "start", "onDestroy", "onEnable", "onDisable", "fixedUpdate", "lateUpdate",
                            "update", "onCollisionEnter", "onCollisionExit", "onCollisionStay", "onTriggerEnter",
                            "onTriggerExit", "onTriggerStay", "onParticleCollision", "self"
                        };
                        jsValues.Clear();
                        foreach (var jsKey in jsKeys)
                            if (!ignoreKeys.Contains(jsKey))
                                switch (jsObject.AsObject().GetProperty(jsKey).Value.Type)
                                {
                                    case Types.Boolean:
                                        jsValues[jsKey] = jsObject.AsObject().GetProperty(jsKey).Value.AsBoolean();
                                        break;
                                    case Types.Number:
                                        jsValues[jsKey] = jsObject.AsObject().GetProperty(jsKey).Value.AsNumber();
                                        break;
                                    case Types.String:
                                        jsValues[jsKey] = jsObject.AsObject().GetProperty(jsKey).Value.AsString();
                                        break;
                                    case Types.Object:
                                        // TODO: Too tired to figure out serializing this!
                                        // jsValues[jsKey] = jsObject.AsObject().GetProperty(jsKey).Value.AsObject();
                                        break;
                                    case Types.Undefined:
                                        jsValues[jsKey] = null;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                        foreach (var jsKey in jsKeys)
                            if (!values.ContainsKey(jsKey) && !ignoreKeys.Contains(jsKey) &&
                                jsValues.TryGetValue(jsKey, out var value))
                                values[jsKey] = value;

                        var valuesKeys = values.Keys.ToArray();
                        foreach (var valuesKey in valuesKeys)
                            if (!jsKeys.Contains(valuesKey))
                                values.Remove(valuesKey);
                    }

                    var keys = values.Keys.ToArray();
                    foreach (var key in keys)
                    {
                        var value = values[key];
                        UnityEditor.EditorGUILayout.BeginHorizontal();
                        var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
                        var guiLayoutOptions = new[] { GUILayout.MaxWidth(labelWidth) };
                        var changeSuffix = "*";
                        if (value is bool boolValue)
                        {
                            var isNotInitialValue = jsValues.ContainsKey(key) && (bool)jsValues[key] != boolValue;
                            var style = isNotInitialValue ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label;
                            var label = key + (isNotInitialValue ? changeSuffix : "");
                            label = UnityEditor.ObjectNames.NicifyVariableName(label);
                            UnityEditor.EditorGUILayout.LabelField(label, style, guiLayoutOptions);
                            values[key] = UnityEditor.EditorGUILayout.Toggle(boolValue);
                        }
                        else if (value is int intValue)
                        {
                            var isNotInitialValue = jsValues.ContainsKey(key) && (int)jsValues[key] != intValue;
                            var style = isNotInitialValue ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label;
                            var label = key + (isNotInitialValue ? changeSuffix : "");
                            label = UnityEditor.ObjectNames.NicifyVariableName(label);
                            UnityEditor.EditorGUILayout.LabelField(label, style, guiLayoutOptions);
                            values[key] = UnityEditor.EditorGUILayout.IntField(intValue);
                        }
                        else if (value is float floatValue)
                        {
                            var isNotInitialValue = jsValues.ContainsKey(key) && (float)jsValues[key] != floatValue;
                            var style = isNotInitialValue ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label;
                            var label = key + (isNotInitialValue ? changeSuffix : "");
                            label = UnityEditor.ObjectNames.NicifyVariableName(label);
                            UnityEditor.EditorGUILayout.LabelField(label, style, guiLayoutOptions);
                            values[key] = UnityEditor.EditorGUILayout.FloatField(floatValue);
                        }
                        else if (value is double doubleValue)
                        {
                            var isNotInitialValue = jsValues.ContainsKey(key) && (double)jsValues[key] != doubleValue;
                            var style = isNotInitialValue ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label;
                            var label = key + (isNotInitialValue ? changeSuffix : "");
                            label = UnityEditor.ObjectNames.NicifyVariableName(label);
                            UnityEditor.EditorGUILayout.LabelField(label, style, guiLayoutOptions);
                            values[key] = UnityEditor.EditorGUILayout.DoubleField(doubleValue);
                        }
                        else if (value is string stringValue)
                        {
                            var isNotInitialValue = jsValues.ContainsKey(key) && (string)jsValues[key] != stringValue;
                            var style = isNotInitialValue ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label;
                            var label = key + (isNotInitialValue ? changeSuffix : "");
                            label = UnityEditor.ObjectNames.NicifyVariableName(label);
                            UnityEditor.EditorGUILayout.LabelField(label, style, guiLayoutOptions);
                            values[key] = UnityEditor.EditorGUILayout.TextField(stringValue);
                        }
                        UnityEditor.EditorGUILayout.EndHorizontal();
                    }
                    var newValue = JsonConvert.SerializeObject(values);
                    if (newValue != overrideInitialValues.stringValue)
                    {
                        overrideInitialValues.stringValue = newValue;
                        serializedObject.ApplyModifiedProperties();
                        target.HotReload();
                    }
                }
                
                if (js != null && js.objectReferenceValue != null)
                {
                    var jsScriptableObject = (JsScriptableObject)js.objectReferenceValue;
                    var jsText = jsScriptableObject.JavaScript;
                    UnityEditor.EditorGUILayout.LabelField("JavaScript", UnityEditor.EditorStyles.boldLabel);
                    UnityEditor.EditorGUI.BeginDisabledGroup(true);
                    UnityEditor.EditorGUILayout.TextArea(jsText, UnityEditor.EditorStyles.textArea);
                    UnityEditor.EditorGUI.EndDisabledGroup();
                }
                
                UnityEditor.EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Hot Reload"))
                    target.HotReload();
                UnityEditor.EditorGUI.BeginDisabledGroup(js == null || js.objectReferenceValue == null);
                if (GUILayout.Button("Open Script"))
                    UnityEditor.AssetDatabase.OpenAsset(js.objectReferenceValue);
                UnityEditor.EditorGUI.EndDisabledGroup();
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
        }
#endif
    }
}