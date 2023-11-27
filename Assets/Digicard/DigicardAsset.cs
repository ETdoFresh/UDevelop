using System;
using System.IO;
using UnityEngine;

public sealed class DigicardAsset : ScriptableObject
{
    private int m_instanceID;

    public int InstanceID
    {
        get
        {
            if (m_instanceID == 0)
                m_instanceID = GetInstanceID();
            return m_instanceID;
        }
    }

    private int m_hashCode;
    public int HashCode => InstanceID.GetHashCode();

    // Imagine we have field and properties representing our file type
    [HideInInspector] public string filePath;

    // Exposible Field
    [HideInInspector] public string cardName;
    [HideInInspector] public CardType cardType;
    [HideInInspector] public CardProps[] cardProperties;
    [HideInInspector] public Sprite cardCoverArt;
    [HideInInspector] public string cardDescription;

    private DigicardData data = new();

    private void OnEnable()
    {
        cardName = name;
    }

    public override int GetHashCode()
    {
        return HashCode + data.GetHashCode();
    }

    /// <summary>
    /// Save Digicard Data
    /// </summary>
    public void Save()
    {
        GenerateNewCard();

        var jsonString = JsonUtility.ToJson(data, true);

        if (filePath == null)
        {
            Debug.LogError("Failed to save Digicard");
            return;
        }

        using StreamWriter streamWriter = new(filePath);
        streamWriter.Write(jsonString);
    }

    private void GenerateNewCard()
    {
        // Only run this if our data
        // object is void. This prevents
        // unnecessary memory allocation.
        data ??= new DigicardData()
        {
            name = cardName,
            type = cardType,
            properties = cardProperties,
            description = cardDescription,
#if UNITY_EDITOR
            coverArtPath = UnityEditor.AssetDatabase.GetAssetPath(cardCoverArt)
#endif
        };

        // We then reuse our data, and change the field
        // accordingly.
        data.name = cardName;
        data.type = cardType;
        data.properties = cardProperties;
        data.description = cardDescription;
#if UNITY_EDITOR
        data.coverArtPath = UnityEditor.AssetDatabase.GetAssetPath(cardCoverArt);
#endif
    }

    public void Load()
    {
        using StreamReader streamReader = new StreamReader(filePath);
        var jsonString = streamReader.ReadToEnd();
        data = JsonUtility.FromJson<DigicardData>(jsonString);

        cardName = data.name;
        cardType = data.type;
        cardProperties = data.properties;
        cardDescription = data.description;

        if (data.coverArtPath == string.Empty) return;
        RequestingImage(data.coverArtPath);
    }

    private bool RequestingImage(string path)
    {
        Sprite image = ConvertTextureToSprite(LoadTexture(path), 100f, SpriteMeshType.Tight);

        cardCoverArt = image;

        return true;
    }

    private Sprite ConvertTextureToSprite(Texture2D _texture, float _pixelPerUnit = 100.0f,
        SpriteMeshType _spriteType = SpriteMeshType.Tight)
    {
        Sprite newSprite = null;
        //Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference
        newSprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0, 0),
            _pixelPerUnit, 0, _spriteType);

        return newSprite;
    }

    private Texture2D LoadTexture(string _filePath)
    {
        /*We'll load a png or jpg file from disk to a Texture2D
         If we fail at doing so, return null.*/

        Texture2D tex2D;

        //We want to read the binary data of this file,
        //in order to know the formatting. With the formatting,
        //we'll use the data to create the image that we requested
        byte[] fileData;

        if (File.Exists(_filePath))
        {
            fileData = File.ReadAllBytes(_filePath);
            tex2D = new Texture2D(2, 2);
            if (tex2D.LoadImage(fileData))
                return tex2D;
        }

        return null;
    }
}

public enum CardType { Normal, Spell, Summons }

[Serializable]
public sealed class DigicardData
{
    public string name;
    public CardType type;
    public CardProps[] properties;
    public string description;
    public string coverArtPath;
}

[Serializable]
public sealed class CardProps
{
    public string name;
    public int value;
    public string description;
}