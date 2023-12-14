using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ETdoFresh.Localbase;

namespace GameEditor.Databases
{
    public class LocalbaseAdapter : IDatabase
    {
        private readonly LocalbaseDatabase _database = LocalbaseDatabase.DefaultInstance;
        private readonly Dictionary<EventHandler<IValueChangedEventArgs>, EventHandler<ValueChangedEventArgs>>
            _valueChangedListeners = new();
        private readonly Dictionary<EventHandler<IChildChangedEventArgs>, EventHandler<ChildChangedEventArgs>>
            _childChangedListeners = new();

        public void OpenDatabaseFromEditor()
        {
#if UNITY_EDITOR
            UnityEngine.Application.OpenURL(LocalbaseDatabase.DefaultInstance.Path);
#endif
        }

        public void AddValueChangedListener(string path, EventHandler<IValueChangedEventArgs> listener)
        {
            if (!_valueChangedListeners.ContainsKey(listener))
            {
                var valueChangedListener = new EventHandler<ValueChangedEventArgs>((sender, args) =>
                    listener.Invoke(sender, new LocalbaseValueChangedEventArgs(args)));
                _valueChangedListeners.Add(listener, valueChangedListener);
            }

            _database.GetReference(PathToKey(path)).ValueChanged += _valueChangedListeners[listener];
        }

        public void RemoveValueChangedListener(string path, EventHandler<IValueChangedEventArgs> listener)
        {
            if (_valueChangedListeners.TryGetValue(listener, out var valueChangedListener))
                _database.GetReference(PathToKey(path)).ValueChanged -= valueChangedListener;
        }

        public void AddChildAddedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (!_childChangedListeners.ContainsKey(listener))
            {
                var childChangedListener = new EventHandler<ChildChangedEventArgs>((sender, args) =>
                    listener(sender, new LocalbaseChildChangedEventArgs(args)));
                _childChangedListeners.Add(listener, childChangedListener);
            }

            _database.GetReference(PathToKey(path)).ChildAdded += _childChangedListeners[listener];
        }

        public void RemoveChildAddedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (_childChangedListeners.TryGetValue(listener, out var childAddedListener))
                _database.GetReference(PathToKey(path)).ChildAdded -= childAddedListener;
        }

        public void AddChildRemovedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (!_childChangedListeners.ContainsKey(listener))
            {
                var childChangedListener = new EventHandler<ChildChangedEventArgs>((sender, args) =>
                    listener(sender, new LocalbaseChildChangedEventArgs(args)));
                _childChangedListeners.Add(listener, childChangedListener);
            }

            _database.GetReference(PathToKey(path)).ChildRemoved += _childChangedListeners[listener];
        }

        public void RemoveChildRemovedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (_childChangedListeners.TryGetValue(listener, out var childRemovedListener))
                _database.GetReference(PathToKey(path)).ChildRemoved -= childRemovedListener;
        }

        public void AddChildChangedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (!_childChangedListeners.ContainsKey(listener))
            {
                var childChangedListener = new EventHandler<ChildChangedEventArgs>((sender, args) =>
                    listener(sender, new LocalbaseChildChangedEventArgs(args)));
                _childChangedListeners.Add(listener, childChangedListener);
            }

            _database.GetReference(PathToKey(path)).ChildChanged += _childChangedListeners[listener];
        }

        public void RemoveChildChangedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (_childChangedListeners.TryGetValue(listener, out var childChangedListener))
                _database.GetReference(PathToKey(path)).ChildChanged -= childChangedListener;
        }

        public void AddChildMovedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (!_childChangedListeners.ContainsKey(listener))
            {
                var childChangedListener = new EventHandler<ChildChangedEventArgs>((sender, args) =>
                    listener(sender, new LocalbaseChildChangedEventArgs(args)));
                _childChangedListeners.Add(listener, childChangedListener);
            }

            _database.GetReference(PathToKey(path)).ChildMoved += _childChangedListeners[listener];
        }

        public void RemoveChildMovedListener(string path, EventHandler<IChildChangedEventArgs> listener)
        {
            if (_childChangedListeners.TryGetValue(listener, out var childMovedListener))
                _database.GetReference(PathToKey(path)).ChildMoved -= childMovedListener;
        }

        public async void AddObjectChild(string path, string key, object value) => 
            await _database.GetReference(PathToKey(path)).Child(key).SetValueAsync(value);

        public async void RemoveObjectChild(string path, string key) =>
            await _database.GetReference(PathToKey(path)).Child(key).RemoveValueAsync();

        public async void AddArrayChild(string path, object value) =>
            await _database.GetReference(PathToKey(path)).Push().SetValueAsync(value);

        public async void RemoveArrayChild(string path, int index) =>
            await _database.GetReference(PathToKey(path)).Child(index.ToString()).RemoveValueAsync();

        public async UniTask SetValueAsync(string path, object value) =>
            await _database.GetReference(PathToKey(path)).SetValueAsync(value);

        public async UniTask<object> GetValueAsync(string path) =>
            (await _database.GetReference(PathToKey(path)).GetValueAsync()).Value;
        
        private static string PathToKey(string path) => path.Replace('/', '.');
    }

    public class LocalbaseValueChangedEventArgs : IValueChangedEventArgs
    {
        private readonly ValueChangedEventArgs _args;
        public object Snapshot => _args.Snapshot;
        public object SnapshotValue => _args.Snapshot.Value;
        public string SnapshotGetRawJsonValue() => _args.Snapshot.GetRawJsonValue();
        public LocalbaseValueChangedEventArgs(ValueChangedEventArgs args) => _args = args;
    }

    public class LocalbaseChildChangedEventArgs : IChildChangedEventArgs
    {
        private readonly ChildChangedEventArgs _args;
        public string Key => _args.Snapshot.Key;
        public object Snapshot => _args.Snapshot;
        public object SnapshotValue => _args.Snapshot.Value;
        public string PreviousChildName => _args.PreviousChildName;
        public LocalbaseChildChangedEventArgs(ChildChangedEventArgs args) => _args = args;
    }
}