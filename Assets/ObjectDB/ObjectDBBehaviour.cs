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
    
    private static List<ObjectEntry> ObjectEntries = new();

    [SerializeField] private List<ObjectEntry> objectEntries;

    private void Awake()
    {
        objectEntries = ObjectEntries;
    }
    
    public static GameObject Get(string name, int index = 0)
    {
        var entries = GetAll(name);
        return entries[index];
    }

    public static List<GameObject> GetAll(string name)
    {
        var entries = ObjectEntries.FindAll(e => string.Equals(e.name, name, StringComparison.CurrentCultureIgnoreCase));
        return entries?.ConvertAll(e => e.instance);
    }

    public static void Remove(string objectName, int index)
    {
        var entries = ObjectEntries.FindAll(e => string.Equals(e.name, objectName, StringComparison.CurrentCultureIgnoreCase));
        var entry = entries[index];
        ObjectEntries.Remove(entry);
    }

    public static int Add(string name, GameObject instance)
    {
        ObjectEntries.Add(new ObjectEntry { name = name, instance = instance });
        return ObjectEntries.Count - 1;
    }
}
