using System;
using System.Collections.Generic;
using ETdoFresh.Localbase;
using ETdoFresh.ReadonlyInspectorAttribute;
using ETdoFresh.SceneReferences;
using ETdoFresh.UnityPackages.ExtensionMethods;
using Firebase.Extensions;
using GameEditor.References;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ETdoFresh.Localbase.Paths;

namespace GameEditor.Project
{
    public class ProjectSceneBehaviour : MonoBehaviour
    {
        [Header("Sections")]
        [SerializeField] private GameObject selectProjectUI;
        [SerializeField] private GameObject createProjectUI;

        [Header("Title Bar")]
        [SerializeField] private SceneReference previousSceneReference;
        [SerializeField] private Button backButton;

        [Header("Select Project Fields")]
        [SerializeField] private SceneReference loadProjectSceneReference;
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

        private void OnValidate()
        {
            if (inSceneProjectSlot) projectButtonsParent = inSceneProjectSlot.transform.parent;
        }

        private void Awake()
        {
            inSceneProjectSlot.SetActive(false);
        }

        private void Start()
        {
            selectProjectUI.SetActive(true);
            createProjectUI.SetActive(false);
        }

        private void OnEnable()
        {
            backButton.onClick.AddPersistentListener(OnBackButtonClicked);
            addProjectButton.onClick.AddPersistentListener(OnAddProjectButtonClicked);
            createProjectButton.onClick.AddPersistentListener(OnCreateProjectButtonClicked);
            cancelCreateProjectButton.onClick.AddPersistentListener(OnCancelCreateProjectButtonClicked);
            Database.ChildAdded.AddListener(ProjectsPath, OnProjectsChildAdded);
            Database.ChildChanged.AddListener(ProjectsPath, OnProjectsChildChanged);
            Database.ChildRemoved.AddListener(ProjectsPath, OnProjectsChildRemoved);
            Database.ChildMoved.AddListener(ProjectsPath, OnProjectsChildMoved);
            Database.GetValueAsync(ProjectsPath).ContinueWithOnMainThread(task => OnProjectsValueChanged(task.Result));
        }

        private void OnDisable()
        {
            backButton.onClick.RemovePersistentListener(OnBackButtonClicked);
            addProjectButton.onClick.RemovePersistentListener(OnAddProjectButtonClicked);
            createProjectButton.onClick.RemovePersistentListener(OnCreateProjectButtonClicked);
            cancelCreateProjectButton.onClick.RemovePersistentListener(OnCancelCreateProjectButtonClicked);
            Database.ChildAdded.RemoveListener(ProjectsPath, OnProjectsChildAdded);
            Database.ChildChanged.RemoveListener(ProjectsPath, OnProjectsChildChanged);
            Database.ChildRemoved.RemoveListener(ProjectsPath, OnProjectsChildRemoved);
            Database.ChildMoved.RemoveListener(ProjectsPath, OnProjectsChildMoved);
        }

        private void OnBackButtonClicked()
        {
            Debug.Log(
                $"[{nameof(ProjectSceneBehaviour)}] OnBackButtonClicked {previousSceneReference.sceneIndex} {previousSceneReference.sceneName}");
            SceneManager.LoadScene(previousSceneReference.sceneIndex);
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
            Debug.Log(
                $"[{nameof(ProjectSceneBehaviour)}] OnCreateProjectButtonClicked {projectNameInputField.text} {projectGuidText.text}");

            var projectJsonObject = new ProjectJsonObject();
            projectJsonObject.name = projectNameInputField.text;
            projectJsonObject.guid = projectGuidText.text;
            projectJsonObject.version = NullIfEmpty(projectVersionInputField.text);
            projectJsonObject.description = NullIfEmpty(projectDescriptionInputField.text);
            projectJsonObject.authors = NullIfEmpty(projectAuthorInputField.text)?.Split(',');
            projectJsonObject.localPath = NullIfEmpty(projectLocalPathInputField.text);
            projectJsonObject.localImagePath = NullIfEmpty(projectImageLocalPathInputField.text);
            projectJsonObject.localAudioPath = NullIfEmpty(projectAudioLocalPathInputField.text);
            projectJsonObject.localVideoPath = NullIfEmpty(projectVideoLocalPathInputField.text);
            projectJsonObject.localFontPath = NullIfEmpty(projectFontLocalPathInputField.text);
            projectJsonObject.localScriptPath = NullIfEmpty(projectScriptLocalPathInputField.text);
            projectJsonObject.localPrefabPath = NullIfEmpty(projectPrefabLocalPathInputField.text);
            projectJsonObject.localScriptableObjectPath = NullIfEmpty(projectScriptableObjectLocalPathInputField.text);
            projectJsonObject.localScenePath = NullIfEmpty(projectSceneLocalPathInputField.text);
            projectJsonObject.localMaterialPath = NullIfEmpty(projectMaterialLocalPathInputField.text);
            projectJsonObject.localAnimationPath = NullIfEmpty(projectAnimationLocalPathInputField.text);
            Database.Object.AddChild(ProjectsPath, projectJsonObject.guid, projectJsonObject);

            createProjectUI.SetActive(false);
            selectProjectUI.SetActive(true);
        }

