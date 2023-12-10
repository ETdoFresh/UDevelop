using System;
using System.Threading.Tasks;
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

        public static async Task<Uri> UploadText(string objectName, string text, string contentType = "text/plain")
        {
            return await UploadBytes(objectName, System.Text.Encoding.UTF8.GetBytes(text), contentType);
        }

        public static async Task<Uri> UploadBytes(string objectName, byte[] bytes, string contentType = null)
        {
            if (_app == null)
            {
                await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.Exception != null)
                    {
                        UnityEngine.Debug.LogError(task.Exception);
                        return;
                    }

                    _app = FirebaseApp.DefaultInstance;
                });
            }

            var storage = FirebaseStorage.GetInstance(_app, Bucket);
            var storageRef = storage.GetReferenceFromUrl(Bucket);
            var obj = storageRef.Child(objectName);
            await obj.PutBytesAsync(bytes);
            if (contentType != null)
            {
                await obj.UpdateMetadataAsync(new MetadataChange { ContentType = contentType });
            }

            // Return download URL
            return await obj.GetDownloadUrlAsync();
        }
    }
}