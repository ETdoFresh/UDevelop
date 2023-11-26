using System.Linq;
using System.Reflection;
using Jint;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "js", AllowCaching = true)]
public sealed class JsImporter : ScriptedImporter
{
    private const string JavaScriptExtension = "js";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        if (ctx.mainObject is JsScriptableObject jsScriptableObject)
        {
            var assetPath = ctx.assetPath;
            var assetText = System.IO.File.ReadAllText(assetPath);
            var javascriptField = jsScriptableObject.GetType()
                .GetField("javascript", BindingFlags.NonPublic | BindingFlags.Instance);
            javascriptField.SetValue(jsScriptableObject, assetText);
        }
        else
        {
            var asset = ScriptableObject.CreateInstance<JsScriptableObject>();
            var assetPath = ctx.assetPath;
            var assetText = System.IO.File.ReadAllText(assetPath);
            var javascriptField =
                asset.GetType().GetField("javascript", BindingFlags.NonPublic | BindingFlags.Instance);
            javascriptField.SetValue(asset, assetText);
            ctx.AddObjectToAsset("JavaScript", asset);
            ctx.SetMainObject(asset);
            TryIncludeJavaScriptExtension();
        }
    }

    private static void TryIncludeJavaScriptExtension()
    {
        if (EditorSettings.projectGenerationUserExtensions.Contains(JavaScriptExtension)) return;
        var list = EditorSettings.projectGenerationUserExtensions.ToList();
        list.Add(JavaScriptExtension);
        EditorSettings.projectGenerationUserExtensions = list.ToArray();
    }
}