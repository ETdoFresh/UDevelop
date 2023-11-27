#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using Jint;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "u.json", AllowCaching = true)]
public sealed class JsonImporter : ScriptedImporter
{
    private const string UJsonExtension = "u.json";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        if (ctx.mainObject is JsonScriptableObject jsonScriptableObject)
        {
            var assetPath = ctx.assetPath;
            var assetText = System.IO.File.ReadAllText(assetPath);
            var jsonValueField = jsonScriptableObject.GetType()
                .GetField("jsonValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var jsonValue = jsonValueField.GetValue(jsonScriptableObject) as JsonValue;
            jsonValue.jsonString = assetText;
        }
        else
        {
            var asset = ScriptableObject.CreateInstance<JsonScriptableObject>();
            var assetPath = ctx.assetPath;
            var assetText = System.IO.File.ReadAllText(assetPath);
            asset.jsonValue.jsonString = assetText;
            ctx.AddObjectToAsset("Json", asset);
            ctx.SetMainObject(asset);
        }

        TryIncludeUJsonExtension();
    }

    private static void TryIncludeUJsonExtension()
    {
        if (EditorSettings.projectGenerationUserExtensions.Contains(UJsonExtension)) return;
        var list = EditorSettings.projectGenerationUserExtensions.ToList();
        list.Add(UJsonExtension);
        EditorSettings.projectGenerationUserExtensions = list.ToArray();
    }
}
#endif