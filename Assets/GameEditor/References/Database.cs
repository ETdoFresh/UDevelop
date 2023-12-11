using System.Collections.Generic;
using System.Threading.Tasks;
using ETdoFresh.Localbase;
using UnityEngine.Events;

namespace GameEditor.References
{
    // Database Facade around LocalbaseDatabase
    public static class Database
    {
        private static Dictionary<System.Action<string>, UnityAction<ValueChangedEventArgs>> _listeners = new();
    
        public static void AddListener(string path, System.Action<string> callback)
        {
            if (!_listeners.ContainsKey(callback)) 
                _listeners.Add(callback, args => callback(args.Snapshot.Value.ToString()));

            LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(_listeners[callback]);
        }
    
        public static void RemoveListener(string path, System.Action<string> callback)
        {
            if (_listeners.TryGetValue(callback, out var listener))
                LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
        }
    
        public static Task SetValue(string path, string value)
        {
            return LocalbaseDatabase.DefaultInstance.GetReference(path).SetValueAsync(value);
        }
    
        public static Task<object> GetValue(string path)
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