        private static string NullIfEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private void OnLoadProjectButtonClicked(ProjectSlotBehaviour projectSlot)
        {
            var data = projectSlot.Data;
            var projectGuid = data.guid;
            var projectName = data.name;
            Debug.Log(
                $"[{nameof(ProjectSceneBehaviour)}] OnLoadProjectButtonClicked: {projectName} {projectGuid} {projectSlot}");
            ProjectSingletonBehaviour.SetGuid(projectGuid);
            createProjectUI.SetActive(false);
            selectProjectUI.SetActive(true);
            SceneManager.LoadScene(loadProjectSceneReference.sceneIndex);
        }

        private void OnCancelCreateProjectButtonClicked()
        {
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnCancelCreateProjectButtonClicked");
            selectProjectUI.SetActive(true);
            createProjectUI.SetActive(false);
        }

        private void OnProjectsChildAdded(object value)
        {
            var newProject = (value as JObject)?.ToObject<ProjectJsonObject>();
            Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnProjectsChildAdded {newProject.name} {newProject.guid}");

            var projectSlot = Instantiate(inSceneProjectSlot, projectButtonsParent);
            projectSlot.SetActive(true);

            var projectSlotBehaviour = projectSlot.GetComponent<ProjectSlotBehaviour>();
            projectSlotBehaviour.SetData(newProject, OnLoadProjectButtonClicked);

            projectSlots.Add(projectSlot);
        }

        private void OnProjectsChildRemoved(object value)
        {
            var removedProject = (value as JObject)?.ToObject<ProjectJsonObject>();
            Debug.Log(
                $"[{nameof(ProjectSceneBehaviour)}] OnProjectsChildRemoved {removedProject.name} {removedProject.guid}");
            var projectSlot = projectSlots.Find(slot =>
                slot.GetComponent<ProjectSlotBehaviour>().Data.guid == removedProject.guid);
            if (projectSlot == null) return;
            projectSlots.Remove(projectSlot);
            Destroy(projectSlot);
        }

        private void OnProjectsChildMoved(object value)
        {
            // var oldProjectGuid = e.PreviousChildName;
            // var newProjectGuid = e.Snapshot.Key;
            // Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnProjectsChildMoved {oldProjectGuid} {newProjectGuid}");
            // var projectSlot =
            //     projectSlots.Find(slot => slot.GetComponent<ProjectSlotBehaviour>().Data.guid == oldProjectGuid);
            // if (projectSlot == null) return;
            // projectSlot.GetComponent<ProjectSlotBehaviour>().Data.guid = newProjectGuid;
        }

        private void OnProjectsChildChanged(object value)
        {
            // var databaseReference = e.Snapshot.Reference;
            // while (!databaseReference.IsRoot() && databaseReference.Parent.Key != ProjectsPath)
            //     databaseReference = databaseReference.Parent;
            // var projectGuid = databaseReference.Key;
            // var projectSlot =
            //     projectSlots.Find(slot => slot.GetComponent<ProjectSlotBehaviour>().Data.guid == projectGuid);
            // if (projectSlot == null) return;
            // var newProject = databaseReference.ValueChanged.Value.Snapshot.GetValue<ProjectJsonObject>();
            // Debug.Log($"[{nameof(ProjectSceneBehaviour)}] OnProjectsChildChanged {newProject.name} {newProject.guid}");
            // projectSlot.GetComponent<ProjectSlotBehaviour>().SetData(newProject, OnLoadProjectButtonClicked);
        }

        private void OnProjectsValueChanged(object value)
        {
            if (value != null && value is not JObject)
                throw new Exception(
                    $"[{nameof(ProjectSceneBehaviour)}] OnProjectsValueChanged: Snapshot value is not JObject");

            var projectsJObject = value as JObject ?? new JObject();
            var projectDictionary = projectsJObject.ToObject<Dictionary<string, ProjectJsonObject>>() ??
                                    new Dictionary<string, ProjectJsonObject>();

            Debug.Log(
                $"[{nameof(ProjectSceneBehaviour)}] OnProjectsValueChanged: Project Count: {projectDictionary.Count}");

            projectSlots.ForEach(Destroy);
            projectSlots.Clear();

            foreach (var project in projectDictionary.Values)
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