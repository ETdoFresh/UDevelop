using System;

namespace GameEditor.Databases
{
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

        public override string ToString() => _localbaseArgs?.ToString() ?? _firebaseArgs.ToString();
    }
}