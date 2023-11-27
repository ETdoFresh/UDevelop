using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using Battlehub.Utils;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.RTSL.Interface;
using UnityObject = UnityEngine.Object;
#if UNITY_STANDALONE || UNITY_ANDROID
using Battlehub.CodeAnalysis;
#endif

namespace Battlehub.RTScripting
{
    public interface IRuntimeScriptManager
    {
        event Action Loading;
        event Action Loaded;
        event Action Compiling;
        event Action<bool> Complied;

        bool IsLoaded
        {
            get;
        }


        string Ext
        {
            get;
        }

        Type GetType(string name);
        void AddReference(string assemblyLocation);
        void AddReference(byte[] peImage);
        void ClearReferences();

        Task<ProjectItem> CreateScriptAsync(ProjectItem folder, string name = null);
        Task<RuntimeTextAsset> LoadScriptAsync(ProjectItem assetItem);
        Task SaveScriptAsync(ProjectItem assetItem, RuntimeTextAsset script);
        Task CompileAsync();

        //[Obsolete] 
        void CreateScript(ProjectItem folder);
        //[Obsolete] 
        ProjectAsyncOperation<RuntimeTextAsset> LoadScript(ProjectItem assetItem);
        //[Obsolete] 
        ProjectAsyncOperation SaveScript(ProjectItem assetItem, RuntimeTextAsset script);
        //[Obsolete] 
        ProjectAsyncOperation Compile();
    }

    [Serializable]
    public class RuntimeTypeGuid
    {
        public string FullName;
        public string Guid;
    }

    [Serializable]
    public class RuntimeTypeGuids
    {
        public RuntimeTypeGuid[] Guids;
    }


    [DefaultExecutionOrder(-1)]
    public class RuntimeScriptsManager : MonoBehaviour, IRuntimeScriptManager
    {
        public event Action Loading;
        public event Action Loaded;
        public event Action Compiling;
        public event Action<bool> Complied;

        public bool IsLoaded
        {
            get;
            private set;
        }

        private const string RuntimeAssemblyKey = "RuntimeAssembly";
        private const string RuntimeTypeGuids = "RuntimeTypeGuids";
        public string Ext
        {
            get { return ".cs"; }
        }

        private IProjectAsync m_project;
        private IEditorsMap m_editorsMap;
        private ITypeMap m_typeMap;
        private ILocalization m_localization;
#if UNITY_STANDALONE || UNITY_ANDROID
        private ICompiler m_compiler;
#endif
        private Assembly m_runtimeAssembly;
        private Dictionary<string, Guid> m_typeNameToGuid;
        private RuntimeTextAsset m_runtimeTypeGuidsAsset;

        private void Awake()
        {
#if UNITY_STANDALONE || UNITY_ANDROID
            m_compiler = IOC.Resolve<ICompiler>();
#endif
            m_localization = IOC.Resolve<ILocalization>();
            m_project = IOC.Resolve<IProjectAsync>();
            m_project.Events.OpenProjectCompleted += OnOpenProjectCompleted;
            m_project.Events.DeleteCompleted += OnDeleteCompleted;

            m_editorsMap = IOC.Resolve<IEditorsMap>();
            m_typeMap = IOC.Resolve<ITypeMap>();
            IOC.RegisterFallback<IRuntimeScriptManager>(this);

            if (m_project.State.IsOpened)
            {
                LoadAsync();
            }
        }

        private class ReferencesList
        {
            public string[] references;
        }

        private IEnumerator Start()
        {
#if UNITY_STANDALONE

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    m_compiler.AddReference(assembly.Location);
                }
                catch (Exception)
                {
                }
            }

            yield break;
#elif UNITY_ANDROID

            UnityWebRequest uwr = UnityWebRequest.Get($"{Application.streamingAssetsPath}/RTScripting.References.json");
            yield return uwr.SendWebRequest();

            string[] references = new string[0];
            if (uwr.error == null)
            {
                ReferencesList referencesList = JsonUtility.FromJson<ReferencesList>(uwr.downloadHandler.text);
                references = referencesList.references;
            }

