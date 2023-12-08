using System;

namespace ETdoFresh.Localbase
{
    public class ChildChangedEventArgs : EventArgs, IDoNotInvokeOnAddListenerWhenNull
    {
        public DataSnapshot Snapshot { get; private set; }
        public DatabaseError DatabaseError { get; private set; }
        public string PreviousChildName { get; private set; }

        internal ChildChangedEventArgs(DataSnapshot snapshot, string previousChildName)
        {
            Snapshot = snapshot;
            PreviousChildName = previousChildName;
        }

        internal ChildChangedEventArgs(DatabaseError error) => DatabaseError = error;
    }
}