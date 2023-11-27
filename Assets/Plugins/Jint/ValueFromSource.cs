using System;
using Jint.Native;
using UnityEngine;
using UnityEngine.Events;

namespace Jint
{
    [Serializable]
    public class ValueFromSource
    {
        public enum ValueType { Bool, Int, Float, String, GuidReference, JsValue, Object }

        [SerializeField] protected string key;
        [SerializeField] protected string source = "instance";
        [SerializeField] protected ValueType valueType;
        [SerializeField] protected bool boolValueFromJs;
        [SerializeField] protected bool boolValueFromJson;
        [SerializeField] protected bool boolValueFromInstance;
        [SerializeField] protected int intValueFromJs;
        [SerializeField] protected int intValueFromJson;
        [SerializeField] protected int intValueFromInstance;
        [SerializeField] protected float floatValueFromJs;
        [SerializeField] protected float floatValueFromJson;
        [SerializeField] protected float floatValueFromInstance;
        [SerializeField] protected string stringValueFromJs;
        [SerializeField] protected string stringValueFromJson;
        [SerializeField] protected string stringValueFromInstance;
        [SerializeField] protected string guidReferenceFromJs;
        [SerializeField] protected string guidReferenceFromJson;
        [SerializeField] protected string guidReferenceFromInstance;
        [SerializeField] protected JsValue jsValueFromJs;
        [SerializeField] protected JsValue jsValueFromJson;
        [SerializeField] protected JsValue jsValueFromInstance;
        [SerializeField] protected object objectValueFromJs;
        [SerializeField] protected object objectValueFromJson;
        [SerializeField] protected object objectValueFromInstance;
    }
    
    [Serializable]
    public class ValueFromSource<T> : ValueFromSource
    {
        [SerializeField] protected UnityEvent<T, T> onValueChanged;

        public T Value { get => GetValue(); set => SetValue(value); }

        protected virtual T GetValue()
        {
            switch (source)
            {
                case "js":
                    return (T)objectValueFromJs ?? (T)objectValueFromJson ?? (T)objectValueFromInstance;
                case "json":
                    return (T)objectValueFromJson ?? (T)objectValueFromJs ?? (T)objectValueFromInstance;
                case "instance":
                    return (T)objectValueFromInstance ?? (T)objectValueFromJson ?? (T)objectValueFromJs;
                default:
                    return (T)objectValueFromInstance ?? (T)objectValueFromJson ?? (T)objectValueFromJs;
            }
        }

