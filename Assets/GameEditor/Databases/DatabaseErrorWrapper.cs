namespace GameEditor.Databases
{
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