using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameEditor.Databases
{
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
}