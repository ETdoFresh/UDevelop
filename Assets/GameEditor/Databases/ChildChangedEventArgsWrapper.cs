using System;

namespace GameEditor.Databases
{
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
        
        public override string ToString() => _localbaseArgs?.ToString() ?? _firebaseArgs.ToString();
    }
}