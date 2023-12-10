using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GameEditor.References
{
    // Responsible for caching http requests to persistent storage
    public static class HttpCache
    {
        private const string CachePath = "cache";
        
        public static async Task<string> GetTextAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            
            var cachePath = GetCachePath(url, ".txt");
            if (File.Exists(cachePath))
                return await File.ReadAllTextAsync(cachePath);

            var webRequest = UnityWebRequest.Get(url);
            var tcs = new TaskCompletionSource<string>();
            webRequest.SendWebRequest().completed += operation =>
            {
                if (webRequest.error != null)
                    tcs.SetException(new WebException(webRequest.error));
                else
                    tcs.SetResult(webRequest.downloadHandler.text);
            };
            var text = await tcs.Task;
            Directory.CreateDirectory(Path.GetDirectoryName(cachePath));
            await File.WriteAllTextAsync(cachePath, text);
            return text;
        }
        
        public static async Task<byte[]> GetBytesAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            
            var cachePath = GetCachePath(url, ".bytes");
            if (File.Exists(cachePath))
                return await File.ReadAllBytesAsync(cachePath);

            var webRequest = UnityWebRequest.Get(url);
            var tcs = new TaskCompletionSource<byte[]>();
            webRequest.SendWebRequest().completed += operation =>
            {
                if (webRequest.error != null)
                    tcs.SetException(new WebException(webRequest.error));
                else
                    tcs.SetResult(webRequest.downloadHandler.data);
            };
            var bytes = await tcs.Task;
            Directory.CreateDirectory(Path.GetDirectoryName(cachePath));
            await File.WriteAllBytesAsync(cachePath, bytes);
            return bytes;
        }

        private static string GetCachePath(string url, string backupExtension)
        {
            var uri = new System.Uri(url);
            url = uri.AbsoluteUri;
            var queryIndex = url.IndexOf('?');
            if (queryIndex >= 0) url = url[..queryIndex];
            var extension = Path.GetExtension(url);
            if (string.IsNullOrEmpty(extension)) extension = backupExtension;
            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(url));
            var fileName = System.BitConverter.ToString(hash).Replace("-", "").ToLower();
            var persistentDataPath = UnityEngine.Application.persistentDataPath;
            var cachePath = Path.Combine(persistentDataPath, CachePath);
            return Path.Combine(cachePath, $"{fileName}{extension}");
        }
    }
}