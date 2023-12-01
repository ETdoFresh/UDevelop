using ETdoFresh.UnityPackages.EventBusSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameEditor.Project
{
    public class ProjectSingletonBehaviour : MonoBehaviourLazyLoadedSingleton<ProjectSingletonBehaviour>
    {
        [SerializeField] private string projectName;
        [SerializeField] private string projectVersion;
        [SerializeField] private string projectDescription;
        [SerializeField] private string[] projectAuthors;
        [SerializeField] private string[] projectContributors;
        //[SerializeField] private string projectRemotePath;
        [SerializeField] private string projectLocalPath;
        [SerializeField] private string projectLocalImagePath = "{LocalPath}/Images";
        [SerializeField] private string projectLocalAudioPath = "{LocalPath}/Audio";
        [SerializeField] private string projectLocalVideoPath = "{LocalPath}/Video";
        [SerializeField] private string projectLocalFontPath = "{LocalPath}/Fonts";
        [SerializeField] private string projectLocalScriptPath = "{LocalPath}/Scripts";
        //[SerializeField] private string projectLocalShaderPath = "{LocalPath}/Shaders";
        [SerializeField] private string projectLocalPrefabPath = "{LocalPath}/Prefabs";
        [SerializeField] private string projectLocalScriptableObjectPath = "{LocalPath}/ScriptableObjects";
        [SerializeField] private string projectLocalScenePath = "{LocalPath}/Scenes";
        [SerializeField] private string projectLocalMaterialPath = "{LocalPath}/Materials";
        [SerializeField] private string projectLocalAnimationPath = "{LocalPath}/Animations";

        public void Load(string json)
        {
            var JObject = JsonConvert.DeserializeObject<JObject>(json);
            projectName = JObject["name"]?.Value<string>();
            projectVersion = JObject["version"]?.Value<string>();
            projectDescription = JObject["description"]?.Value<string>();
            projectAuthors = JObject["authors"]?.ToObject<string[]>();
            projectContributors = JObject["contributors"]?.ToObject<string[]>();
            //projectRemotePath = JObject["projectRemotePath"]?.Value<string>();
            projectLocalPath = JObject["localPath"]?.Value<string>();
            projectLocalImagePath = JObject["localImagePath"]?.Value<string>();
            projectLocalAudioPath = JObject["localAudioPath"]?.Value<string>();
            projectLocalVideoPath = JObject["localVideoPath"]?.Value<string>();
            projectLocalFontPath = JObject["localFontPath"]?.Value<string>();
            projectLocalScriptPath = JObject["localScriptPath"]?.Value<string>();
            //projectLocalShaderPath = JObject["localShaderPath"]?.Value<string>();
            projectLocalPrefabPath = JObject["localPrefabPath"]?.Value<string>();
            projectLocalScriptableObjectPath = JObject["localScriptableObjectPath"]?.Value<string>();
            projectLocalScenePath = JObject["localScenePath"]?.Value<string>();
            projectLocalMaterialPath = JObject["localMaterialPath"]?.Value<string>();
            projectLocalAnimationPath = JObject["localAnimationPath"]?.Value<string>();
        }
    }
}
