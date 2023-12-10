using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

namespace GameEditor.References
{
    public static class GoogleCloudStorage
    {
        private const string Bucket = "gs://ingameeditor-f4937.appspot.com";
        private static string _defaultBucket;
        private static FirebaseApp _app;
        
        public static void UploadText(string objectName, string text)
        {
            UploadBytes(objectName, System.Text.Encoding.UTF8.GetBytes(text));
        }
        
        public static void UploadBytes(string objectName, byte[] bytes)
        {
            if (_app == null)
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.Exception != null)
                    {
                        UnityEngine.Debug.LogError(task.Exception);
                        return;
                    }

                    _app = FirebaseApp.DefaultInstance;
                    UploadBytes(objectName, bytes);
                });
                return;
            }
            
            var storage = FirebaseStorage.GetInstance(_app, Bucket);
            var storageRef = storage.GetReferenceFromUrl(Bucket);
            var obj = storageRef.Child(objectName);
            obj.PutBytesAsync(bytes);
        }
    }
}