using System;
using System.Linq;
using ETdoFresh.Localbase;
using GameEditor.Organizations;
using GameEditor.Project;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static ETdoFresh.Localbase.Paths;

public class ReferenceEditorWindow : EditorWindow
{
    [MenuItem("ETdoFresh/Reference Editor")]
    public static void ShowWindow()
    {
        GetWindow<ReferenceEditorWindow>("Reference Editor");
    }

    private enum ReferenceType
    {
        Database, Organization, Project, User, Scene, Package, Prefab, Image,
        Audio, Script, Material, Model, Text
    }

    private enum OperationType { List, Create, Read, Update, Delete }

    private string _currentOrganizationsPath = OrganizationsPath;
    private string _currentProjectsPath = ProjectsPath;
    private string _currentUsersPath = UsersPath;
    private string _currentScenesPath = ScenesPath;
    private string _currentPackagesPath = PackagesPath;
    private string _currentPrefabsPath = PrefabsPath;
    private string _currentImagesPath = ImagesPath;
    private string _currentAudioPath = AudioPath;
    private string _currentScriptsPath = ScriptsPath;
    private string _currentMaterialsPath = MaterialsPath;
    private string _currentModelsPath = ModelsPath;

    private OrganizationJsonObject[] _organizationListItems;
    private OrganizationJsonObject _currentOrganizationListItem;
    private ProjectJsonObject[] _projectListItems;
    private ProjectJsonObject _currentProjectListItem;
    private UserJsonObject[] _userListItems;
    private UserJsonObject _currentUserListItem;
    private SceneJsonObject[] _sceneListItems;
    private SceneJsonObject _currentSceneListItem;
    private PackageJsonObject[] _packageListItems;
    private PackageJsonObject _currentPackageListItem;
    private PrefabJsonObject[] _prefabListItems;
    private PrefabJsonObject _currentPrefabListItem;
    private ImageJsonObject[] _imageListItems;
    private ImageJsonObject _currentImageListItem;
    private AudioJsonObject[] _audioListItems;
    private AudioJsonObject _currentAudioListItem;
    private ScriptJsonObject[] _scriptListItems;
    private SceneJsonObject _currentScriptListItem;
    private MaterialJsonObject[] _materialListItems;
    private MaterialJsonObject _currentMaterialListItem;
    private ModelJsonObject[] _modelListItems;
    private ModelJsonObject _currentModelListItem;
    private TextJsonObject[] _textListItems;
    private TextJsonObject _currentTextListItem;

    private ReferenceType _referenceType;
    private ReferenceType _previousReferenceType;
    private bool _referenceTypeChangedThisFrame;
    private OperationType _operationType;
    private OperationType _previousOperationType;
    private bool _operationTypeChangedThisFrame;

    private void OnGUI()
    {
        _referenceType = (ReferenceType)EditorGUILayout.EnumPopup("Reference Type", _referenceType);
        EditorGUI.BeginDisabledGroup(true);
        _operationType = (OperationType)EditorGUILayout.EnumPopup("Operation Type", _operationType);
        EditorGUI.EndDisabledGroup();
        _referenceTypeChangedThisFrame = _referenceType != _previousReferenceType;
        _operationTypeChangedThisFrame = _operationType != _previousOperationType;
        _previousReferenceType = _referenceType;
        _previousOperationType = _operationType;

        switch (_referenceType)
        {
            case ReferenceType.Organization:
                OnOrganizationsGUI();
                break;
            case ReferenceType.Project:
                OnProjectsGUI();
                break;
            case ReferenceType.User:
                OnUsersGUI();
                break;
            case ReferenceType.Scene:
                OnScenesGUI();
                break;
            case ReferenceType.Package:
                OnPackagesGUI();
                break;
            case ReferenceType.Prefab:
                OnPrefabsGUI();
                break;
            case ReferenceType.Image:
                OnImagesGUI();
                break;
            case ReferenceType.Audio:
                OnAudioGUI();
                break;
            case ReferenceType.Script:
                OnScriptsGUI();
                break;
            case ReferenceType.Material:
                OnMaterialsGUI();
                break;
            case ReferenceType.Model:
                OnModelsGUI();
                break;
            case ReferenceType.Text:
                OnTextsGUI();
                break;
        }

        _referenceTypeChangedThisFrame = false;
    }

    private void OnOrganizationsGUI() { }

