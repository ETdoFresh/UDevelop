using System.Collections.Generic;
using UnityEngine;

public class MapBehaviour : MonoBehaviour
{
    public TextAsset mapFile;
    public int revision; 
    public MapData mapData;
    public List<string> commands;
}

public class MapData
{
    public List<Object> objects = new();
}