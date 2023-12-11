using System;

namespace ETdoFresh.Localbase
{
    public class Data<T>
    {
        private T _value;
        private EventHandler<T> _valueChanged;

        public T Value { get => _value; set => SetValue(value); }

        public Data(T initialValue)
        {
            _value = initialValue;
        }

        private void SetValue(T value)
        {
            _value = value;
            _valueChanged.Invoke(null, value);
        }

        public void AddListener(EventHandler<T> listener)
        {
            _valueChanged += listener; 
            if (typeof(IDoNotInvokeOnAddListener).IsAssignableFrom(typeof(T))) return;
            listener.Invoke(null, _value);
        }

        public void RemoveListener(EventHandler<T> listener) => 
            _valueChanged -= listener;
        
        public void RemoveAllListeners() =>
            _valueChanged = delegate { };
    }
}