using System;

namespace ETdoFresh.Localbase
{
    public class Data<T>
    {
        private T _value;
        private readonly Action<T> _valueChanged = delegate { };

        public T Value { get => _value; set => SetValue(value); }
        public Action<T> ValueChanged => _valueChanged;

        public Data(T initialValue)
        {
            _value = initialValue;
        }

        private void SetValue(T value)
        {
            _value = value;
            _valueChanged.Invoke(value);
        }

        public void AddListener(Action<T> listener)
        {
            _valueChanged.AddListener(listener); 
            if (typeof(IDoNotInvokeOnAddListener).IsAssignableFrom(typeof(T))) return;
            listener.Invoke(_value);
        }

        public void RemoveListener(Action<T> listener) => 
            _valueChanged.RemoveListener(listener);
        
        public void RemoveAllListeners() =>
            _valueChanged.RemoveAllListeners();
    }
    
    public static class ActionExtensionMethods
    {
        public static void AddListener<T>(this Action<T> action, Action<T> listener) =>
            action += listener;
        
        public static void RemoveListener<T>(this Action<T> action, Action<T> listener) =>
            action -= listener;
        
        public static void RemoveAllListeners<T>(this Action<T> action) =>
            action = delegate { };
    }
}