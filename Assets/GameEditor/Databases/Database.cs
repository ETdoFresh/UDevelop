using System;
using System.Threading.Tasks;
using ETdoFresh.Localbase;

namespace GameEditor.Databases
{
    // Database Facade around LocalbaseDatabase
    public static class Database
    {
        public static class ValueChanged
        {
            public static void AddListener(string path, Action<ValueChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(listener);

            public static void RemoveListener(string path, Action<ValueChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
        }
        
        public static class ChildAdded
        {
            public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildAdded.AddListener(listener);

            public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildAdded.RemoveListener(listener);
        }
        
        public static class ChildRemoved
        {
            public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildRemoved.AddListener(listener);

            public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildRemoved.RemoveListener(listener);
        }
        
        public static class ChildChanged
        {
            public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildChanged.AddListener(listener);

            public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildChanged.RemoveListener(listener);
        }
        
        public static class ChildMoved
        {
            public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildMoved.AddListener(listener);

            public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).ChildMoved.RemoveListener(listener);
        }
        

        public static class Object
        {
            public static void AddChild(string path, string key, object value) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).AddObjectChild(key, value);

            public static void RemoveChild(string path, string key) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).RemoveObjectChild(key);
        }

        public static class Array
        {
            public static void AddChild(string path, object value) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).AddArrayChild(value);

            public static void RemoveChild(string path, int index) => 
                LocalbaseDatabase.DefaultInstance.GetReference(path).RemoveArrayChild(index);
        }
    
        public static Task SetValueAsync(string path, object value) => 
            LocalbaseDatabase.DefaultInstance.GetReference(path).SetValueAsync(value);

        public static Task<object> GetValueAsync(string path)
        {
            var tcs = new TaskCompletionSource<object>();
            Action<ValueChangedEventArgs> listener = null;
            listener = args =>
            {
                LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
                tcs.SetResult(args.Snapshot.Value);
            };
            LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(listener);
            return tcs.Task;
        }

        public static async Task<bool> IsNullCheckAsync(string path) => 
            await GetValueAsync(path) == null;
    }
}