using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using UnityEngine;

public class RuntimeCSharpBehaviour2 : MonoBehaviour
{
    [SerializeField] private RuntimeCSharpAsset runtimeCSharp;
    [SerializeField] private RuntimeBehaviour2 runtimeBehaviour2;
    [SerializeField] private AssemblyReferenceAsset[] assemblyReferences;
    [SerializeField] private bool copySerializedFields = true;
    [SerializeField] private bool copyNonSerializedFields = true;
    [SerializeField] private bool copySerializedProperties;
    [SerializeField] private bool copyNonSerializedProperties;
    private ScriptDomain _domain;
    private string _activeCSharpSource;
    private Dictionary<MemberInfo, object> _originalValues = new();
    
    public RuntimeCSharpAsset RuntimeCSharpAsset => runtimeCSharp;

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
                throw new Exception($"{runtimeCSharp.name} code contained errors. Please fix and try again");

            if (_domain.SecurityResult.IsSecurityVerified == false)
                throw new Exception($"{runtimeCSharp.name} code failed code security verification");

            throw new Exception(
                $"{runtimeCSharp.name} code does not define a class. You must include one class definition of any name that inherits from '{typeof(RuntimeBehaviour2).FullName}'");
        }

        if (type.SystemType.IsSubclassOf(typeof(RuntimeBehaviour2)) == false)
            throw new Exception($"{type.FullName} does not inherit from {typeof(RuntimeBehaviour2).FullName}");

        Debug.Log($"Hot Reload Type: {type} {type.FullName}");
        _activeCSharpSource = cSharpSource;
        runtimeCSharp.runtimeType = type.SystemType;
        
        var members = new List<MemberInfo>();
        var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var fields = type.SystemType.GetFields(bindingFlags);
        var serializableFields = fields.Where(field =>
            copySerializedFields && (field.IsPublic || field.IsDefined(typeof(SerializeField), false)));
        var nonSerializableFields = fields.Where(field =>
            copyNonSerializedFields && (field.IsPublic && field.IsDefined(typeof(NonSerializedAttribute), false) ||
                                        !field.IsPublic && !field.IsDefined(typeof(SerializeField), false)));
        var properties = type.SystemType.GetProperties(bindingFlags);
        var serializableProperties = properties.Where(property =>
            copySerializedProperties && property.GetCustomAttribute(typeof(SerializeField), false) != null);
        var nonSerializableProperties = properties.Where(property =>
            copyNonSerializedProperties && property.GetCustomAttribute(typeof(SerializeField), false) == null);
        members.AddRange(serializableFields);
        members.AddRange(nonSerializableFields);
        members.AddRange(serializableProperties);
        members.AddRange(nonSerializableProperties);
        
        if (!runtimeBehaviour2)
        {
            runtimeBehaviour2 = type.CreateInstance(gameObject).Instance as RuntimeBehaviour2;
            _originalValues.Clear();
            foreach(var member in members)
                _originalValues[member] = member is FieldInfo fieldInfo ? fieldInfo.GetValue(runtimeBehaviour2) : ((PropertyInfo)member).GetValue(runtimeBehaviour2);
            return;
        }
        
        var originalRuntimeBehaviour2 = runtimeBehaviour2;
        runtimeBehaviour2 = type.CreateInstance(gameObject).Instance as RuntimeBehaviour2;
        foreach (var member in members)
        {
            try
            {
                if (member is FieldInfo fieldInfo)
                {
                    var originalFieldInfo = originalRuntimeBehaviour2.GetType().GetField(fieldInfo.Name, bindingFlags);
                    if (originalFieldInfo == null)
                    {
                        Debug.LogWarning($"[{nameof(RuntimeCSharpBehaviour2)}] Could not find field {fieldInfo.Name} in {originalRuntimeBehaviour2}");
                        continue;
                    }
                    var value = originalFieldInfo.GetValue(originalRuntimeBehaviour2);
                    if (_originalValues.TryGetValue(originalFieldInfo, out var originalValue) && Equals(value, originalValue))
                    {
                        Debug.Log($"[{nameof(RuntimeCSharpBehaviour2)}] Skipping {fieldInfo.Name} because it has not changed");
                        continue;
                    }
                    fieldInfo.SetValue(runtimeBehaviour2, value);
                    Debug.Log($"[{nameof(RuntimeCSharpBehaviour2)}] Copied {fieldInfo.Name} from {originalRuntimeBehaviour2} to {runtimeBehaviour2}");
                }
                else if (member is PropertyInfo propertyInfo)
                {
                    var originalPropertyInfo = originalRuntimeBehaviour2.GetType().GetProperty(propertyInfo.Name, bindingFlags);
                    if (originalPropertyInfo == null)
                    {
                        Debug.LogWarning($"[{nameof(RuntimeCSharpBehaviour2)}] Could not find property {propertyInfo.Name} in {originalRuntimeBehaviour2}");
                        continue;
                    }
                    var value = originalPropertyInfo.GetValue(originalRuntimeBehaviour2);
                    if (_originalValues.TryGetValue(originalPropertyInfo, out var originalValue) && Equals(value, originalValue))
                    {
                        Debug.Log($"[{nameof(RuntimeCSharpBehaviour2)}] Skipping {propertyInfo.Name} because it has not changed");
                        continue;
                    }
                    propertyInfo.SetValue(runtimeBehaviour2, value);
                    Debug.Log($"[{nameof(RuntimeCSharpBehaviour2)}] Copied {propertyInfo.Name} from {originalRuntimeBehaviour2} to {runtimeBehaviour2}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(RuntimeCSharpBehaviour2)}] Error copying {member.Name} from {originalRuntimeBehaviour2} to {runtimeBehaviour2}: {e}");
            }
        }
        Destroy(originalRuntimeBehaviour2);
        _originalValues.Clear();
        foreach(var member in members)
            _originalValues[member] = member is FieldInfo fieldInfo ? fieldInfo.GetValue(runtimeBehaviour2) : ((PropertyInfo)member).GetValue(runtimeBehaviour2);
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
        runtimeCSharp.SourceChanged.AddListener(HotReload);
    }

    private void OnDestroy()
    {
        runtimeCSharp.SourceChanged.RemoveListener(HotReload);
    }
}