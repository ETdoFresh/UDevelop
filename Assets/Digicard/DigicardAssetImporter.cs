using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

/// <summary>
/// Import any files with the .digicard extension
/// </summary>
[ScriptedImporter(1, "digicard", AllowCaching = true)]
public sealed class DigicardAssetImporter : ScriptedImporter
{
    private DigicardAsset asset;

    public string assetReferencePath;

    // Exposible Field
    public string cardName;
    public CardType cardType;
    public CardProps[] cardProperties;
    public Sprite cardCoverArt;
    public string cardDescription;

    private const string DigicardExtension = "digicard";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        // Create our DigicardAsset
        asset = ScriptableObject.CreateInstance<DigicardAsset>();
        var assetPath = ctx.assetPath;
        assetReferencePath = assetPath;
        asset.filePath = assetPath;
        asset.Load();

        PopulateImporterFields();

        ctx.AddObjectToAsset(GUID.Generate().ToString(), asset);
        ctx.SetMainObject(asset);

        // If extension not included in our project
        // add it.
        TryIncludeDigicardExtension();
    }

    public void PopulateImporterFields()
    {
        cardName = asset.cardName;
        cardType = asset.cardType;
        cardProperties = asset.cardProperties;
        cardCoverArt = asset.cardCoverArt;
        cardDescription = asset.cardDescription;
    }

    public void Save()
    {
        asset = AssetDatabase.LoadAssetAtPath<DigicardAsset>(assetReferencePath);
        asset.cardName = cardName;
        asset.cardType = cardType;
        asset.cardProperties = cardProperties;
        asset.cardCoverArt = cardCoverArt;
        asset.cardDescription = cardDescription;
        asset.Save();
        PopulateImporterFields();
    }

    // You can add this if you want to prevent from 
    // manually including your new extension in the
    // Player Settings
    private static void TryIncludeDigicardExtension()
    {
        if (EditorSettings.projectGenerationUserExtensions.Contains(DigicardExtension)) return;
        var list = EditorSettings.projectGenerationUserExtensions.ToList();
        list.Add(DigicardExtension);
        EditorSettings.projectGenerationUserExtensions = list.ToArray();
    }
}
