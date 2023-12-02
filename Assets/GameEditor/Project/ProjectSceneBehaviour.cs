using System;
using System.Collections.Generic;
using ETdoFresh.Localbase;
using ETdoFresh.ReadonlyInspectorAttribute;
using ETdoFresh.UnityPackages.ExtensionMethods;
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
        [SerializeField, Readonly, TextArea(3, 50)] private string _projectsJArrayString;
        [SerializeField, Readonly] private ProjectJsonObject[] _projects;
        private JArray _projectsJArray;

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
        }
        
        private void OnCreateProjectButtonClicked()
        {
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnCreateProjectButtonClicked {projectNameInputField.text} {projectGuidText.text}"); 
            // Create a new project using all the fields and load project
            createProjectUI.SetActive(false);
            selectProjectUI.SetActive(true);
        }

        private void OnLoadProjectButtonClicked(ProjectSlotBehaviour projectSlot)
        {
            var projectGuid = projectSlot.Guid;
            var projectName = projectSlot.Data.name;
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnLoadProjectButtonClicked: {projectName} {projectGuid} {projectSlot}");
            // Load Project by Guid
            createProjectUI.SetActive(false);
            selectProjectUI.SetActive(true);
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
            _projectsJArrayString = null;

            var value = e.Snapshot.Value;
            _projectsJArray = value as JArray;
            if (value != null && _projectsJArray == null)
                throw new Exception(
                    $"[{nameof(ProjectSceneBehaviour)}] OnProjectsValueChanged: Snapshot value is not JObject");

            _projects = _projectsJArray?.ToObject<ProjectJsonObject[]>();
            _projectsJArrayString = _projectsJArray?.ToString();

            Debug.Log(
                $"[{nameof(ProjectSceneBehaviour)}] OnProjectsValueChanged: Project Count: {_projects?.Length ?? -1}");

            projectSlots.ForEach(Destroy);
            projectSlots.Clear();
            
            if (_projects == null) return;
            foreach (var project in _projects)
            {
                var projectSlot = Instantiate(inSceneProjectSlot, projectButtonsParent);
                projectSlot.SetActive(true);
                
                var projectSlotBehaviour = projectSlot.GetComponent<ProjectSlotBehaviour>();
                projectSlotBehaviour.SetData(project, OnLoadProjectButtonClicked);

                var projectButton = projectSlotBehaviour.Button;
                
                projectSlots.Add(projectSlot);
            }
        }
    }
}