using ETdoFresh.UnityPackages.DataBusSystem;
using GameEditor.References;
using UnityEngine;

namespace ETdoFresh.Localbase
{
    public class LocalbaseFileMonitor : MonoBehaviourLazyLoadedSingleton<LocalbaseFileMonitor>
    {
        public static void Touch()
        {
            if (!Application.isPlaying) return;
            HttpCache.Initialize();
            if (Instance) return;
            Debug.LogWarning($"[{nameof(LocalbaseFileMonitor)}] could not be initialized. You are probably quitting the application.");
        }
        
        private void Update()
        {
            foreach (var entry in LocalbaseDatabase.Databases)
            {
                var database = entry.Value;
                var path = database.Path;
                if (!System.IO.File.Exists(path)) continue;
                var lastWriteTime = System.IO.File.GetLastWriteTime(path);
                if (lastWriteTime <= database.LastReadWriteTime) continue;
                database.UpdateFromFile();
            }
        }
    }
}
