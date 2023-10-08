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

    public static void Remove(string objectName, int index)
    {
        var entries = _instance.objectEntries.FindAll(e => string.Equals(e.name, objectName, StringComparison.CurrentCultureIgnoreCase));
        var entry = entries[index];
        _instance.objectEntries.Remove(entry);
    }

    public static int Add(string name, GameObject instance)
    {
        _instance.objectEntries.Add(new ObjectEntry { name = name, instance = instance });
        return _instance.objectEntries.Count - 1;
    }
}
