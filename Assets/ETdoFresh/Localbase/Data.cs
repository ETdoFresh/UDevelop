using UnityEngine.Events;

namespace ETdoFresh.Localbase
{
    public class Data<T>
    {
        private T _value;
        private readonly UnityEvent<T> _valueChanged = new();

        public T Value { get => _value; set => SetValue(value); }
        public UnityEvent<T> ValueChanged => _valueChanged;

        public Data(T initialValue)
        {
            _value = initialValue;
        }

        private void SetValue(T value)
        {
            _value = value;
            _valueChanged.Invoke(value);
        }

        public void AddListener(UnityAction<T> listener)
        {
            _valueChanged.AddListener(listener); 
            if (typeof(IDoNotInvokeOnAddListenerWhenNull).IsAssignableFrom(typeof(T)) && _value == null) return;
            listener.Invoke(_value);
        }

        public void RemoveListener(UnityAction<T> listener) => _valueChanged.RemoveListener(listener);
        public void RemoveAllListeners() => _valueChanged.RemoveAllListeners();
    }
}