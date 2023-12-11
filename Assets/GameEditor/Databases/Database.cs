using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// using ETdoFresh.Localbase;
// using ChildChangedEventArgs = ETdoFresh.Localbase.ChildChangedEventArgs;
// using ValueChangedEventArgs = ETdoFresh.Localbase.ValueChangedEventArgs;

using Firebase.Database;
using ChildChangedEventArgs = Firebase.Database.ChildChangedEventArgs;
using ValueChangedEventArgs = Firebase.Database.ValueChangedEventArgs;

namespace GameEditor.Databases
{
    // Database Facade around LocalbaseDatabase
    // public static class Database
    // {
    //     public static class ValueChanged
    //     {
    //         public static void AddListener(string path, Action<ValueChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(listener);
    //
    //         public static void RemoveListener(string path, Action<ValueChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
    //     }
    //     
    //     public static class ChildAdded
    //     {
    //         public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildAdded.AddListener(listener);
    //
    //         public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildAdded.RemoveListener(listener);
    //     }
    //     
    //     public static class ChildRemoved
    //     {
    //         public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildRemoved.AddListener(listener);
    //
    //         public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildRemoved.RemoveListener(listener);
    //     }
    //     
    //     public static class ChildChanged
    //     {
    //         public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildChanged.AddListener(listener);
    //
    //         public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildChanged.RemoveListener(listener);
    //     }
    //     
    //     public static class ChildMoved
    //     {
    //         public static void AddListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildMoved.AddListener(listener);
    //
    //         public static void RemoveListener(string path, Action<ChildChangedEventArgs> listener) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ChildMoved.RemoveListener(listener);
    //     }
    //     
    //
    //     public static class Object
    //     {
    //         public static void AddChild(string path, string key, object value) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).AddObjectChild(key, value);
    //
    //         public static void RemoveChild(string path, string key) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).RemoveObjectChild(key);
    //     }
    //
    //     public static class Array
    //     {
    //         public static void AddChild(string path, object value) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).AddArrayChild(value);
    //
    //         public static void RemoveChild(string path, int index) => 
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).RemoveArrayChild(index);
    //     }
    //
    //     public static async Task SetValueAsync(string path, object value) => 
    //         await LocalbaseDatabase.DefaultInstance.GetReference(path).SetValueAsync(value);
    //
    //     public static async Task<object> GetValueAsync(string path)
    //     {
    //         var tcs = new TaskCompletionSource<object>();
    //         Action<ValueChangedEventArgs> listener = null;
    //         listener = args =>
    //         {
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
    //             tcs.SetResult(args.Snapshot.Value);
    //         };
    //         LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(listener);
    //         return await tcs.Task;
    //     }
    //     
    //     public static void GetValueCallback(string path, Action<object> callback)
    //     {
    //         Action<ValueChangedEventArgs> listener = null;
    //         listener = args =>
    //         {
    //             LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.RemoveListener(listener);
    //             callback?.Invoke(args.Snapshot.Value);
    //         };
    //         LocalbaseDatabase.DefaultInstance.GetReference(path).ValueChanged.AddListener(listener);
    //     }
    //     
    //     public static async Task<bool> IsNullCheckAsync(string path) => 
    //         await GetValueAsync(path) == null;
    // }
    
    // Database Facade around FirebaseDatabase
    public static class Database
    {
        public static class ValueChanged
        {
            public static void AddListener(string path, EventHandler<ValueChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged += listener;

            public static void RemoveListener(string path, EventHandler<ValueChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged -= listener;
        }
        
        public static class ChildAdded
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildAdded += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildAdded -= listener;
        }
        
        public static class ChildRemoved
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildRemoved += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildRemoved -= listener;
        }
        
        public static class ChildChanged
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildChanged += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildChanged -= listener;
        }
        
        public static class ChildMoved
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildMoved += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgs> listener) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).ChildMoved -= listener;
        }
        

        public static class Object
        {
            public static void AddChild(string path, string key, object value) =>
                FirebaseDatabase.DefaultInstance.GetReference(path).UpdateChildrenAsync(
                    new Dictionary<string, object> { {key, value} });

            public static void RemoveChild(string path, string key) => 
                FirebaseDatabase.DefaultInstance.GetReference(path).UpdateChildrenAsync(
                    new Dictionary<string, object> { {key, null} });
        }

        public static class Array
        {
            public static void AddChild(string path, object value) =>
                FirebaseDatabase.DefaultInstance.GetReference(path).Push().SetValueAsync(value);

            public static void RemoveChild(string path, int index) =>
                FirebaseDatabase.DefaultInstance.GetReference(path).Child(index.ToString()).RemoveValueAsync();
        }
        
        public static async Task SetValueAsync(string path, object value) => 
            await FirebaseDatabase.DefaultInstance.GetReference(path).SetValueAsync(value);

        public static async Task<object> GetValueAsync(string path)
        {
            var tcs = new TaskCompletionSource<object>();
            EventHandler<ValueChangedEventArgs> listener = null;
            listener = (sender, args) =>
            {
                FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged -= listener;
                tcs.SetResult(args.Snapshot.Value);
            };
            FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged += listener;
            return await tcs.Task;
        }
        
        public static void GetValueCallback(string path, Action<object, ValueChangedEventArgs> callback)
        {
            EventHandler<ValueChangedEventArgs> listener = null;
            listener = (sender, args) =>
            {
                FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged -= listener;
                callback?.Invoke(sender, args);
            };
            FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged += listener;
        }
        
        public static async Task<bool> IsNullCheckAsync(string path) => 
            await GetValueAsync(path) == null;
    }
}