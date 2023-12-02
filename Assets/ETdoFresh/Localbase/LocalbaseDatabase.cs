using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ETdoFresh.Localbase
{
    [Serializable]
    public class LocalbaseDatabase
    {
        private static Dictionary<string, LocalbaseDatabase> _databases = new();
        private string _name;
        private string _json;
        private JObject _jObject;
        private static string _defaultName = "__DEFAULT__";
        private bool _persistenceEnabled = true;

        public static LocalbaseDatabase DefaultInstance => GetInstance(_defaultName);
        internal string Json { get => _json; set => SetValue(value); }
        internal JObject JObject => _jObject;

        public static LocalbaseDatabase GetInstance(string name)
        {
            if (!_databases.TryAdd(name, new LocalbaseDatabase { _name = name })) return _databases[name];
            var path = Path.Combine(Application.persistentDataPath, "Databases", $"{name}.json");
            _databases[name].Json = File.Exists(path) ? File.ReadAllText(path) : "";
            return _databases[name];
        }

        public DatabaseReference RootReference => GetReference();

        public DatabaseReference GetReference(string pathString = "", object caller = null) => 
            DatabaseReference.Create(this, pathString, caller);

        public void SetPersistenceEnabled(bool enabled)
        {
            _persistenceEnabled = enabled;
        }

        private void SetValue(string value)
        {
            _json = string.IsNullOrEmpty(value) ? "{}" : value;
            _jObject = JObject.Parse(_json);
            if (!_persistenceEnabled) return;
            var path = Path.Combine(Application.persistentDataPath, "Databases", $"{_name}.json");
            var directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, value);
        }

        public string FormatPath(string path)
        {
            // Change slash to dot delimiter
            path = path.Replace("/", ".");
            if (_jObject == null) return path;
            
            // Discover array vs object paths
            var pathParts = path.Split('.');
            var currentPath = "";
            foreach (var pathPart in pathParts)
            {
                if (string.IsNullOrEmpty(pathPart)) continue;
                
                if (int.TryParse(pathPart, out var index))
                {
                    var testPath = $"{currentPath}[{index}]";
                    var jToken = _jObject.SelectToken(testPath);
                    if (jToken != null)
                    {
                        currentPath = testPath;
                        continue;
                    }
                }
                
                currentPath = $"{currentPath}.{pathPart}";
            }

            return currentPath;
        }
    }
}