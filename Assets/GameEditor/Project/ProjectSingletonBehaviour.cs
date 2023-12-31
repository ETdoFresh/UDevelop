using ETdoFresh.Localbase;
using ETdoFresh.UnityPackages.EventBusSystem;
using GameEditor.Databases;
using Newtonsoft.Json;
using UnityEngine;
using ValueChangedEventArgs = Firebase.Database.ValueChangedEventArgs;

namespace GameEditor.Project
{
    public class ProjectSingletonBehaviour : MonoBehaviourLazyLoadedSingleton<ProjectSingletonBehaviour>
    {
        [SerializeField] private string guid;
        [SerializeField] private ProjectJsonObject projectData;
        
        private string ProjectPath => $"{Paths.ProjectsPath}/{guid}";
        
        private void Awake()
        {
            // TODO: Temporary fix since Awake is private in packaged parent class
            var baseType = typeof(MonoBehaviourLazyLoadedSingleton<>).MakeGenericType(GetType());
            var baseAwake = baseType.GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            baseAwake.Invoke(this, null);
        }

        private void OnEnable()
        {
            SetGuid(guid);
        }
        
        private void OnDisable()
        {
            if (!string.IsNullOrEmpty(guid))
                Database.RemoveValueChangedListener(ProjectPath, OnProjectChanged);
        }

        public static void SetGuid(string newGuid)
        {
            if (Instance == null) return;
            
            if (!string.IsNullOrEmpty(Instance.guid))
                Database.RemoveValueChangedListener(Instance.ProjectPath, OnProjectChanged);
            
            Instance.guid = newGuid;
            Database.AddValueChangedListener(Instance.ProjectPath, OnProjectChanged);
        }

        private static void OnProjectChanged(object sender, IValueChangedEventArgs e)
        {
            var projectData = JsonConvert.DeserializeObject<ProjectJsonObject>(e.SnapshotGetRawJsonValue());
            Debug.Log($"[{nameof(ProjectSingletonBehaviour)}] {nameof(OnProjectChanged)} {projectData?.name} {projectData?.guid}");
            Instance.projectData = projectData;
        }
    }
}