    private void OnProjectsGUI()
    {
        switch (_operationType)
        {
            case OperationType.List:

                if (_referenceTypeChangedThisFrame || _operationTypeChangedThisFrame)
                {
                    _operationType = OperationType.List;
                    _currentProjectsPath = ProjectsPath;
                    var databaseReference = LocalbaseDatabase.DefaultInstance.GetReference(_currentProjectsPath);
                    UnityAction<ValueChangedEventArgs> onValueChanged = null;
                    onValueChanged = (e) =>
                    {
                        databaseReference.ValueChanged.RemoveListener(onValueChanged);
                        var value = e.Snapshot.Value;
                        var jObject = value as JObject;
                        var properties = jObject?.Properties().ToArray() ?? Array.Empty<JProperty>();
                        var propertiesCount = properties.Length;
                        _projectListItems = new ProjectJsonObject[propertiesCount];
                        for (var i = 0; i < propertiesCount; i++)
                            _projectListItems[i] = (properties[i].Value as JObject)?.ToObject<ProjectJsonObject>();
                    };
                    databaseReference.ValueChanged.AddListener(onValueChanged);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Projects", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                _projectListItems ??= Array.Empty<ProjectJsonObject>();
                var projectListItemsCount = _projectListItems.Length;
                for (var i = 0; i < projectListItemsCount; i++)
                {
                    var projectListItem = _projectListItems[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(projectListItem.name);
                    EditorGUILayout.LabelField(projectListItem.guid);
                    if (GUILayout.Button("Read"))
                    {
                        _operationType = OperationType.Read;
                        _currentProjectListItem = projectListItem;
                    }
                    if (GUILayout.Button("Update"))
                    {
                        _operationType = OperationType.Update;
                        _currentProjectListItem = projectListItem;
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        _operationType = OperationType.Delete;
                        _currentProjectListItem = projectListItem;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Create"))
                {
                    _operationType = OperationType.Create;
                    _currentProjectListItem = new ProjectJsonObject { guid = Guid.NewGuid().ToString() };
                }
                break;

            case OperationType.Create:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Create Project", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                _currentProjectListItem.name = EditorGUILayout.TextField("Name", _currentProjectListItem.name);
                _currentProjectListItem.guid = EditorGUILayout.TextField("Guid", _currentProjectListItem.guid);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                if (GUILayout.Button("Create"))
                {
                    var projectReference = LocalbaseDatabase.DefaultInstance.GetReference(ProjectsPath);
                    projectReference.AddObjectChild(_currentProjectListItem.guid, _currentProjectListItem);
                    _operationType = OperationType.List;
                }
                EditorGUILayout.EndHorizontal();
                break;

            case OperationType.Read:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Read Project", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField(_currentProjectListItem.name);
                EditorGUILayout.LabelField(_currentProjectListItem.guid);
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                break;
            case OperationType.Update:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Update Project", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                _currentProjectListItem.name = EditorGUILayout.TextField("Name", _currentProjectListItem.name);
                _currentProjectListItem.guid = EditorGUILayout.TextField("Guid", _currentProjectListItem.guid);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                if (GUILayout.Button("Update"))
                {
                    var projectItemReference =
                        LocalbaseDatabase.DefaultInstance.GetReference(
                            $"{ProjectsPath}.{_currentProjectListItem.guid}");
                    projectItemReference.SetValueAsync(_currentProjectListItem);
                    _operationType = OperationType.List;
                }
                EditorGUILayout.EndHorizontal();
                break;
            case OperationType.Delete:
                if (_operationTypeChangedThisFrame)
                {
                    var databaseReference = LocalbaseDatabase.DefaultInstance.GetReference(ProjectsPath);
                    databaseReference.RemoveObjectChild(_currentProjectListItem.guid);
                }
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Delete Project", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("You have chosen to deleted the following project:");
                EditorGUILayout.LabelField(_currentProjectListItem.name);
                EditorGUILayout.LabelField(_currentProjectListItem.guid);
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                break;
        }
    }

    private void OnUsersGUI() { }
    private void OnScenesGUI() { }
    private void OnPackagesGUI() { }
    private void OnPrefabsGUI() { }
    private void OnImagesGUI() { }
    private void OnAudioGUI() { }
    private void OnScriptsGUI() { }
    private void OnMaterialsGUI() { }
    private void OnModelsGUI() { }

    private void OnTextsGUI()
    {
        // TODO: Implement this using history and timestamps
        switch (_operationType)
        {
            case OperationType.List:
                break;
        }
    }
}
