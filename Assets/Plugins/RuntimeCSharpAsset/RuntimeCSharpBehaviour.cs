using System;
using System.Linq;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using UnityEngine;

public class RuntimeCSharpBehaviour : MonoBehaviour
{
    [SerializeField] private RuntimeCSharpAsset runtimeCSharp;
    [SerializeField] private AssemblyReferenceAsset[] assemblyReferences;
    private RuntimeBehaviour _runtimeBehaviour;
    private ScriptDomain _domain;
    private string _activeCSharpSource;

    [ContextMenu("Hot Reload")]
    private void HotReload()
    {
        if (_domain == null)
        {
            _domain = ScriptDomain.CreateDomain("CSharpRuntimeCode", true);
            foreach (var reference in assemblyReferences)
                _domain.RoslynCompilerService.ReferenceAssemblies.Add(reference);
        }
        var cSharpSource = runtimeCSharp.sourceCode;
        if (_activeCSharpSource == cSharpSource) return;
        const ScriptSecurityMode scriptSecurityMode = ScriptSecurityMode.UseSettings;
        var metadataReferenceProviders = assemblyReferences.Cast<IMetadataReferenceProvider>().ToArray();
        var type = _domain.CompileAndLoadMainSource(cSharpSource, scriptSecurityMode, metadataReferenceProviders);
        if (type == null)
        {
            if (_domain.RoslynCompilerService.LastCompileResult.Success == false)
                throw new Exception("Maze crawler code contained errors. Please fix and try again");

            if (_domain.SecurityResult.IsSecurityVerified == false)
                throw new Exception("Maze crawler code failed code security verification");

            throw new Exception(
                "Maze crawler code does not define a class. You must include one class definition of any name that inherits from 'RoslynCSharp.Example.MazeCrawler'");
        }

        Debug.Log($"Hot Reload Type: {type} {type.FullName}");
        _activeCSharpSource = cSharpSource;
        var systemType = type.SystemType;
        // Call RuntimeBehaviour.Create<T>(RuntimeCSharpBehaviour self, Transform transform, GameObject gameObject);
        _runtimeBehaviour = typeof(RuntimeBehaviour)
            .GetMethod("Create")?
            .MakeGenericMethod(systemType)
            .Invoke(null, new object[] {this, transform, gameObject}) as RuntimeBehaviour;
    }

    [ContextMenu("Hot Restart")]
    private void HotRestart()
    {
        _domain = null;
        _activeCSharpSource = null;
        HotReload();
    }

    private void Awake()
    {
        HotReload();
        _runtimeBehaviour.Awake();
        runtimeCSharp.SourceChanged.AddListener(HotReload);
    }

    private void OnDestroy()
    {
        runtimeCSharp.SourceChanged.RemoveListener(HotReload);
        _runtimeBehaviour.OnDestroy();
    }

    private void Start() => _runtimeBehaviour.Start();
    private void Update() => _runtimeBehaviour.Update();
    private void FixedUpdate() => _runtimeBehaviour.FixedUpdate();
    private void LateUpdate() => _runtimeBehaviour.LateUpdate();
    private void OnEnable() => _runtimeBehaviour.OnEnable();
    private void OnDisable() => _runtimeBehaviour.OnDisable();
    private void OnApplicationQuit() => _runtimeBehaviour.OnApplicationQuit();
    private void OnApplicationFocus(bool hasFocus) => _runtimeBehaviour.OnApplicationFocus(hasFocus);
    private void OnApplicationPause(bool pauseStatus) => _runtimeBehaviour.OnApplicationPause(pauseStatus);
    private void OnValidate() => _runtimeBehaviour?.OnValidate();
    private void OnCollisionEnter(Collision collision) => _runtimeBehaviour.OnCollisionEnter(collision);
    private void OnCollisionStay(Collision collision) => _runtimeBehaviour.OnCollisionStay(collision);
    private void OnCollisionExit(Collision collision) => _runtimeBehaviour.OnCollisionExit(collision);
    private void OnTriggerEnter(Collider other) => _runtimeBehaviour.OnTriggerEnter(other);
    private void OnTriggerStay(Collider other) => _runtimeBehaviour.OnTriggerStay(other);
    private void OnTriggerExit(Collider other) => _runtimeBehaviour.OnTriggerExit(other);
    private void OnCollisionEnter2D(Collision2D collision) => _runtimeBehaviour.OnCollisionEnter2D(collision);
    private void OnCollisionStay2D(Collision2D collision) => _runtimeBehaviour.OnCollisionStay2D(collision);
    private void OnCollisionExit2D(Collision2D collision) => _runtimeBehaviour.OnCollisionExit2D(collision);
    private void OnTriggerEnter2D(Collider2D collision) => _runtimeBehaviour.OnTriggerEnter2D(collision);
    private void OnTriggerStay2D(Collider2D collision) => _runtimeBehaviour.OnTriggerStay2D(collision);
    private void OnTriggerExit2D(Collider2D collision) => _runtimeBehaviour.OnTriggerExit2D(collision);
}