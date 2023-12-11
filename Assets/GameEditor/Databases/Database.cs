using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameEditor.Databases
{
    // Database Facade around FirebaseDatabase
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

    public class DataSnapshotWrapper
    {
        private ETdoFresh.Localbase.DataSnapshot _localbaseSnapshot;
        private Firebase.Database.DataSnapshot _firebaseSnapshot;

        public DataSnapshotWrapper(ETdoFresh.Localbase.DataSnapshot snapshot) => _localbaseSnapshot = snapshot;
        public DataSnapshotWrapper(Firebase.Database.DataSnapshot snapshot) => _firebaseSnapshot = snapshot;

        // Implicit conversion from Localbase.DataSnapshot to SnapshotWrapper
        public static implicit operator DataSnapshotWrapper(ETdoFresh.Localbase.DataSnapshot snapshot) =>
            new DataSnapshotWrapper(snapshot);

        // Implicit conversion from Firebase.Database.DataSnapshot to SnapshotWrapper
        public static implicit operator DataSnapshotWrapper(Firebase.Database.DataSnapshot snapshot) =>
            new DataSnapshotWrapper(snapshot);

        public bool HasChildren => _localbaseSnapshot?.HasChildren ?? _firebaseSnapshot.HasChildren;

        public bool Exists => _localbaseSnapshot?.Exists ?? _firebaseSnapshot.Exists;

        public object Value => _localbaseSnapshot?.Value ?? _firebaseSnapshot.Value;

        public long ChildrenCount => _localbaseSnapshot?.ChildrenCount ?? _firebaseSnapshot.ChildrenCount;

        public DatabaseReferenceWrapper Reference =>
            (DatabaseReferenceWrapper)_localbaseSnapshot?.Reference ?? _firebaseSnapshot.Reference;

        public string Key => _localbaseSnapshot?.Key ?? _firebaseSnapshot.Key;

        public IEnumerable<DataSnapshotWrapper> Children =>
            _localbaseSnapshot != null
                ? _localbaseSnapshot.Children.Cast<DataSnapshotWrapper>()
                : _firebaseSnapshot.Children.Cast<DataSnapshotWrapper>();

        // public object Priority => _localbaseSnapshot?.Priority ?? _firebaseSnapshot.Priority;

        public DataSnapshotWrapper Child(string path) =>
            (DataSnapshotWrapper)_localbaseSnapshot?.Child(path) ?? _firebaseSnapshot.Child(path);

        public bool HasChild(string path) => _localbaseSnapshot?.HasChild(path) ?? _firebaseSnapshot.HasChild(path);

        public string GetRawJsonValue() => _localbaseSnapshot?.GetRawJsonValue() ?? _firebaseSnapshot.GetRawJsonValue();

        public object GetValue(bool useExportFormat) => _localbaseSnapshot?.GetValue(useExportFormat) ??
                                                        _firebaseSnapshot.GetValue(useExportFormat);

        public override string ToString() => _localbaseSnapshot?.ToString() ?? _firebaseSnapshot.ToString();
    }

    public class DatabaseReferenceWrapper : QueryWrapper
    {
        private ETdoFresh.Localbase.DatabaseReference _localbaseReference;
        private Firebase.Database.DatabaseReference _firebaseReference;

        public DatabaseReferenceWrapper(ETdoFresh.Localbase.DatabaseReference reference) : base(reference)
            => _localbaseReference = reference;

        public DatabaseReferenceWrapper(Firebase.Database.DatabaseReference reference) : base(reference)
            => _firebaseReference = reference;

        // Implicit conversion from Localbase.DatabaseReference to DatabaseReferenceWrapper
        public static implicit operator DatabaseReferenceWrapper(ETdoFresh.Localbase.DatabaseReference reference) =>
            new DatabaseReferenceWrapper(reference);

        // Implicit conversion from Firebase.Database.DatabaseReference to DatabaseReferenceWrapper
        public static implicit operator DatabaseReferenceWrapper(Firebase.Database.DatabaseReference reference) =>
            new DatabaseReferenceWrapper(reference);

        public DatabaseWrapper Database =>
            (DatabaseWrapper)_localbaseReference?.Database ?? _firebaseReference.Database;

        public DatabaseReferenceWrapper Parent =>
            (DatabaseReferenceWrapper)_localbaseReference?.Parent ?? _firebaseReference.Parent;

        public DatabaseReferenceWrapper Root =>
            (DatabaseReferenceWrapper)_localbaseReference?.Root ?? _firebaseReference.Root;

        public DatabaseReferenceWrapper Child(string pathString) =>
            (DatabaseReferenceWrapper)_localbaseReference?.Child(pathString) ?? _firebaseReference.Child(pathString);

        public DatabaseReferenceWrapper Push() =>
            (DatabaseReferenceWrapper)_localbaseReference?.Push() ?? _firebaseReference.Push();

        public Task SetValueAsync(object value) =>
            _localbaseReference?.SetValueAsync(value) ?? _firebaseReference.SetValueAsync(value);

        public Task SetRawJsonValueAsync(string jsonValue) =>
            _localbaseReference?.SetRawJsonValueAsync(jsonValue) ?? _firebaseReference.SetRawJsonValueAsync(jsonValue);

        // public Task SetValueAsync(object value, object priority) =>
        //     _localbaseReference?.SetValueAsync(value, priority) ?? _firebaseReference.SetValueAsync(value, priority);
        //
        // public Task SetRawJsonValueAsync(string jsonValue, object priority) =>
        //     _localbaseReference?.SetRawJsonValueAsync(jsonValue, priority) ??
        //     _firebaseReference.SetRawJsonValueAsync(jsonValue, priority);
        //
        // public Task SetPriorityAsync(object priority) =>
        //     _localbaseReference?.SetPriorityAsync(priority) ?? _firebaseReference.SetPriorityAsync(priority);

        public Task UpdateChildrenAsync(IDictionary<string, object> update) =>
            _localbaseReference?.UpdateChildrenAsync(update) ?? _firebaseReference.UpdateChildrenAsync(update);

        public Task RemoveValueAsync() =>
            _localbaseReference?.RemoveValueAsync() ?? _firebaseReference.RemoveValueAsync();

        // public Firebase.Database.OnDisconnect OnDisconnect() =>
        //     _localbaseReference?.OnDisconnect() ?? _firebaseReference.OnDisconnect();
        //
        // public Task<DataSnapshot> RunTransaction(Func<MutableData, TransactionResult> transaction) =>
        //     _localbaseReference?.RunTransaction(transaction) ?? _firebaseReference.RunTransaction(transaction);
        //
        // public Task<DataSnapshot> RunTransaction(Func<MutableData, TransactionResult> transaction,
        //     bool fireLocalEvents) =>
        //     _localbaseReference?.RunTransaction(transaction, fireLocalEvents) ??
        //     _firebaseReference.RunTransaction(transaction, fireLocalEvents);
        //
        // public static void GoOffline() => _localbaseReference?.GoOffline() ?? _firebaseReference.GoOffline();
        //
        // public static void GoOnline() => _localbaseReference?.GoOnline() ?? _firebaseReference.GoOnline();

        public override string ToString() => _localbaseReference?.ToString() ?? _firebaseReference.ToString();

        public string Key => _localbaseReference?.Key ?? _firebaseReference.Key;

        public override bool Equals(object other) =>
            _localbaseReference?.Equals(other) ?? _firebaseReference.Equals(other);

        public override int GetHashCode() => _localbaseReference?.GetHashCode() ?? _firebaseReference.GetHashCode();
    }

    public class DatabaseWrapper
    {
        private ETdoFresh.Localbase.LocalbaseDatabase _localbaseDatabase;
        private Firebase.Database.FirebaseDatabase _firebaseDatabase;

        public DatabaseWrapper(ETdoFresh.Localbase.LocalbaseDatabase database) => _localbaseDatabase = database;
        public DatabaseWrapper(Firebase.Database.FirebaseDatabase database) => _firebaseDatabase = database;

        // Implicit conversion from Localbase.LocalbaseDatabase to DatabaseWrapper
        public static implicit operator DatabaseWrapper(ETdoFresh.Localbase.LocalbaseDatabase database) =>
            new DatabaseWrapper(database);

        // Implicit conversion from Firebase.FirebaseDatabase to DatabaseWrapper
        public static implicit operator DatabaseWrapper(Firebase.Database.FirebaseDatabase database) =>
            new DatabaseWrapper(database);

        // public AppWrapper App => (AppWrapper)_localbaseDatabase?.App ?? _firebaseDatabase.App;

        public static DatabaseWrapper DefaultInstance => ETdoFresh.Localbase.LocalbaseDatabase.DefaultInstance;
        //public static DatabaseWrapper DefaultInstance => Firebase.Database.FirebaseDatabase.DefaultInstance;

        // public static DatabaseWrapper GetInstance(AppWrapper app) =>
        //     (DatabaseWrapper)LocalbaseDatabase.GetInstance(app) ?? Firebase.Database.FirebaseDatabase.GetInstance(app);
        //
        // internal static DatabaseWrapper AnyInstance => (DatabaseWrapper)LocalbaseDatabase.AnyInstance ??
        //                                                 Firebase.Database.FirebaseDatabase.AnyInstance;

        public static DatabaseWrapper GetInstance(string url) =>
            (DatabaseWrapper)ETdoFresh.Localbase.LocalbaseDatabase.GetInstance(url) ?? Firebase.Database.FirebaseDatabase.GetInstance(url);

        // public static DatabaseWrapper GetInstance(AppWrapper app, string url) =>
        //     (DatabaseWrapper)LocalbaseDatabase.GetInstance(app, url) ??
        //     Firebase.Database.FirebaseDatabase.GetInstance(app, url);

        public DatabaseReferenceWrapper RootReference =>
            (DatabaseReferenceWrapper)_localbaseDatabase?.RootReference ?? _firebaseDatabase.RootReference;

        public DatabaseReferenceWrapper GetReference(string path) =>
            (DatabaseReferenceWrapper)_localbaseDatabase?.GetReference(path) ?? _firebaseDatabase.GetReference(path);

        // public DatabaseReferenceWrapper GetReferenceFromUrl(Uri url) =>
        //     (DatabaseReferenceWrapper)_localbaseDatabase?.GetReferenceFromUrl(url) ??
        //     _firebaseDatabase.GetReferenceFromUrl(url);
        //
        // public DatabaseReferenceWrapper GetReferenceFromUrl(string url) =>
        //     (DatabaseReferenceWrapper)_localbaseDatabase?.GetReferenceFromUrl(url) ??
        //     _firebaseDatabase.GetReferenceFromUrl(url);

        // public void PurgeOutstandingWrites() => _localbaseDatabase?.PurgeOutstandingWrites() ??
        //                                          _firebaseDatabase.PurgeOutstandingWrites();
        //
        // public void GoOnline() => _localbaseDatabase?.GoOnline() ?? _firebaseDatabase.GoOnline();
        //
        // public void GoOffline() => _localbaseDatabase?.GoOffline() ?? _firebaseDatabase.GoOffline();

        public void SetPersistenceEnabled(bool enabled)
        {
            if (_localbaseDatabase != null) _localbaseDatabase.SetPersistenceEnabled(enabled);
            else _firebaseDatabase.SetPersistenceEnabled(enabled);
        }

        // public LogLevel LogLevel => _localbaseDatabase?.LogLevel ?? _firebaseDatabase.LogLevel;
    }

    public class QueryWrapper
    {
        private ETdoFresh.Localbase.Query _localbaseQuery;
        private Firebase.Database.Query _firebaseQuery;

        public QueryWrapper(ETdoFresh.Localbase.Query query) => _localbaseQuery = query;
        public QueryWrapper(Firebase.Database.Query query) => _firebaseQuery = query;

        // Implicit conversion from Localbase.Query to QueryWrapper
        public static implicit operator QueryWrapper(ETdoFresh.Localbase.Query query) =>
            new QueryWrapper(query);

        // Implicit conversion from Firebase.Query to QueryWrapper
        public static implicit operator QueryWrapper(Firebase.Database.Query query) =>
            new QueryWrapper(query);

        public event EventHandler<ValueChangedEventArgsWrapper> ValueChanged
        {
            add
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ValueChanged += ValueChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ValueChanged += ValueChangedEventArgsWrapper.ConvertFirebase(value);
            }
            remove
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ValueChanged -= ValueChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ValueChanged -= ValueChangedEventArgsWrapper.ConvertFirebase(value);
            }
        }

        public event EventHandler<ChildChangedEventArgsWrapper> ChildAdded
        {
            add
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildAdded += ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildAdded += ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
            remove
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildAdded -= ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildAdded -= ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
        }

        public event EventHandler<ChildChangedEventArgsWrapper> ChildChanged
        {
            add
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildChanged += ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildChanged += ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
            remove
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildChanged -= ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildChanged -= ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
        }

        public event EventHandler<ChildChangedEventArgsWrapper> ChildRemoved
        {
            add
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildRemoved += ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildRemoved += ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
            remove
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildRemoved -= ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildRemoved -= ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
        }

        public event EventHandler<ChildChangedEventArgsWrapper> ChildMoved
        {
            add
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildMoved += ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildMoved += ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
            remove
            {
                if (_localbaseQuery != null)
                    _localbaseQuery.ChildMoved -= ChildChangedEventArgsWrapper.ConvertLocalbase(value);
                else
                    _firebaseQuery.ChildMoved -= ChildChangedEventArgsWrapper.ConvertFirebase(value);
            }
        }

        public async Task<DataSnapshotWrapper> GetValueAsync()
        {
            if (_localbaseQuery != null) return await _localbaseQuery.GetValueAsync();
            return await _firebaseQuery.GetValueAsync();
        }

        public void KeepSynced(bool keepSynced)
        {
            if (_localbaseQuery != null) _localbaseQuery.KeepSynced(keepSynced);
            else _firebaseQuery.KeepSynced(keepSynced);
        }

        public QueryWrapper StartAt(string value) =>
            _localbaseQuery != null ? _localbaseQuery.StartAt(value) : _firebaseQuery.StartAt(value);

        public QueryWrapper StartAt(double value) =>
            _localbaseQuery != null ? _localbaseQuery.StartAt(value) : _firebaseQuery.StartAt(value);

        public QueryWrapper StartAt(bool value) =>
            _localbaseQuery != null ? _localbaseQuery.StartAt(value) : _firebaseQuery.StartAt(value);

        public QueryWrapper StartAt(string value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.StartAt(value, key) : _firebaseQuery.StartAt(value, key);

        public QueryWrapper StartAt(double value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.StartAt(value, key) : _firebaseQuery.StartAt(value, key);

        public QueryWrapper StartAt(bool value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.StartAt(value, key) : _firebaseQuery.StartAt(value, key);

        public QueryWrapper EndAt(string value) =>
            _localbaseQuery != null ? _localbaseQuery.EndAt(value) : _firebaseQuery.EndAt(value);

        public QueryWrapper EndAt(double value) =>
            _localbaseQuery != null ? _localbaseQuery.EndAt(value) : _firebaseQuery.EndAt(value);

        public QueryWrapper EndAt(bool value) =>
            _localbaseQuery != null ? _localbaseQuery.EndAt(value) : _firebaseQuery.EndAt(value);

        public QueryWrapper EndAt(string value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.EndAt(value, key) : _firebaseQuery.EndAt(value, key);

        public QueryWrapper EndAt(double value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.EndAt(value, key) : _firebaseQuery.EndAt(value, key);

        public QueryWrapper EndAt(bool value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.EndAt(value, key) : _firebaseQuery.EndAt(value, key);

        public QueryWrapper EqualTo(string value) =>
            _localbaseQuery != null ? _localbaseQuery.EqualTo(value) : _firebaseQuery.EqualTo(value);

        public QueryWrapper EqualTo(double value) =>
            _localbaseQuery != null ? _localbaseQuery.EqualTo(value) : _firebaseQuery.EqualTo(value);

        public QueryWrapper EqualTo(bool value) =>
            _localbaseQuery != null ? _localbaseQuery.EqualTo(value) : _firebaseQuery.EqualTo(value);

        public QueryWrapper EqualTo(string value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.EqualTo(value, key) : _firebaseQuery.EqualTo(value, key);

        public QueryWrapper EqualTo(double value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.EqualTo(value, key) : _firebaseQuery.EqualTo(value, key);

        public QueryWrapper EqualTo(bool value, string key) =>
            _localbaseQuery != null ? _localbaseQuery.EqualTo(value, key) : _firebaseQuery.EqualTo(value, key);

        public QueryWrapper LimitToFirst(int limit) =>
            _localbaseQuery != null ? _localbaseQuery.LimitToFirst(limit) : _firebaseQuery.LimitToFirst(limit);

        public QueryWrapper LimitToLast(int limit) =>
            _localbaseQuery != null ? _localbaseQuery.LimitToLast(limit) : _firebaseQuery.LimitToLast(limit);

        public QueryWrapper OrderByChild(string path) =>
            _localbaseQuery != null ? _localbaseQuery.OrderByChild(path) : _firebaseQuery.OrderByChild(path);

        public QueryWrapper OrderByPriority() =>
            _localbaseQuery != null ? _localbaseQuery.OrderByPriority() : _firebaseQuery.OrderByPriority();

        public QueryWrapper OrderByKey() =>
            _localbaseQuery != null ? _localbaseQuery.OrderByKey() : _firebaseQuery.OrderByKey();

        public QueryWrapper OrderByValue() =>
            _localbaseQuery != null ? _localbaseQuery.OrderByValue() : _firebaseQuery.OrderByValue();

        public DatabaseReferenceWrapper Reference =>
            (DatabaseReferenceWrapper)_localbaseQuery?.Reference ?? _firebaseQuery.Reference;
    }

    public class ValueChangedEventArgsWrapper
    {
        private ETdoFresh.Localbase.ValueChangedEventArgs _localbaseArgs;
        private Firebase.Database.ValueChangedEventArgs _firebaseArgs;

        public ValueChangedEventArgsWrapper(ETdoFresh.Localbase.ValueChangedEventArgs args) => _localbaseArgs = args;
        public ValueChangedEventArgsWrapper(Firebase.Database.ValueChangedEventArgs args) => _firebaseArgs = args;

        // Implicit conversion from Localbase.ValueChangedEventArgs to ValueChangedEventArgsWrapper
        public static implicit operator ValueChangedEventArgsWrapper(ETdoFresh.Localbase.ValueChangedEventArgs args) =>
            new ValueChangedEventArgsWrapper(args);

        // Implicit conversion from Firebase.ValueChangedEventArgs to ValueChangedEventArgsWrapper
        public static implicit operator ValueChangedEventArgsWrapper(Firebase.Database.ValueChangedEventArgs args) =>
            new ValueChangedEventArgsWrapper(args);

        public static EventHandler<Firebase.Database.ValueChangedEventArgs> ConvertFirebase(
            EventHandler<ValueChangedEventArgsWrapper> handler) =>
            (sender, args) => handler?.Invoke(sender, args);

        public static EventHandler<ETdoFresh.Localbase.ValueChangedEventArgs> ConvertLocalbase(
            EventHandler<ValueChangedEventArgsWrapper> handler) =>
            (sender, args) => handler?.Invoke(sender, args);

        public DataSnapshotWrapper Snapshot =>
            (DataSnapshotWrapper)_localbaseArgs?.Snapshot ?? _firebaseArgs.Snapshot;

        public DatabaseErrorWrapper DatabaseError =>
            (DatabaseErrorWrapper)_localbaseArgs?.DatabaseError ?? _firebaseArgs.DatabaseError;
    }

    public class ChildChangedEventArgsWrapper
    {
        private ETdoFresh.Localbase.ChildChangedEventArgs _localbaseArgs;
        private Firebase.Database.ChildChangedEventArgs _firebaseArgs;

        public ChildChangedEventArgsWrapper(ETdoFresh.Localbase.ChildChangedEventArgs args) => _localbaseArgs = args;
        public ChildChangedEventArgsWrapper(Firebase.Database.ChildChangedEventArgs args) => _firebaseArgs = args;

        // Implicit conversion from Localbase.ChildChangedEventArgs to ChildChangedEventArgsWrapper
        public static implicit operator ChildChangedEventArgsWrapper(ETdoFresh.Localbase.ChildChangedEventArgs args) =>
            new ChildChangedEventArgsWrapper(args);

        // Implicit conversion from Firebase.ChildChangedEventArgs to ChildChangedEventArgsWrapper
        public static implicit operator ChildChangedEventArgsWrapper(Firebase.Database.ChildChangedEventArgs args) =>
            new ChildChangedEventArgsWrapper(args);

        public static EventHandler<Firebase.Database.ChildChangedEventArgs> ConvertFirebase(
            EventHandler<ChildChangedEventArgsWrapper> handler) =>
            (sender, args) => handler?.Invoke(sender, args);

        public static EventHandler<ETdoFresh.Localbase.ChildChangedEventArgs> ConvertLocalbase(
            EventHandler<ChildChangedEventArgsWrapper> handler) =>
            (sender, args) => handler?.Invoke(sender, args);

        public DataSnapshotWrapper Snapshot =>
            (DataSnapshotWrapper)_localbaseArgs?.Snapshot ?? _firebaseArgs.Snapshot;

        public string PreviousChildName =>
            _localbaseArgs?.PreviousChildName ?? _firebaseArgs.PreviousChildName;
    }

    public class DatabaseErrorWrapper
    {
        private ETdoFresh.Localbase.DatabaseError _localbaseError;
        private Firebase.Database.DatabaseError _firebaseError;

        public DatabaseErrorWrapper(ETdoFresh.Localbase.DatabaseError error) => _localbaseError = error;
        public DatabaseErrorWrapper(Firebase.Database.DatabaseError error) => _firebaseError = error;

        // Implicit conversion from Localbase.DatabaseError to DatabaseErrorWrapper
        public static implicit operator DatabaseErrorWrapper(ETdoFresh.Localbase.DatabaseError error) =>
            new DatabaseErrorWrapper(error);

        // Implicit conversion from Firebase.DatabaseError to DatabaseErrorWrapper
        public static implicit operator DatabaseErrorWrapper(Firebase.Database.DatabaseError error) =>
            new DatabaseErrorWrapper(error);

        public string Message => _localbaseError?.Message ?? _firebaseError.Message;

        public int Code => _localbaseError?.Code ?? _firebaseError.Code;

        public override string ToString() => _localbaseError?.ToString() ?? _firebaseError.ToString();
    }
}