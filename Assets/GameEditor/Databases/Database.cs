using System;
using Cysharp.Threading.Tasks;

namespace GameEditor.Databases
{
    public static class Database
    {
        //private static IDatabase _database = new LocalbaseAdapter();
        private static IDatabase _database = new FirebaseAdapter();
        
        public static void OpenDatabaseFromEditor() => _database.OpenDatabaseFromEditor();
        
        public static void AddValueChangedListener(string path, EventHandler<IValueChangedEventArgs> listener) => 
            _database.AddValueChangedListener(path, listener);
        
        public static void RemoveValueChangedListener(string path, EventHandler<IValueChangedEventArgs> listener) =>
            _database.RemoveValueChangedListener(path, listener);
        
        public static void AddChildAddedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.AddChildAddedListener(path, listener);
        
        public static void RemoveChildAddedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.RemoveChildAddedListener(path, listener);
        
        public static void AddChildRemovedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.AddChildRemovedListener(path, listener);
        
        public static void RemoveChildRemovedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.RemoveChildRemovedListener(path, listener);
        
        public static void AddChildChangedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.AddChildChangedListener(path, listener);
        
        public static void RemoveChildChangedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.RemoveChildChangedListener(path, listener);
        
        public static void AddChildMovedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.AddChildMovedListener(path, listener);
        
        public static void RemoveChildMovedListener(string path, EventHandler<IChildChangedEventArgs> listener) =>
            _database.RemoveChildMovedListener(path, listener);
        
        public static void AddObjectChild(string path, string key, object value) =>
            _database.AddObjectChild(path, key, value);
        
        public static void RemoveObjectChild(string path, string key) =>
            _database.RemoveObjectChild(path, key);
        
        public static void AddArrayChild(string path, object value) =>
            _database.AddArrayChild(path, value);
        
        public static void RemoveArrayChild(string path, int index) =>
            _database.RemoveArrayChild(path, index);
        
        public static async UniTask SetValueAsync(string path, object value) =>
            await _database.SetValueAsync(path, value);
        
        public static async UniTask<object> GetValueAsync(string path) =>
            await _database.GetValueAsync(path);
    }
}