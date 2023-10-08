using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDBBehaviour : MonoBehaviour
{
    [Serializable]
    public class ObjectEntry
    {
        public string name;
        public GameObject instance;
    }
    
    private static ObjectDBBehaviour _instance;
    
    [SerializeField] private List<ObjectEntry> objectEntries = new();

    private void Awake()
    {
        _instance = this;
    }
    
    public static GameObject Get(string name, int index = 0)
    {
        var entries = GetAll(name);
        return entries[index];
    }

    public static List<GameObject> GetAll(string name)
    {
        var entries = _instance.objectEntries.FindAll(e => string.Equals(e.name, name, StringComparison.CurrentCultureIgnoreCase));
        return entries?.ConvertAll(e => e.instance);
    }

    public static GameObject GetFirst(string name)
    {
        var entries = GetAll(name);
        return entries[0];
    }
    
    public static GameObject GetLast(string name)
    {
        var entries = GetAll(name);
        return entries[^1];
    }

    public static void RemoveLast(string name)
    {
        var lastEntry = _instance.objectEntries.FindLast(e => string.Equals(e.name, name, StringComparison.CurrentCultureIgnoreCase));
        if (lastEntry == null) return;
        _instance.objectEntries.Remove(lastEntry);
        Destroy(lastEntry.instance);
    }

    public static void Add(string name, GameObject instance)
    {
        _instance.objectEntries.Add(new ObjectEntry { name = name, instance = instance });
    }
}
