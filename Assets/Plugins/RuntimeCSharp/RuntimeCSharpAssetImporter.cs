#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RuntimeCSharp
{
    /// <summary>
    /// Import any files with the runtime.cs extension
    /// </summary>
    [ScriptedImporter(1, "runtime.cs", AllowCaching = true)]
    public sealed class RuntimeCSharpAssetImporter : ScriptedImporter
    {
        private const string JavaScriptExtension = "runtime.cs";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var assetPath = ctx.assetPath;
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            if (ctx.mainObject is RuntimeCSharpAsset cSharpScriptableObject)
            {
                cSharpScriptableObject.filePath = assetPath;
                cSharpScriptableObject.sourceCode = System.IO.File.ReadAllText(assetPath);
            }
            else
            {
                cSharpScriptableObject = ScriptableObject.CreateInstance<RuntimeCSharpAsset>();
                cSharpScriptableObject.filePath = assetPath;
                cSharpScriptableObject.sourceCode = System.IO.File.ReadAllText(assetPath);
                ctx.AddObjectToAsset("CSharp", cSharpScriptableObject);
                ctx.SetMainObject(cSharpScriptableObject);
                TryIncludeExtensionToSolution();
            }
        }

        private static void TryIncludeExtensionToSolution()
        {
            if (EditorSettings.projectGenerationUserExtensions.Contains(JavaScriptExtension)) return;
            var list = EditorSettings.projectGenerationUserExtensions.ToList();
            list.Add(JavaScriptExtension);
            EditorSettings.projectGenerationUserExtensions = list.ToArray();
        }
    }
}
#endif