            foreach(string reference in references)
            {
                uwr = UnityWebRequest.Get($"{Application.streamingAssetsPath}/{reference}");
                yield return uwr.SendWebRequest();
                if(uwr.error == null)
                {
                    m_compiler.AddReference(uwr.downloadHandler.data);
                }
            }
#else
            yield break;
#endif
        }

        private void OnDestroy()
        {
            if (m_project != null)
            {
                m_project.Events.OpenProjectCompleted -= OnOpenProjectCompleted;
                m_project.Events.DeleteCompleted -= OnDeleteCompleted;
            }

#if UNITY_STANDALONE || UNITY_ANDROID
            if (m_compiler != null)
            {
                m_compiler.ClearReferences();
            }
#endif

            IOC.UnregisterFallback<IRuntimeScriptManager>(this);
            UnloadTypes(false);
        }
        private void OnOpenProjectCompleted(object sender, ProjectEventArgs<ProjectInfo> e)
        {
            LoadAsync();
        }

        private async void LoadAsync()
        {
            IsLoaded = false;

            Loading?.Invoke();

            RuntimeBinaryAsset assemblyAsset = null;
            try
            {
                assemblyAsset = await m_project.Safe.GetValueAsync<RuntimeBinaryAsset>(RuntimeAssemblyKey);
            }
            catch (StorageException e)
            {
                if (e.ErrorCode != Error.E_NotFound)
                {
                    throw;
                }
            }

            if (assemblyAsset == null)
            {
                m_runtimeTypeGuidsAsset = ScriptableObject.CreateInstance<RuntimeTextAsset>();
                m_typeNameToGuid = new Dictionary<string, Guid>();
            }
            else
            {
                m_runtimeTypeGuidsAsset = await m_project.Safe.GetValueAsync<RuntimeTextAsset>(RuntimeTypeGuids);
                m_typeNameToGuid = new Dictionary<string, Guid>();

                string xml = m_runtimeTypeGuidsAsset.Text;
                if (!string.IsNullOrEmpty(xml))
                {
                    RuntimeTypeGuids typeGuids = XmlUtility.FromXml<RuntimeTypeGuids>(xml);
                    foreach (RuntimeTypeGuid typeGuid in typeGuids.Guids)
                    {
                        Guid guid;
                        if (!m_typeNameToGuid.ContainsKey(typeGuid.FullName) && Guid.TryParse(typeGuid.Guid, out guid))
                        {
                            m_typeNameToGuid.Add(typeGuid.FullName, guid);
                        }
                    }
                }

                LoadAssembly(assemblyAsset.Data);
            }

            IsLoaded = true;
            Loaded?.Invoke();
        }

        private void OnDeleteCompleted(object sender, ProjectEventArgs<ProjectItem[]> e)
        {
            for (int i = 0; i < e.Payload.Length; ++i)
            {
                if (!e.Payload[i].IsFolder)
                {
                    if (e.Payload[i].Ext == Ext)
                    {
                        CompileAsync();
                        break;
                    }
                }
            }
        }

        public Type GetType(string fullName)
        {
            return m_runtimeAssembly.GetType(fullName);
        }

        public void AddReference(string assemblyLocation)
        {
#if UNITY_STANDALONE || UNITY_ANDROID
            m_compiler.AddReference(assemblyLocation);
#endif
        }

        public void AddReference(byte[] peImage)
        {
#if UNITY_STANDALONE || UNITY_ANDROID
            m_compiler.AddReference(peImage);
#endif
        }

        public void ClearReferences()
        {
#if UNITY_STANDALONE || UNITY_ANDROID
            m_compiler.ClearReferences();
#endif
        }

        public Task<ProjectItem> CreateScriptAsync(ProjectItem folder, string name = null)
        {
            return CreateScript(folder, name, true);
        }
        public async Task<RuntimeTextAsset> LoadScriptAsync(ProjectItem assetItem)
        {
            UnityObject[] loadedObjects = await m_project.Safe.LoadAsync(new[] { assetItem });
            return loadedObjects[0] as RuntimeTextAsset;
        }

        public Task SaveScriptAsync(ProjectItem assetItem, RuntimeTextAsset script)
        {
            return m_project.Safe.SaveAsync(new[] { assetItem }, new[] { script });
        }

        public Task CompileAsync()
        {
            return CompileAsync(null);
        }

        public async void CreateScript(ProjectItem folder)
        {
            await CreateScript(folder, null, true);
        }

        private async Task<ProjectItem> CreateScript(ProjectItem folder, string name, bool activeProjectFolder)
        {
            name = m_project.Utils.GetUniqueName(!string.IsNullOrEmpty(name) ? name : m_localization.GetString("ID_RTScripting_ScriptsManager_Script", "Script"), Ext, folder, true);

            string nl = Environment.NewLine;
            RuntimeTextAsset csFile = ScriptableObject.CreateInstance<RuntimeTextAsset>();
            csFile.name = name;
            csFile.Ext = Ext;
            csFile.Text =
                "using System.Collections;" + nl +
                "using System.Collections.Generic;" + nl +
                "using UnityEngine;" + nl + nl +

                "public class " + name + " : MonoBehaviour" + nl +
                "{" + nl +
                "    // Start is called before the first frame update" + nl +
                "    void Start()" + nl +
                "    {" + nl +
                "    }" + nl + nl +

                "    // Update is called once per frame" + nl +
                "    void Update()" + nl +
                "    {" + nl +
                "    }" + nl +
                "}";


            IRTE rte = IOC.Resolve<IRTE>();
            IResourcePreviewUtility resourcePreview = IOC.Resolve<IResourcePreviewUtility>();
            byte[] preview = resourcePreview.CreatePreviewData(csFile);

            rte.IsBusy = true;
            ProjectItem[] assetItem = await m_project.Safe.SaveAsync(new[] { folder }, new[] { preview }, new[] { csFile }, null);
            rte.IsBusy = false;

            Destroy(csFile);

            if (activeProjectFolder)
            {
                var projectFolder = rte.GetWindow(RuntimeWindowType.ProjectFolder);
                if (projectFolder != null)
                {
                    rte.ActivateWindow(projectFolder);
                }
            }

            return assetItem[0];
        }

        [Obsolete] //12.11.2020
        public ProjectAsyncOperation<RuntimeTextAsset> LoadScript(ProjectItem assetItem)
        {
            ProjectAsyncOperation<RuntimeTextAsset> ao = new ProjectAsyncOperation<RuntimeTextAsset>();
            LoadScriptAsync(assetItem, ao);
            return ao;
        }

        private async void LoadScriptAsync(ProjectItem assetItem, ProjectAsyncOperation<RuntimeTextAsset> ao)
        {
            try
            {
                UnityObject[] loadedObjects = await m_project.Safe.LoadAsync(new[] { assetItem });
                ao.Result = (RuntimeTextAsset)loadedObjects[0];
                ao.Error = Error.NoError;
            }
            catch (Exception e)
            {
                ao.Error = new Error(Error.E_Failed);
                Debug.LogException(e);
            }
            finally
            {
                ao.IsCompleted = true;
            }

        }

        [Obsolete] //12.11.2020
        public ProjectAsyncOperation SaveScript(ProjectItem assetItem, RuntimeTextAsset script)
        {
            ProjectAsyncOperation ao = new ProjectAsyncOperation();
            SaveScriptAsync(assetItem, script, ao);
            return ao;
        }

        private async void SaveScriptAsync(ProjectItem projectItem, RuntimeTextAsset script, ProjectAsyncOperation ao)
        {
            try
            {
                await m_project.Safe.SaveAsync(new[] { projectItem }, new[] { script });
            }
            catch (Exception e)
            {
                ao.Error = new Error(Error.E_Failed);
                Debug.LogException(e);
            }
            finally
            {
                ao.Error = Error.NoError;
                ao.IsCompleted = true;
            }
        }

        private void RaiseCompiling()
        {
            if (Compiling != null)
            {
                Compiling();
            }

        }

        private void RaiseCompiled(bool completed)
        {
            if (Complied != null)
            {
                Complied(completed);
            }
        }

        [Obsolete]
        public ProjectAsyncOperation Compile()
        {
            ProjectAsyncOperation ao = new ProjectAsyncOperation();
            _ = CompileAsync(ao);
            return ao;
        }

        private async Task CompileAsync(ProjectAsyncOperation ao)
        {
            RaiseCompiling();

            ProjectItem[] assetItems = m_project.Utils.FindAssetItems(null, true, typeof(RuntimeTextAsset)).Where(assetItem => assetItem.Ext == Ext).ToArray();
            try
            {
                UnityObject[] loadedObjects = await m_project.LoadAsync(assetItems);
                await RunCompilerAsync(loadedObjects.OfType<RuntimeTextAsset>().Select(s => s.Text).ToArray());
                if (ao != null)
                {
                    ao.Error = Error.NoError;
                }
            }
            catch (Exception e)
            {
                if (ao != null)
                {
                    ao.Error = new Error(Error.E_Failed);
                    Debug.LogException(e);
                    RaiseCompiled(false);
                }
                else
                {
                    RaiseCompiled(false);
                    throw;
                }
            }
            finally
            {
                if (ao != null)
                {
                    ao.IsCompleted = true;
                }
            }
        }

