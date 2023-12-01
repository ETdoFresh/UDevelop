using System;

namespace ETdoFresh.Localbase
{
    public class ValueChangedEventArgs : EventArgs
    {
        public DataSnapshot Snapshot { get; private set; }
        public DatabaseError DatabaseError { get; private set; }

        internal ValueChangedEventArgs(DataSnapshot snapshot) => Snapshot = snapshot;
        internal ValueChangedEventArgs(DatabaseError error) => DatabaseError = error;
    }
}