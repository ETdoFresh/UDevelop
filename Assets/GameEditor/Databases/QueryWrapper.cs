using System;
using System.Threading.Tasks;

namespace GameEditor.Databases
{
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
}