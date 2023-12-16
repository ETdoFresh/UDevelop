using System;
using System.Collections.Generic;
using System.Linq;
using Battlehub.Utils;
using Cysharp.Threading.Tasks;
using GameEditor.Databases;
using GameEditor.Organizations;
using GameEditor.Project;
using GameEditor.References;
using GameEditor.Storages;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
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
        Organization, Project, User, Scene, Package, Prefab, Image, Audio,
        Script, Material, Model, Text
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
    private string[] _scenePreviews;
    private PackageJsonObject[] _packageListItems;
    private PackageJsonObject _currentPackageListItem;
    private string[] _packagePreviews;
    private PrefabJsonObject[] _prefabListItems;
    private PrefabJsonObject _currentPrefabListItem;
    private string[] _prefabPreviews;
    private ImageJsonObject[] _imageListItems;
    private ImageJsonObject _currentImageListItem;
    private Texture2D[] _imagePreviews;
    private AudioJsonObject[] _audioListItems;
    private AudioJsonObject _currentAudioListItem;
    private AudioClip[] _audioPreviews;
    private ScriptJsonObject[] _scriptListItems;
    private ScriptJsonObject _currentScriptListItem;
    private string[] _scriptPreviews;
    private MaterialJsonObject[] _materialListItems;
    private MaterialJsonObject _currentMaterialListItem;
    private string[] _materialPreviews;
    private ModelJsonObject[] _modelListItems;
    private ModelJsonObject _currentModelListItem;
    private string[] _modelPreviews;
    private TextJsonObject[] _textListItems;
    private TextJsonObject _currentTextListItem;
    private string[] _textPreviews;
    private MonoScript _script;

    private ReferenceType _referenceType;
    private ReferenceType _previousReferenceType;
    private bool _referenceTypeChangedThisFrame;
    private OperationType _operationType;
    private OperationType _previousOperationType;
    private bool _operationTypeChangedThisFrame;
    private Vector2 _scrollViewPosition;

    private void OnGUI()
    {
        _script ??= MonoScript.FromScriptableObject(this);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("Open Database"))
            Database.OpenDatabaseFromEditor();
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
        if (_referenceTypeChangedThisFrame)
            _operationType = OperationType.List;

        switch (_operationType)
        {
            case OperationType.List:

                if (_referenceTypeChangedThisFrame || _operationTypeChangedThisFrame)
                {
                    _scrollViewPosition = Vector2.zero;
                    _currentProjectsPath = ProjectsPath;
                    Database.GetValueAsync(_currentProjectsPath).ContinueWith(value =>
                    {
                        var jObject = value != null ? JObject.FromObject(value) : new JObject();
                        var properties = jObject.Properties().ToArray();
                        var propertiesCount = properties.Length;
                        _projectListItems = new ProjectJsonObject[propertiesCount];
                        for (var i = 0; i < propertiesCount; i++)
                            _projectListItems[i] = (properties[i].Value as JObject)?.ToObject<ProjectJsonObject>();
                    });
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Projects", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                _scrollViewPosition =
                    EditorGUILayout.BeginScrollView(_scrollViewPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

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

                EditorGUILayout.EndScrollView();
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
                    Database.AddObjectChild(ProjectsPath, _currentProjectListItem.guid, _currentProjectListItem);
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
                    _ = Database.SetValueAsync($"{ProjectsPath}.{_currentProjectListItem.guid}",
                        _currentProjectListItem);
                    _operationType = OperationType.List;
                }

                EditorGUILayout.EndHorizontal();
                break;
            case OperationType.Delete:
                if (_operationTypeChangedThisFrame)
                {
                    Database.RemoveObjectChild(ProjectsPath, _currentProjectListItem.guid);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Delete Project", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("You have deleted the following project:");
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

    private void OnImagesGUI()
    {
        if (_referenceTypeChangedThisFrame)
            _operationType = OperationType.List;

        switch (_operationType)
        {
            case OperationType.List:

                if (_referenceTypeChangedThisFrame || _operationTypeChangedThisFrame)
                {
                    _scrollViewPosition = Vector2.zero;
                    _imageListItems = Array.Empty<ImageJsonObject>();
                    Database.GetValueAsync(ImagesPath).ContinueWith(value =>
                    {
                        var jObject = value != null ? JObject.FromObject(value) : new JObject();
                        var itemHistories = jObject.Properties().ToArray();
                        var itemHistoriesLength = itemHistories.Length;
                        _imageListItems = new ImageJsonObject[itemHistoriesLength];
                        for (var i = 0; i < itemHistoriesLength; i++)
                        {
                            var itemHistoryJObject = itemHistories[i].Value as JObject;
                            _imageListItems[i] =
                                DatabaseTickUtility.GetValueAtUtcNow<ImageJsonObject>(itemHistoryJObject);
                        }
                    });
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Image Assets", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                _scrollViewPosition =
                    EditorGUILayout.BeginScrollView(_scrollViewPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

                _imageListItems ??= Array.Empty<ImageJsonObject>();
                var listItemsCount = _imageListItems.Length;
                for (var i = 0; i < listItemsCount; i++)
                {
                    var listItem = _imageListItems[i];
                    var isDeleted = listItem?.deletedUtcTicks.HasValue == true;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(listItem?.name ?? "null");
                    EditorGUILayout.LabelField(listItem?.guid ?? "null");
                    if (GUILayout.Button("Read"))
                    {
                        _operationType = OperationType.Read;
                        _currentImageListItem = listItem;
                    }

                    EditorGUI.BeginDisabledGroup(isDeleted);
                    if (GUILayout.Button(isDeleted ? "Deleted" : "Update"))
                    {
                        _operationType = OperationType.Update;
                        _currentImageListItem = listItem;
                    }

                    if (GUILayout.Button(isDeleted ? "Deleted" : "Delete"))
                    {
                        _operationType = OperationType.Delete;
                        _currentImageListItem = listItem;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Create"))
                {
                    _operationType = OperationType.Create;
                    _currentImageListItem = new ImageJsonObject { guid = Guid.NewGuid().ToString("N") };
                }

                EditorGUILayout.EndScrollView();
                break;

            case OperationType.Read:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Read Image Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                if (_operationTypeChangedThisFrame)
                {
                    _imageListItems = Array.Empty<ImageJsonObject>();
                    _imagePreviews = Array.Empty<Texture2D>();
                    var guid = _currentImageListItem.guid;
                    Database.GetValueAsync($"{ImagesPath}.{guid}").ContinueWith(value =>
                    {
                        var jObject = JObject.FromObject(value);
                        var itemHistories = jObject?.Properties().ToArray() ?? Array.Empty<JProperty>();
                        var itemHistoriesLength = itemHistories.Length;
                        itemHistories = itemHistories.OrderBy(x => long.Parse(x.Name)).ToArray();
                        _imageListItems = new ImageJsonObject[itemHistoriesLength];
                        for (var i = 0; i < itemHistoriesLength; i++)
                        {
                            var itemHistoryJObject = itemHistories[i].Value as JObject;
                            var imageAsset = itemHistoryJObject?.ToObject<ImageJsonObject>();
                            _imageListItems[i] = imageAsset;
                        }

                        _imagePreviews = new Texture2D[itemHistoriesLength];
                        for (var i = 0; i < itemHistoriesLength; i++)
                        {
                            var imagePath = _imageListItems[i]?.path;
                            if (string.IsNullOrEmpty(imagePath)) continue;
                            var index = i;
                            HttpCache.GetBytesAsync(imagePath).ContinueWith(textureValue =>
                                _imagePreviews[index] = GetTexture2DFromBytes(textureValue));
                        }
                    });
                }

                _imageListItems ??= Array.Empty<ImageJsonObject>();
                _imagePreviews ??= Array.Empty<Texture2D>();
                var imageListItemsCount = _imageListItems.Length;
                for (var i = 0; i < imageListItemsCount; i++)
                {
                    var imageListItem = _imageListItems[i];
                    var isDeleted = imageListItem?.deletedUtcTicks.HasValue == true;
                    var lastModifiedUtc = new DateTime(imageListItem.lastModifiedUtcTicks);
                    var lastModified = lastModifiedUtc.ToLocalTime();
                    var lastModifiedString = lastModified.ToString("yyyy-MM-dd HH:mm:ss");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(imageListItem.guid);
                    EditorGUILayout.LabelField(lastModifiedString);
                    EditorGUILayout.LabelField(imageListItem.name);
                    EditorGUILayout.EndHorizontal();

                    if (isDeleted)
                    {
                        EditorGUILayout.LabelField("Deleted");
                    }
                    else
                    {
                        EditorGUILayout.LabelField(imageListItem.path);
                        EditorGUILayout.LabelField("Cache", HttpCache.GetCacheFilename(imageListItem.path, ".bytes"));

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(_imagePreviews[i], typeof(Texture2D), false);
                        EditorGUI.EndDisabledGroup();
                    }

                    if (i < imageListItemsCount - 1)
                        EditorGUILayout.Space();
                }

                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                break;

            case OperationType.Update:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Update Image Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                if (_operationTypeChangedThisFrame)
                {
                    _imagePreviews = new Texture2D[1];
                    _imagePreviews[0] = null;
                    HttpCache.GetBytesAsync(_currentImageListItem.path).ContinueWith(textureValue =>
                        _imagePreviews[0] = GetTexture2DFromBytes(textureValue));
                }

                EditorGUILayout.BeginHorizontal();
                _currentImageListItem.name = EditorGUILayout.TextField("Name", _currentImageListItem.name);
                EditorGUILayout.LabelField("Guid", _currentImageListItem.guid);
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginDisabledGroup(_imagePreviews[0] == null);
                _imagePreviews[0] = (Texture2D)EditorGUILayout.ObjectField(_imagePreviews[0], typeof(Texture2D), false);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                if (GUILayout.Button("Update"))
                {
                    var ext = System.IO.Path.GetExtension(_currentImageListItem.name);
                    var guid = _currentImageListItem.guid;
                    var utcNowTicks = DateTime.UtcNow.Ticks;
                    var objectName = $"{guid}-{utcNowTicks}{ext}";
                    GoogleCloudStorage.UploadBytes(objectName, _imagePreviews[0].DeCompress().EncodeToPNG())
                        .ContinueWith(
                            downloadUri =>
                            {
                                _currentImageListItem.lastModifiedUtcTicks = utcNowTicks;
                                _currentImageListItem.path = downloadUri.ToString();
                                Database.AddObjectChild($"{ImagesPath}.{guid}", utcNowTicks.ToString(),
                                    _currentImageListItem);
                                _operationType = OperationType.List;
                            });
                }

                EditorGUILayout.EndHorizontal();
                break;

            case OperationType.Create:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Create Image Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                if (_operationTypeChangedThisFrame)
                {
                    _imagePreviews = new Texture2D[1];
                    _imagePreviews[0] = null;
                }

                EditorGUILayout.BeginHorizontal();
                _currentImageListItem.name = EditorGUILayout.TextField("Name", _currentImageListItem.name);
                EditorGUILayout.LabelField("Guid", _currentImageListItem.guid);
                EditorGUILayout.EndHorizontal();

                _imagePreviews[0] = (Texture2D)EditorGUILayout.ObjectField(_imagePreviews[0], typeof(Texture2D), false);

                if (string.IsNullOrEmpty(_currentImageListItem.name) && _imagePreviews[0] &&
                    !string.IsNullOrEmpty(_imagePreviews[0].name))
                    _currentImageListItem.name = $"{_imagePreviews[0].name}.png";

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                if (GUILayout.Button("Create"))
                {
                    var ext = System.IO.Path.GetExtension(_currentImageListItem.name);
                    var guid = _currentImageListItem.guid;
                    var path = $"{ImagesPath}.{guid}";
                    var utcNowTicks = DateTime.UtcNow.Ticks;
                    var utcNowTicksString = utcNowTicks.ToString();
                    var objectName = $"{guid}-{utcNowTicks}{ext}";
                    GoogleCloudStorage.UploadBytes(objectName, _imagePreviews[0].DeCompress().EncodeToPNG())
                        .ContinueWith(
                            downloadUri =>
                            {
                                //var imageReference = LocalbaseDatabase.DefaultInstance.GetReference($"{ImagesPath}.{guid}");
                                _currentImageListItem.createdUtcTicks = utcNowTicks;
                                _currentImageListItem.lastModifiedUtcTicks = utcNowTicks;
                                _currentImageListItem.path = downloadUri.ToString();
                                Database.GetValueAsync($"{ImagesPath}.{guid}").ContinueWith(value =>
                                {
                                    if (value == null)
                                    {
                                        var dictionary = new Dictionary<string, object>
                                            { { utcNowTicksString, _currentImageListItem } };
                                        _ = Database.SetValueAsync(path, dictionary);
                                        _operationType = OperationType.List;
                                    }
                                    else
                                    {
                                        Database.AddObjectChild(path, utcNowTicksString, _currentImageListItem);
                                        _operationType = OperationType.List;
                                    }
                                });
                            });
                }

                EditorGUILayout.EndHorizontal();
                break;

            case OperationType.Delete:
                if (_operationTypeChangedThisFrame)
                {
                    var guid = _currentImageListItem.guid;
                    var utcNowTicks = DateTime.UtcNow.Ticks;
                    var utcNowTicksString = utcNowTicks.ToString();
                    var path = $"{ImagesPath}.{guid}";
                    _currentImageListItem.lastModifiedUtcTicks = utcNowTicks;
                    _currentImageListItem.deletedUtcTicks = utcNowTicks;
                    _currentImageListItem.path = null;
                    Database.AddObjectChild(path, utcNowTicksString, _currentImageListItem);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Delete Image Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("You have deleted the following image asset:");
                EditorGUILayout.LabelField(_currentImageListItem.name);
                EditorGUILayout.LabelField(_currentImageListItem.guid);
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                break;
        }
    }

    private void OnAudioGUI() { }
    private void OnScriptsGUI() { }
    private void OnMaterialsGUI() { }
    private void OnModelsGUI() { }

    private void OnTextsGUI()
    {
        if (_referenceTypeChangedThisFrame)
            _operationType = OperationType.List;

        switch (_operationType)
        {
            case OperationType.List:

                if (_referenceTypeChangedThisFrame || _operationTypeChangedThisFrame)
                {
                    _scrollViewPosition = Vector2.zero;
                    _textListItems = Array.Empty<TextJsonObject>();
                    Database.GetValueAsync(TextsPath).ContinueWith(value =>
                    {
                        var jObject = value != null ? JObject.FromObject(value) : new JObject();
                        var itemHistories = jObject.Properties().ToArray();
                        var itemHistoriesLength = itemHistories.Length;
                        _textListItems = new TextJsonObject[itemHistoriesLength];
                        for (var i = 0; i < itemHistoriesLength; i++)
                        {
                            var itemHistoryJObject = itemHistories[i].Value as JObject;
                            _textListItems[i] =
                                DatabaseTickUtility.GetValueAtUtcNow<TextJsonObject>(itemHistoryJObject);
                        }
                    });
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Text Assets", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                _scrollViewPosition =
                    EditorGUILayout.BeginScrollView(_scrollViewPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

                _textListItems ??= Array.Empty<TextJsonObject>();
                var listItemsCount = _textListItems.Length;
                for (var i = 0; i < listItemsCount; i++)
                {
                    var listItem = _textListItems[i];
                    var isDeleted = listItem?.deletedUtcTicks.HasValue == true;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(listItem?.name ?? "null");
                    EditorGUILayout.LabelField(listItem?.guid ?? "null");
                    if (GUILayout.Button("Read"))
                    {
                        _operationType = OperationType.Read;
                        _currentTextListItem = listItem;
                    }

                    EditorGUI.BeginDisabledGroup(isDeleted);
                    if (GUILayout.Button(isDeleted ? "Deleted" : "Update"))
                    {
                        _operationType = OperationType.Update;
                        _currentTextListItem = listItem;
                    }

                    if (GUILayout.Button(isDeleted ? "Deleted" : "Delete"))
                    {
                        _operationType = OperationType.Delete;
                        _currentTextListItem = listItem;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Create"))
                {
                    _operationType = OperationType.Create;
                    _currentTextListItem = new TextJsonObject { guid = Guid.NewGuid().ToString("N") };
                }

                EditorGUILayout.EndScrollView();
                break;
            case OperationType.Read:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Read Text Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                if (_operationTypeChangedThisFrame)
                {
                    _textListItems = Array.Empty<TextJsonObject>();
                    _textPreviews = Array.Empty<string>();
                    var guid = _currentTextListItem.guid;
                    Database.GetValueAsync($"{TextsPath}.{guid}").ContinueWith(value =>
                    {
                        var jObject = JObject.FromObject(value);
                        var itemHistories = jObject.Properties().ToArray() ?? Array.Empty<JProperty>();
                        var itemHistoriesLength = itemHistories.Length;
                        itemHistories = itemHistories.OrderBy(x => long.Parse(x.Name)).ToArray();
                        _textListItems = new TextJsonObject[itemHistoriesLength];
                        for (var i = 0; i < itemHistoriesLength; i++)
                        {
                            var itemHistoryJObject = itemHistories[i].Value as JObject;
                            var textAsset = itemHistoryJObject?.ToObject<TextJsonObject>();
                            _textListItems[i] = textAsset;
                        }

                        _textPreviews = new string[itemHistoriesLength];
                        for (var i = 0; i < itemHistoriesLength; i++)
                        {
                            var textPath = _textListItems[i]?.path;
                            if (string.IsNullOrEmpty(textPath)) continue;
                            var index = i;
                            HttpCache.GetTextAsync(textPath)
                                .ContinueWith(textValue => _textPreviews[index] = textValue);
                        }
                    });
                }

                _textListItems ??= Array.Empty<TextJsonObject>();
                _textPreviews ??= Array.Empty<string>();
                var textListItemsCount = _textListItems.Length;
                for (var i = 0; i < textListItemsCount; i++)
                {
                    var textListItem = _textListItems[i];
                    var isDeleted = textListItem?.deletedUtcTicks.HasValue == true;
                    var lastModifiedUtc = new DateTime(textListItem.lastModifiedUtcTicks);
                    var lastModified = lastModifiedUtc.ToLocalTime();
                    var lastModifiedString = lastModified.ToString("yyyy-MM-dd HH:mm:ss");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(textListItem.guid);
                    EditorGUILayout.LabelField(lastModifiedString);
                    EditorGUILayout.LabelField(textListItem.name);
                    EditorGUILayout.EndHorizontal();

                    if (isDeleted)
                    {
                        EditorGUILayout.LabelField("Deleted");
                    }
                    else
                    {
                        EditorGUILayout.LabelField(textListItem.path);
                        EditorGUILayout.LabelField("Cache", HttpCache.GetCacheFilename(textListItem.path, ".txt"));

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextArea(_textPreviews[i]);
                        EditorGUI.EndDisabledGroup();
                    }

                    if (i < textListItemsCount - 1)
                        EditorGUILayout.Space();
                }

                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                break;
            case OperationType.Update:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Update Text Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                if (_operationTypeChangedThisFrame)
                {
                    _textPreviews = new string[1];
                    _textPreviews[0] = null;
                    HttpCache.GetTextAsync(_currentTextListItem.path)
                        .ContinueWith(textValue => _textPreviews[0] = textValue);
                }

                EditorGUILayout.BeginHorizontal();
                _currentTextListItem.name = EditorGUILayout.TextField("Name", _currentTextListItem.name);
                EditorGUILayout.LabelField("Guid", _currentTextListItem.guid);
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginDisabledGroup(_textPreviews[0] == null);
                _textPreviews[0] = EditorGUILayout.TextArea(_textPreviews[0]);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                if (GUILayout.Button("Update"))
                {
                    var ext = System.IO.Path.GetExtension(_currentTextListItem.name);
                    var guid = _currentTextListItem.guid;
                    var utcNowTicks = DateTime.UtcNow.Ticks;
                    var objectName = $"{guid}-{utcNowTicks}{ext}";
                    GoogleCloudStorage.UploadText(objectName, _textPreviews[0]).ContinueWith(downloadUri =>
                    {
                        _currentTextListItem.lastModifiedUtcTicks = utcNowTicks;
                        _currentTextListItem.path = downloadUri.ToString();
                        Database.AddObjectChild($"{TextsPath}.{guid}", utcNowTicks.ToString(), _currentTextListItem);
                        _operationType = OperationType.List;
                    });
                }

                EditorGUILayout.EndHorizontal();
                break;
            case OperationType.Create:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Create Text Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                if (_operationTypeChangedThisFrame)
                {
                    _textPreviews = new string[1];
                    _textPreviews[0] = "";
                }

                EditorGUILayout.BeginHorizontal();
                _currentTextListItem.name = EditorGUILayout.TextField("Name", _currentTextListItem.name);
                EditorGUILayout.LabelField("Guid", _currentTextListItem.guid);
                EditorGUILayout.EndHorizontal();

                _textPreviews[0] = EditorGUILayout.TextArea(_textPreviews[0]);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                if (GUILayout.Button("Create"))
                {
                    var ext = System.IO.Path.GetExtension(_currentTextListItem.name);
                    var guid = _currentTextListItem.guid;
                    var path = $"{TextsPath}.{guid}";
                    var utcNowTicks = DateTime.UtcNow.Ticks;
                    var utcNowTicksString = utcNowTicks.ToString();
                    var objectName = $"{guid}-{utcNowTicks}{ext}";
                    GoogleCloudStorage.UploadText(objectName, _textPreviews[0]).ContinueWith(downloadUri =>
                    {
                        //var textReference = LocalbaseDatabase.DefaultInstance.GetReference($"{TextsPath}.{guid}");
                        _currentTextListItem.createdUtcTicks = utcNowTicks;
                        _currentTextListItem.lastModifiedUtcTicks = utcNowTicks;
                        _currentTextListItem.path = downloadUri.ToString();
                        Database.GetValueAsync($"{TextsPath}.{guid}").ContinueWith(value =>
                        {
                            if (value == null)
                            {
                                var dictionary = new Dictionary<string, object>
                                    { { utcNowTicksString, _currentTextListItem } };
                                _ = Database.SetValueAsync(path, dictionary);
                                _operationType = OperationType.List;
                            }
                            else
                            {
                                Database.AddObjectChild(path, utcNowTicksString, _currentTextListItem);
                                _operationType = OperationType.List;
                            }
                        });
                    });
                }

                EditorGUILayout.EndHorizontal();
                break;
            case OperationType.Delete:
                if (_operationTypeChangedThisFrame)
                {
                    var guid = _currentTextListItem.guid;
                    var utcNowTicks = DateTime.UtcNow.Ticks;
                    var utcNowTicksString = utcNowTicks.ToString();
                    var path = $"{TextsPath}.{guid}";
                    _currentTextListItem.lastModifiedUtcTicks = utcNowTicks;
                    _currentTextListItem.deletedUtcTicks = utcNowTicks;
                    _currentTextListItem.path = null;
                    Database.AddObjectChild(path, utcNowTicksString, _currentTextListItem);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Delete Text Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("You have deleted the following text asset:");
                EditorGUILayout.LabelField(_currentTextListItem.name);
                EditorGUILayout.LabelField(_currentTextListItem.guid);
                if (GUILayout.Button("Back"))
                    _operationType = OperationType.List;
                break;
        }
    }

    private Texture2D GetTexture2DFromBytes(byte[] bytes)
    {
        var texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return texture;
    }
}