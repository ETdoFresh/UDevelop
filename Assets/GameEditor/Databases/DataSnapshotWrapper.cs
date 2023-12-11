using System.Collections.Generic;
using System.Linq;

namespace GameEditor.Databases
{
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
}