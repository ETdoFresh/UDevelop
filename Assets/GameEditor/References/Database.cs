using System.Collections.Generic;
using System.Threading.Tasks;
using ETdoFresh.Localbase;
using UnityEngine.Events;

namespace GameEditor.References
{
    // Database Facade around LocalbaseDatabase
    public static class Database
    {
        private static Dictionary<System.Action<object>, UnityAction<ValueChangedEventArgs>> _valueChangedListeners = new();
        private static Dictionary<System.Action<object>, UnityAction<ChildChangedEventArgs>> _childAddedListeners = new();

        public static class ValueChanged
        {
            public static void AddListener(string path, System.Action<object> callback)
            {
                if (!_valueChangedListeners.ContainsKey(callback)) 
                    _valueChangedListeners.Add(callback, args => callback(args.Snapshot.Value.ToString()));

                LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(_valueChangedListeners[callback]);
            }
    
            public static void RemoveListener(string path, System.Action<object> callback)
            {
                if (_valueChangedListeners.TryGetValue(callback, out var listener))
                    LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
            }
        }
        
        public static class ChildAdded
        {
            public static void AddListener(string path, System.Action<object> callback)
            {
                if (!_childAddedListeners.ContainsKey(callback)) 
                    _childAddedListeners.Add(callback, args => callback(args.Snapshot.Value.ToString()));

                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildAdded.AddListener(_childAddedListeners[callback]);
            }
    
            public static void RemoveListener(string path, System.Action<object> callback)
            {
                if (_childAddedListeners.TryGetValue(callback, out var listener))
                    LocalbaseDatabase.DefaultInstance.GetReference(path).ChildAdded.RemoveListener(listener);
            }
        }
        
        public static class ChildRemoved
        {
            public static void AddListener(string path, System.Action<object> callback)
            {
                if (!_childAddedListeners.ContainsKey(callback)) 
                    _childAddedListeners.Add(callback, args => callback(args.Snapshot.Value.ToString()));

                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildRemoved.AddListener(_childAddedListeners[callback]);
            }
    
            public static void RemoveListener(string path, System.Action<object> callback)
            {
                if (_childAddedListeners.TryGetValue(callback, out var listener))
                    LocalbaseDatabase.DefaultInstance.GetReference(path).ChildRemoved.RemoveListener(listener);
            }
        }
        
        public static class ChildChanged
        {
            public static void AddListener(string path, System.Action<object> callback)
            {
                if (!_childAddedListeners.ContainsKey(callback)) 
                    _childAddedListeners.Add(callback, args => callback(args.Snapshot.Value.ToString()));

                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildChanged.AddListener(_childAddedListeners[callback]);
            }
    
            public static void RemoveListener(string path, System.Action<object> callback)
            {
                if (_childAddedListeners.TryGetValue(callback, out var listener))
                    LocalbaseDatabase.DefaultInstance.GetReference(path).ChildChanged.RemoveListener(listener);
            }
        }
        
        public static class ChildMoved
        {
            public static void AddListener(string path, System.Action<object> callback)
            {
                if (!_childAddedListeners.ContainsKey(callback)) 
                    _childAddedListeners.Add(callback, args => callback(args.Snapshot.Value.ToString()));

                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildMoved.AddListener(_childAddedListeners[callback]);
            }
    
            public static void RemoveListener(string path, System.Action<object> callback)
            {
                if (_childAddedListeners.TryGetValue(callback, out var listener))
                    LocalbaseDatabase.DefaultInstance.GetReference(path).ChildMoved.RemoveListener(listener);
            }
        }

        public static class Object
        {
            public static void AddChild(string path, string key, object value)
            {
                LocalbaseDatabase.DefaultInstance.GetReference(path).AddObjectChild(key, value);
            }

            public static void RemoveChild(string path, string key)
            {
                LocalbaseDatabase.DefaultInstance.GetReference(path).RemoveObjectChild(key);
            }
        }

        public static class Array
        {
            public static void AddChild(string path, object value)
            {
                LocalbaseDatabase.DefaultInstance.GetReference(path).AddArrayChild(value);
            }
            
            public static void RemoveChild(string path, int index)
            {
                LocalbaseDatabase.DefaultInstance.GetReference(path).RemoveArrayChild(index);
            }
        }
    
        public static Task SetValueAsync(string path, object value)
        {
            return LocalbaseDatabase.DefaultInstance.GetReference(path).SetValueAsync(value);
        }
    
        public static Task<object> GetValueAsync(string path)
        {
            var tcs = new TaskCompletionSource<object>();
            UnityAction<ValueChangedEventArgs> listener = null;
            listener = args =>
            {
                LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
                tcs.SetResult(args.Snapshot.Value);
            };
            LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(listener);
            return tcs.Task;
        }
    }
}