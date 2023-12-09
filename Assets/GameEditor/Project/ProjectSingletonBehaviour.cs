using ETdoFresh.Localbase;
using ETdoFresh.UnityPackages.EventBusSystem;
using UnityEngine;

namespace GameEditor.Project
{
    public class ProjectSingletonBehaviour : MonoBehaviourLazyLoadedSingleton<ProjectSingletonBehaviour>
    {
        [SerializeField] private string guid;
        [SerializeField] private ProjectJsonObject projectData;
        private DatabaseReference _projects;
        private DatabaseReference _currentProject;
        
        private void Awake()
        {
            // TODO: Temporary fix since Awake is private in packaged parent class
            var baseType = typeof(MonoBehaviourLazyLoadedSingleton<>).MakeGenericType(GetType());
            var baseAwake = baseType.GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            baseAwake.Invoke(this, null);
            
            _projects = LocalbaseDatabase.DefaultInstance.GetReference("projects");
            if (!string.IsNullOrEmpty(guid)) 
                SetGuid(guid);
        }

        private void OnDestroy()
        {
            _currentProject?.Destroy();
            _projects?.Destroy();
        }

        private void OnEnable()
        {
            _currentProject?.ValueChanged.AddListener(OnProjectChanged);
        }
        
        private void OnDisable()
        {
            _currentProject?.ValueChanged.RemoveListener(OnProjectChanged);
        }

        public static void SetGuid(string guid)
        {
            if (Instance == null) return;
            var projects = Instance._projects;
            var currentProject = Instance._currentProject;
            if (currentProject != null)
            {
                currentProject.ValueChanged.RemoveListener(OnProjectChanged);
                currentProject.Destroy();
            }
            else if (guid == "game-editor-test")
            {
                
            }
            currentProject = projects.Child(guid);
            currentProject.ValueChanged.AddListener(OnProjectChanged);
        }

        private static void OnProjectChanged(ValueChangedEventArgs e)
        {
            var projectData = e.Snapshot.GetValue<ProjectJsonObject>();
            Debug.Log($"[{nameof(ProjectSingletonBehaviour)}] {nameof(OnProjectChanged)} {projectData?.name} {projectData?.guid}");
            Instance.projectData = projectData;
        }
    }
}
