using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameEditor.Databases
{
    public static class Database
    {
        public static class ValueChanged
        {
            public static void AddListener(string path, EventHandler<ValueChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ValueChanged += listener;

            public static void RemoveListener(string path, EventHandler<ValueChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ValueChanged -= listener;
        }

        public static class ChildAdded
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildAdded += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildAdded -= listener;
        }

        public static class ChildRemoved
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildRemoved += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildRemoved -= listener;
        }

        public static class ChildChanged
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildChanged += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildChanged -= listener;
        }

        public static class ChildMoved
        {
            public static void AddListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildMoved += listener;

            public static void RemoveListener(string path, EventHandler<ChildChangedEventArgsWrapper> listener) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).ChildMoved -= listener;
        }


        public static class Object
        {
            public static void AddChild(string path, string key, object value) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).UpdateChildrenAsync(
                    new Dictionary<string, object> { { key, value } });

            public static void RemoveChild(string path, string key) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).UpdateChildrenAsync(
                    new Dictionary<string, object> { { key, null } });
        }

        public static class Array
        {
            public static void AddChild(string path, object value) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).Push().SetValueAsync(value);

            public static void RemoveChild(string path, int index) =>
                DatabaseWrapper.DefaultInstance.GetReference(path).Child(index.ToString()).RemoveValueAsync();
        }

        public static async Task SetValueAsync(string path, object value) =>
            await DatabaseWrapper.DefaultInstance.GetReference(path).SetValueAsync(value);

        public static async Task<object> GetValueAsync(string path)
        {
            var tcs = new TaskCompletionSource<object>();
            EventHandler<ValueChangedEventArgsWrapper> listener = null;
            listener = (sender, args) =>
            {
                DatabaseWrapper.DefaultInstance.GetReference(path).ValueChanged -= listener;
                tcs.SetResult(args.Snapshot.Value);
            };
            DatabaseWrapper.DefaultInstance.GetReference(path).ValueChanged += listener;
            return await tcs.Task;
        }

        public static void GetValueCallback(string path, Action<object, ValueChangedEventArgsWrapper> callback)
        {
            EventHandler<ValueChangedEventArgsWrapper> listener = null;
            listener = (sender, args) =>
            {
                DatabaseWrapper.DefaultInstance.GetReference(path).ValueChanged -= listener;
                callback?.Invoke(sender, args);
            };
            DatabaseWrapper.DefaultInstance.GetReference(path).ValueChanged += listener;
        }

        public static async Task<bool> IsNullCheckAsync(string path) =>
            await GetValueAsync(path) == null;
    }
}