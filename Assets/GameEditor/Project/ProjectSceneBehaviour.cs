using System;
using System.Collections.Generic;
using ETdoFresh.Localbase;
using ETdoFresh.ReadonlyInspectorAttribute;
using ETdoFresh.UnityPackages.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameEditor.Project
{
    public class ProjectSceneBehaviour : MonoBehaviour
    {
        [Header("Sections")]
        [SerializeField] private GameObject selectProjectUI;
        [SerializeField] private GameObject createProjectUI;

        [Header("Title Bar")]
        [SerializeField] private Button backButton;
        [SerializeField] private string previousSceneName;

        [Header("Select Project Fields")]
        [SerializeField] private Button addProjectButton;
        [SerializeField] private GameObject inSceneProjectSlot;
        [SerializeField, Readonly] private List<GameObject> projectSlots;
        [SerializeField, Readonly] private Transform projectButtonsParent;

        [Header("Create Project Fields")]
        [SerializeField] private TMP_Text projectGuidText;
        [SerializeField] private TMP_InputField projectNameInputField;
        [SerializeField] private TMP_InputField projectVersionInputField;
        [SerializeField] private TMP_InputField projectDescriptionInputField;
        [SerializeField] private TMP_InputField projectAuthorInputField;
        [SerializeField] private TMP_InputField projectLocalPathInputField;
        [SerializeField] private TMP_InputField projectImageLocalPathInputField;
        [SerializeField] private TMP_InputField projectAudioLocalPathInputField;
        [SerializeField] private TMP_InputField projectVideoLocalPathInputField;
        [SerializeField] private TMP_InputField projectFontLocalPathInputField;
        [SerializeField] private TMP_InputField projectScriptLocalPathInputField;
        [SerializeField] private TMP_InputField projectPrefabLocalPathInputField;
        [SerializeField] private TMP_InputField projectScriptableObjectLocalPathInputField;
        [SerializeField] private TMP_InputField projectSceneLocalPathInputField;
        [SerializeField] private TMP_InputField projectMaterialLocalPathInputField;
        [SerializeField] private TMP_InputField projectAnimationLocalPathInputField;
        [SerializeField] private Button createProjectButton;
        [SerializeField] private Button cancelCreateProjectButton;

        [Header("Project Data")]
        [SerializeField, Readonly, TextArea(3, 50)] private string _projectsJObjectString;
        [SerializeField, Readonly] private ProjectJsonObject[] _projects;
        private JObject _projectsJObject;

        private DatabaseReference _projectsDatabaseReference;

        private void OnValidate()
        {
            if (inSceneProjectSlot) projectButtonsParent = inSceneProjectSlot.transform.parent;
        }

        private void Awake()
        {
            _projectsDatabaseReference = LocalbaseDatabase.DefaultInstance.GetReference("projects", this);
            inSceneProjectSlot.SetActive(false);
        }

        private void Start()
        {
            selectProjectUI.SetActive(true);
            createProjectUI.SetActive(false);
        }

        private void OnDestroy()
        {
            _projectsDatabaseReference?.Destroy();
        }

        private void OnEnable()
        {
            backButton.onClick.AddPersistentListener(OnBackButtonClicked);
            addProjectButton.onClick.AddPersistentListener(OnAddProjectButtonClicked);
            createProjectButton.onClick.AddPersistentListener(OnCreateProjectButtonClicked);
            cancelCreateProjectButton.onClick.AddPersistentListener(OnCancelCreateProjectButtonClicked);
            _projectsDatabaseReference.ValueChanged.AddListener(OnProjectsValueChanged);
        }

        private void OnDisable()
        {
            backButton.onClick.RemovePersistentListener(OnBackButtonClicked);
            addProjectButton.onClick.RemovePersistentListener(OnAddProjectButtonClicked);
            createProjectButton.onClick.RemovePersistentListener(OnCreateProjectButtonClicked);
            cancelCreateProjectButton.onClick.RemovePersistentListener(OnCancelCreateProjectButtonClicked);
            _projectsDatabaseReference.ValueChanged.RemoveListener(OnProjectsValueChanged);
        }

        private void OnBackButtonClicked()
        {
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnBackButtonClicked {previousSceneName}");
            if (int.TryParse(previousSceneName, out var sceneIndex) && sceneIndex >= 0 &&
                sceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(sceneIndex);
            else
                SceneManager.LoadScene(previousSceneName);
        }

        private void OnAddProjectButtonClicked()
        {
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnAddProjectButtonClicked");
            createProjectUI.SetActive(true);
            selectProjectUI.SetActive(false);
            projectGuidText.text = Guid.NewGuid().ToString("N");
        }
        
        private void OnCreateProjectButtonClicked()
        {
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnCreateProjectButtonClicked {projectNameInputField.text} {projectGuidText.text}");

            var projectJsonObject = new ProjectJsonObject();
            projectJsonObject.name = projectNameInputField.text;
            projectJsonObject.guid = projectGuidText.text;
            projectJsonObject.version = string.IsNullOrEmpty(projectVersionInputField.text) ? null : projectVersionInputField.text;
            projectJsonObject.description = string.IsNullOrEmpty(projectDescriptionInputField.text) ? null : projectDescriptionInputField.text;
            projectJsonObject.authors = string.IsNullOrEmpty(projectAuthorInputField.text) ? null : projectAuthorInputField.text.Split(',');
            projectJsonObject.localPath = string.IsNullOrEmpty(projectLocalPathInputField.text) ? null : projectLocalPathInputField.text;
            projectJsonObject.localImagePath = string.IsNullOrEmpty(projectImageLocalPathInputField.text) ? null : projectImageLocalPathInputField.text;
            projectJsonObject.localAudioPath = string.IsNullOrEmpty(projectAudioLocalPathInputField.text) ? null : projectAudioLocalPathInputField.text;
            projectJsonObject.localVideoPath = string.IsNullOrEmpty(projectVideoLocalPathInputField.text) ? null : projectVideoLocalPathInputField.text;
            projectJsonObject.localFontPath = string.IsNullOrEmpty(projectFontLocalPathInputField.text) ? null : projectFontLocalPathInputField.text;
            projectJsonObject.localScriptPath = string.IsNullOrEmpty(projectScriptLocalPathInputField.text) ? null : projectScriptLocalPathInputField.text;
            projectJsonObject.localPrefabPath = string.IsNullOrEmpty(projectPrefabLocalPathInputField.text) ? null : projectPrefabLocalPathInputField.text;
            projectJsonObject.localScriptableObjectPath = string.IsNullOrEmpty(projectScriptableObjectLocalPathInputField.text) ? null : projectScriptableObjectLocalPathInputField.text;
            projectJsonObject.localScenePath = string.IsNullOrEmpty(projectSceneLocalPathInputField.text) ? null : projectSceneLocalPathInputField.text;
            projectJsonObject.localMaterialPath = string.IsNullOrEmpty(projectMaterialLocalPathInputField.text) ? null : projectMaterialLocalPathInputField.text;
            projectJsonObject.localAnimationPath = string.IsNullOrEmpty(projectAnimationLocalPathInputField.text) ? null : projectAnimationLocalPathInputField.text;
            
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            var json = JsonConvert.SerializeObject(projectJsonObject, settings);
            var jObject = JObject.Parse(json);
            
            _projectsJObject.Add(projectJsonObject.guid, jObject);
            _projectsDatabaseReference.SetValueAsync(_projectsJObject);
            
            createProjectUI.SetActive(false);
            selectProjectUI.SetActive(true);
        }

        private void OnLoadProjectButtonClicked(ProjectSlotBehaviour projectSlot)
        {
            var data = projectSlot.Data;
            var projectGuid = data.guid;
            var projectName = data.name;
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnLoadProjectButtonClicked: {projectName} {projectGuid} {projectSlot}");
            ProjectSingletonBehaviour.SetGuid(projectGuid);
            createProjectUI.SetActive(false);
            selectProjectUI.SetActive(true);
            SceneManager.LoadScene("RoslynRotateCube");
        }

        private void OnCancelCreateProjectButtonClicked()
        {
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnCancelCreateProjectButtonClicked");
            selectProjectUI.SetActive(true);
            createProjectUI.SetActive(false);
        }

        private void OnProjectsValueChanged(ValueChangedEventArgs e)
        {
            _projects = null;
            _projectsJObjectString = null;

            var value = e.Snapshot.Value;
            _projectsJObject = value as JObject;
            if (value != null && _projectsJObject == null)
                throw new Exception(
                    $"[{nameof(ProjectSceneBehaviour)}] OnProjectsValueChanged: Snapshot value is not JObject");

            var projectDictionary = _projectsJObject?.ToObject<Dictionary<string, ProjectJsonObject>>() ?? new Dictionary<string, ProjectJsonObject>();
            _projects = new ProjectJsonObject[projectDictionary.Count];
            projectDictionary.Values.CopyTo(_projects, 0);
            _projectsJObject ??= new JObject();
            _projectsJObjectString = _projectsJObject.ToString();

            Debug.Log(
                $"[{nameof(ProjectSceneBehaviour)}] OnProjectsValueChanged: Project Count: {_projects?.Length ?? -1}");

            projectSlots.ForEach(Destroy);
            projectSlots.Clear();
            
            foreach (var project in _projects)
            {
                var projectSlot = Instantiate(inSceneProjectSlot, projectButtonsParent);
                projectSlot.SetActive(true);
                
                var projectSlotBehaviour = projectSlot.GetComponent<ProjectSlotBehaviour>();
                projectSlotBehaviour.SetData(project, OnLoadProjectButtonClicked);
                
                projectSlots.Add(projectSlot);
            }
        }
    }
}