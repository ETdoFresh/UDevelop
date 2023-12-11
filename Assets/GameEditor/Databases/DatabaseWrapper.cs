namespace GameEditor.Databases
{
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
}