        protected virtual void SetValue(T value, string incomingSource = "instance")
        {
            switch (incomingSource)
            {
                case "js":
                    var previousValueFromJs = (T)objectValueFromJs;
                    objectValueFromJs = value;
                    onValueChanged?.Invoke(previousValueFromJs, value);
                    break;
                case "json":
                    var previousValueFromJson = (T)objectValueFromJson;
                    objectValueFromJson = value;
                    onValueChanged?.Invoke(previousValueFromJson, value);
                    break;
                case "instance":
                    var previousValueFromInstance = (T)objectValueFromInstance;
                    objectValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance, value);
                    break;
                default:
                    var previousValueFromInstance2 = (T)objectValueFromInstance;
                    objectValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance2, value);
                    break;
            }
        }

        public virtual T GetValueFromSource(string incomingSource)
        {
            switch (incomingSource)
            {
                case "js":
                    return (T)objectValueFromJs;
                case "json":
                    return (T)objectValueFromJson;
                case "instance":
                    return (T)objectValueFromInstance;
                default:
                    return (T)objectValueFromInstance;
            }
        }

        public void AddListener(UnityAction<T, T> action, bool invokeImmediately = true)
        {
            onValueChanged.AddListener(action);
            if (invokeImmediately) action.Invoke(Value, Value);
        }

        public void RemoveListener(UnityAction<T, T> action)
        {
            onValueChanged.RemoveListener(action);
        }
    }

    [Serializable]
    public class ValueFromSourceString : ValueFromSource<string>
    {
        protected override string GetValue()
        {
            switch (source)
            {
                case "js":
                    return stringValueFromJs ?? stringValueFromJson ?? stringValueFromInstance;
                case "json":
                    return stringValueFromJson ?? stringValueFromJs ?? stringValueFromInstance;
                case "instance":
                    return stringValueFromInstance ?? stringValueFromJson ?? stringValueFromJs;
                default:
                    return stringValueFromInstance ?? stringValueFromJson ?? stringValueFromJs;
            }
        }

        protected override void SetValue(string value, string incomingSource = "instance")
        {
            switch (incomingSource)
            {
                case "js":
                    var previousValueFromJs = stringValueFromJs;
                    stringValueFromJs = value;
                    onValueChanged?.Invoke(previousValueFromJs, value);
                    break;
                case "json":
                    var previousValueFromJson = stringValueFromJson;
                    stringValueFromJson = value;
                    onValueChanged?.Invoke(previousValueFromJson, value);
                    break;
                case "instance":
                    var previousValueFromInstance = stringValueFromInstance;
                    stringValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance, value);
                    break;
                default:
                    var previousValueFromInstance2 = stringValueFromInstance;
                    stringValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance2, value);
                    break;
            }
        }

        public override string GetValueFromSource(string incomingSource)
        {
            switch (incomingSource)
            {
                case "js":
                    return stringValueFromJs;
                case "json":
                    return stringValueFromJson;
                case "instance":
                    return stringValueFromInstance;
                default:
                    return stringValueFromInstance;
            }
        }
    }

    [Serializable]
    public class ValueFromSourceInt : ValueFromSource<int>
    {
        protected override int GetValue()
        {
            switch (source)
            {
                case "js":
                    return intValueFromJs != 0 ? intValueFromJs : intValueFromJson != 0 ? intValueFromJson : intValueFromInstance;
                case "json":
                    return intValueFromJson != 0 ? intValueFromJson : intValueFromJs != 0 ? intValueFromJs : intValueFromInstance;
                case "instance":
                    return intValueFromInstance != 0 ? intValueFromInstance : intValueFromJson != 0 ? intValueFromJson : intValueFromJs;
                default:
                    return intValueFromInstance != 0 ? intValueFromInstance : intValueFromJson != 0 ? intValueFromJson : intValueFromJs;
            }
        }

        protected override void SetValue(int value, string incomingSource = "instance")
        {
            switch (incomingSource)
            {
                case "js":
                    var previousValueFromJs = intValueFromJs;
                    intValueFromJs = value;
                    onValueChanged?.Invoke(previousValueFromJs, value);
                    break;
                case "json":
                    var previousValueFromJson = intValueFromJson;
                    intValueFromJson = value;
                    onValueChanged?.Invoke(previousValueFromJson, value);
                    break;
                case "instance":
                    var previousValueFromInstance = intValueFromInstance;
                    intValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance, value);
                    break;
                default:
                    var previousValueFromInstance2 = intValueFromInstance;
                    intValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance2, value);
                    break;
            }
        }

        public override int GetValueFromSource(string incomingSource)
        {
            switch (incomingSource)
            {
                case "js":
                    return intValueFromJs;
                case "json":
                    return intValueFromJson;
                case "instance":
                    return intValueFromInstance;
                default:
                    return intValueFromInstance;
            }
        }
    }

    [Serializable]
    public class ValueFromSourceFloat : ValueFromSource<float>
    {
        protected override float GetValue()
        {
            switch (source)
            {
                case "js":
                    return floatValueFromJs != 0 ? floatValueFromJs : floatValueFromJson != 0 ? floatValueFromJson : floatValueFromInstance;
                case "json":
                    return floatValueFromJson != 0 ? floatValueFromJson : floatValueFromJs != 0 ? floatValueFromJs : floatValueFromInstance;
                case "instance":
                    return floatValueFromInstance != 0 ? floatValueFromInstance : floatValueFromJson != 0 ? floatValueFromJson : floatValueFromJs;
                default:
                    return floatValueFromInstance != 0 ? floatValueFromInstance : floatValueFromJson != 0 ? floatValueFromJson : floatValueFromJs;
            }
        }

        protected override void SetValue(float value, string incomingSource = "instance")
        {
            switch (incomingSource)
            {
                case "js":
                    var previousValueFromJs = floatValueFromJs;
                    floatValueFromJs = value;
                    onValueChanged?.Invoke(previousValueFromJs, value);
                    break;
                case "json":
                    var previousValueFromJson = floatValueFromJson;
                    floatValueFromJson = value;
                    onValueChanged?.Invoke(previousValueFromJson, value);
                    break;
                case "instance":
                    var previousValueFromInstance = floatValueFromInstance;
                    floatValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance, value);
                    break;
                default:
                    var previousValueFromInstance2 = floatValueFromInstance;
                    floatValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance2, value);
                    break;
            }
        }

        public override float GetValueFromSource(string incomingSource)
        {
            switch (incomingSource)
            {
                case "js":
                    return floatValueFromJs;
                case "json":
                    return floatValueFromJson;
                case "instance":
                    return floatValueFromInstance;
                default:
                    return floatValueFromInstance;
            }
        }
    }

    [Serializable]
    public class ValueFromSourceBool : ValueFromSource<bool>
    {
        protected override bool GetValue()
        {
            switch (source)
            {
                case "js":
                    return boolValueFromJs || boolValueFromJson || boolValueFromInstance;
                case "json":
                    return boolValueFromJson || boolValueFromJs || boolValueFromInstance;
                case "instance":
                    return boolValueFromInstance || boolValueFromJson || boolValueFromJs;
                default:
                    return boolValueFromInstance || boolValueFromJson || boolValueFromJs;
            }
        }

        protected override void SetValue(bool value, string incomingSource = "instance")
        {
            switch (incomingSource)
            {
                case "js":
                    var previousValueFromJs = boolValueFromJs;
                    boolValueFromJs = value;
                    onValueChanged?.Invoke(previousValueFromJs, value);
                    break;
                case "json":
                    var previousValueFromJson = boolValueFromJson;
                    boolValueFromJson = value;
                    onValueChanged?.Invoke(previousValueFromJson, value);
                    break;
                case "instance":
                    var previousValueFromInstance = boolValueFromInstance;
                    boolValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance, value);
                    break;
                default:
                    var previousValueFromInstance2 = boolValueFromInstance;
                    boolValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance2, value);
                    break;
            }
        }

        public override bool GetValueFromSource(string incomingSource)
        {
            switch (incomingSource)
            {
                case "js":
                    return boolValueFromJs;
                case "json":
                    return boolValueFromJson;
                case "instance":
                    return boolValueFromInstance;
                default:
                    return boolValueFromInstance;
            }
        }
    }
    
    [Serializable]
    public class ValueFromSourceGuidReference : ValueFromSource<string>
    {
        protected override string GetValue()
        {
            switch (source)
            {
                case "js":
                    return guidReferenceFromJs ?? guidReferenceFromJson ?? guidReferenceFromInstance;
                case "json":
                    return guidReferenceFromJson ?? guidReferenceFromJs ?? guidReferenceFromInstance;
                case "instance":
                    return guidReferenceFromInstance ?? guidReferenceFromJson ?? guidReferenceFromJs;
                default:
                    return guidReferenceFromInstance ?? guidReferenceFromJson ?? guidReferenceFromJs;
            }
        }

        protected override void SetValue(string value, string incomingSource = "instance")
        {
            switch (incomingSource)
            {
                case "js":
                    var previousValueFromJs = guidReferenceFromJs;
                    guidReferenceFromJs = value;
                    onValueChanged?.Invoke(previousValueFromJs, value);
                    break;
                case "json":
                    var previousValueFromJson = guidReferenceFromJson;
                    guidReferenceFromJson = value;
                    onValueChanged?.Invoke(previousValueFromJson, value);
                    break;
                case "instance":
                    var previousValueFromInstance = guidReferenceFromInstance;
                    guidReferenceFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance, value);
                    break;
                default:
                    var previousValueFromInstance2 = guidReferenceFromInstance;
                    guidReferenceFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance2, value);
                    break;
            }
        }

        public override string GetValueFromSource(string incomingSource)
        {
            switch (incomingSource)
            {
                case "js":
                    return guidReferenceFromJs;
                case "json":
                    return guidReferenceFromJson;
                case "instance":
                    return guidReferenceFromInstance;
                default:
                    return guidReferenceFromInstance;
            }
        }
    }
    
    [Serializable]
    public class ValueFromSourceJsValue : ValueFromSource<JsValue>
    {
        protected override JsValue GetValue()
        {
            switch (source)
            {
                case "js":
                    return jsValueFromJs ?? jsValueFromJson ?? jsValueFromInstance;
                case "json":
                    return jsValueFromJson ?? jsValueFromJs ?? jsValueFromInstance;
                case "instance":
                    return jsValueFromInstance ?? jsValueFromJson ?? jsValueFromJs;
                default:
                    return jsValueFromInstance ?? jsValueFromJson ?? jsValueFromJs;
            }
        }

        protected override void SetValue(JsValue value, string incomingSource = "instance")
        {
            switch (incomingSource)
            {
                case "js":
                    var previousValueFromJs = jsValueFromJs;
                    jsValueFromJs = value;
                    onValueChanged?.Invoke(previousValueFromJs, value);
                    break;
                case "json":
                    var previousValueFromJson = jsValueFromJson;
                    jsValueFromJson = value;
                    onValueChanged?.Invoke(previousValueFromJson, value);
                    break;
                case "instance":
                    var previousValueFromInstance = jsValueFromInstance;
                    jsValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance, value);
                    break;
                default:
                    var previousValueFromInstance2 = jsValueFromInstance;
                    jsValueFromInstance = value;
                    onValueChanged?.Invoke(previousValueFromInstance2, value);
                    break;
            }
        }

        public override JsValue GetValueFromSource(string incomingSource)
        {
            switch (incomingSource)
            {
                case "js":
                    return jsValueFromJs;
                case "json":
                    return jsValueFromJson;
                case "instance":
                    return jsValueFromInstance;
                default:
                    return jsValueFromInstance;
            }
        }
    }
}