#pragma warning disable CS1998
        public async Task RunCompilerAsync(string[] scripts)
        {
            byte[] binData = null;
#if UNITY_STANDALONE || UNITY_ANDROID
            binData = await Task.Run(() => m_compiler.Compile(scripts));
#endif
            if (binData == null)
            {
                RaiseCompiled(false);
            }
            else
            {
                await SaveAssemblyAsync(binData);
            }

        }
#pragma warning restore CS1998

        private async Task SaveAssemblyAsync(byte[] binData)
        {
            RuntimeBinaryAsset asmBinaryData = ScriptableObject.CreateInstance<RuntimeBinaryAsset>();
            asmBinaryData.Data = binData;

            await m_project.SetValueAsync(RuntimeAssemblyKey, asmBinaryData);

            LoadAssembly(binData);

            RuntimeTypeGuids guids = new RuntimeTypeGuids
            {
                Guids = m_typeNameToGuid.Select(kvp => new RuntimeTypeGuid { FullName = kvp.Key, Guid = kvp.Value.ToString() }).ToArray()
            };

            m_runtimeTypeGuidsAsset.Text = XmlUtility.ToXml(guids);
            await m_project.SetValueAsync(RuntimeTypeGuids, m_runtimeTypeGuidsAsset);
            RaiseCompiled(true);
        }

        private void LoadAssembly(byte[] binData)
        {
            Dictionary<string, List<UnityObject>> typeToDestroyedObjects = UnloadTypes(true);

            Dictionary<string, Guid> typeNameToGuidNew = new Dictionary<string, Guid>();
            m_runtimeAssembly = Assembly.Load(binData);
            Type[] loadedTypes = m_runtimeAssembly.GetTypes().Where(t => typeof(MonoBehaviour).IsAssignableFrom(typeof(MonoBehaviour))).ToArray();
            foreach (Type type in loadedTypes)
            {
                Guid guid;
                string typeName = type.FullName;
                if (!m_typeNameToGuid.TryGetValue(typeName, out guid))
                {
                    guid = Guid.NewGuid();
                    m_typeNameToGuid.Add(typeName, guid);
                }

                if (m_editorsMap != null)
                {
                    m_editorsMap.AddMapping(type, typeof(RuntimeScriptEditor), true, false);
                }

                m_typeMap.RegisterRuntimeSerializableType(type, guid);
                typeNameToGuidNew.Add(typeName, guid);
            }

            m_typeNameToGuid = typeNameToGuidNew;
            EraseDestroyedObjects(typeToDestroyedObjects);
        }

        private void EraseDestroyedObjects(Dictionary<string, List<UnityObject>> typeToDestroyedObjects)
        {
            foreach (KeyValuePair<string, List<UnityObject>> kvp in typeToDestroyedObjects)
            {
                IRTE editor = IOC.Resolve<IRTE>();
                List<UnityObject> destroyedObjects = kvp.Value;
                Type type = m_runtimeAssembly.GetType(kvp.Key);
                if (!m_typeNameToGuid.ContainsKey(kvp.Key))
                {
                    for (int i = 0; i < destroyedObjects.Count; ++i)
                    {
                        editor.Undo.Erase(destroyedObjects[i], null);
                    }
                }
                else
                {
                    for (int i = 0; i < destroyedObjects.Count; ++i)
                    {
                        UnityObject destroyedObject = destroyedObjects[i];
                        UnityObject obj = null;
                        if (destroyedObject is Component)
                        {
                            obj = ((Component)destroyedObject).gameObject.AddComponent(type);
                        }
                        else if (destroyedObject is ScriptableObject)
                        {
                            obj = ScriptableObject.CreateInstance(type);
                        }
                        editor.Undo.Erase(destroyedObject, obj);
                    }
                }
            }
        }

        private Dictionary<string, List<UnityObject>> UnloadTypes(bool destroyObjects)
        {
            Dictionary<string, List<UnityObject>> typeToDestroyedObjects = new Dictionary<string, List<UnityObject>>();
            if (m_runtimeAssembly != null)
            {
                Type[] unloadedTypes = m_runtimeAssembly.GetTypes().Where(t => typeof(MonoBehaviour).IsAssignableFrom(typeof(MonoBehaviour))).ToArray();
                foreach (Type type in unloadedTypes)
                {
                    if (destroyObjects)
                    {
                        List<UnityObject> destroyedObjects = new List<UnityObject>();
                        UnityObject[] objectsOfType = Resources.FindObjectsOfTypeAll(type);
                        foreach (UnityObject obj in objectsOfType)
                        {
                            Destroy(obj);
                            destroyedObjects.Add(obj);
                            //m_editor.Undo.Erase(obj, null);
                        }
                        typeToDestroyedObjects.Add(type.FullName, destroyedObjects);
                    }

                    if (m_editorsMap != null)
                    {
                        m_editorsMap.RemoveMapping(type);
                    }

                    m_typeMap.UnregisterRuntimeSerialzableType(type);
                }
            }

            return typeToDestroyedObjects;
        }
    }
}

