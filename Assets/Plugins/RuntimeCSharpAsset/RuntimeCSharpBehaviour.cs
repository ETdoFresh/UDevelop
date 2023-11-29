using System;
using System.Linq;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using UnityEngine;

public class RuntimeCSharpBehaviour : MonoBehaviour
{
    [SerializeField] private RuntimeCSharpAsset runtimeCSharp;
    [SerializeField] private AssemblyReferenceAsset[] assemblyReferences;
    private ScriptDomain domain = null;
    private string activeCSharpSource;
    private RuntimeBehaviour runtimeBehaviour;

    [ContextMenu("Hot Reload")]
    private void HotReload()
    {
        if (domain == null)
        {
            domain = ScriptDomain.CreateDomain("CSharpRuntimeCode", true);
            foreach (var reference in assemblyReferences)
                domain.RoslynCompilerService.ReferenceAssemblies.Add(reference);
        }
        var cSharpSource = runtimeCSharp.sourceCode;
        if (activeCSharpSource == cSharpSource) return;
        const ScriptSecurityMode scriptSecurityMode = ScriptSecurityMode.UseSettings;
        var metadataReferenceProviders = assemblyReferences.Cast<IMetadataReferenceProvider>().ToArray();
        var type = domain.CompileAndLoadMainSource(cSharpSource, scriptSecurityMode, metadataReferenceProviders);
        if (type == null)
        {
            if (domain.RoslynCompilerService.LastCompileResult.Success == false)
                throw new Exception("Maze crawler code contained errors. Please fix and try again");

            if (domain.SecurityResult.IsSecurityVerified == false)
                throw new Exception("Maze crawler code failed code security verification");

            throw new Exception(
                "Maze crawler code does not define a class. You must include one class definition of any name that inherits from 'RoslynCSharp.Example.MazeCrawler'");
        }

        Debug.Log($"Hot Reload Type: {type} {type.FullName}");
        activeCSharpSource = cSharpSource;
        var systemType = type.SystemType;
        // Call RuntimeBehaviour.Create<T>(RuntimeCSharpBehaviour self, Transform transform, GameObject gameObject);
        runtimeBehaviour = typeof(RuntimeBehaviour)
            .GetMethod("Create")?
            .MakeGenericMethod(systemType)
            .Invoke(null, new object[] {this, transform, gameObject}) as RuntimeBehaviour;
    }

    [ContextMenu("Hot Restart")]
    private void HotRestart()
    {
        domain = null;
        activeCSharpSource = null;
        HotReload();
    }

    private void Awake()
    {
        HotReload();
        runtimeBehaviour.Awake();
        runtimeCSharp.SourceChanged.AddListener(HotReload);
    }

    private void OnDestroy()
    {
        runtimeCSharp.SourceChanged.RemoveListener(HotReload);
        runtimeBehaviour.OnDestroy();
    }

    private void Start() => runtimeBehaviour.Start();
    private void Update() => runtimeBehaviour.Update();
    private void FixedUpdate() => runtimeBehaviour.FixedUpdate();
    private void LateUpdate() => runtimeBehaviour.LateUpdate();
    private void OnEnable() => runtimeBehaviour.OnEnable();
    private void OnDisable() => runtimeBehaviour.OnDisable();
    private void OnApplicationQuit() => runtimeBehaviour.OnApplicationQuit();
    private void OnApplicationFocus(bool hasFocus) => runtimeBehaviour.OnApplicationFocus(hasFocus);
    private void OnApplicationPause(bool pauseStatus) => runtimeBehaviour.OnApplicationPause(pauseStatus);
    private void OnValidate() => runtimeBehaviour?.OnValidate();
    private void OnCollisionEnter(Collision collision) => runtimeBehaviour.OnCollisionEnter(collision);
    private void OnCollisionStay(Collision collision) => runtimeBehaviour.OnCollisionStay(collision);
    private void OnCollisionExit(Collision collision) => runtimeBehaviour.OnCollisionExit(collision);
    private void OnTriggerEnter(Collider other) => runtimeBehaviour.OnTriggerEnter(other);
    private void OnTriggerStay(Collider other) => runtimeBehaviour.OnTriggerStay(other);
    private void OnTriggerExit(Collider other) => runtimeBehaviour.OnTriggerExit(other);
    private void OnCollisionEnter2D(Collision2D collision) => runtimeBehaviour.OnCollisionEnter2D(collision);
    private void OnCollisionStay2D(Collision2D collision) => runtimeBehaviour.OnCollisionStay2D(collision);
    private void OnCollisionExit2D(Collision2D collision) => runtimeBehaviour.OnCollisionExit2D(collision);
    private void OnTriggerEnter2D(Collider2D collision) => runtimeBehaviour.OnTriggerEnter2D(collision);
    private void OnTriggerStay2D(Collider2D collision) => runtimeBehaviour.OnTriggerStay2D(collision);
    private void OnTriggerExit2D(Collider2D collision) => runtimeBehaviour.OnTriggerExit2D(collision);
}