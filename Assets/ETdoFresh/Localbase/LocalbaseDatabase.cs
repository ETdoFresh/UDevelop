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
        private string _path;
        private string _json;
        private JObject _jObject;
        private static string _defaultName = "__DEFAULT__";
        private bool _persistenceEnabled = true;
        private DateTime _lastWriteTime;

        public static LocalbaseDatabase DefaultInstance => GetInstance(_defaultName);
        internal static Dictionary<string, LocalbaseDatabase> Databases => _databases;

        internal DateTime LastWriteTime => _lastWriteTime;
        internal JObject JObject => _jObject;
        internal string Path => _path;
        internal string Json { get => _json; set => SetValue(value); }

        private static string GetPath(string name) =>
            System.IO.Path.Combine(Application.persistentDataPath, "Databases", $"{name}.json");

        public static LocalbaseDatabase GetInstance(string name)
        {
            if (_databases.TryGetValue(name, out var instance)) return instance;
            _databases[name] = new LocalbaseDatabase { _name = name, _path = GetPath(name) };
            _databases[name].UpdateFromFile();
            LocalbaseFileMonitor.Touch();
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
            var directory = System.IO.Path.GetDirectoryName(_path);
            if (directory != null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(_path, value);
            _lastWriteTime = File.GetLastWriteTime(_path);
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

        internal void UpdateFromFile()
        {
            var path = GetPath(_name);
            var fileExists = File.Exists(path);
            var lastWriteTime = fileExists ? File.GetLastWriteTime(path) : DateTime.MinValue;
            if (fileExists && lastWriteTime >= DateTime.Now) return;
            var value = fileExists ? File.ReadAllText(path) : "{}";
            _json = value;
            _jObject = JObject.Parse(value);
            _lastWriteTime = File.GetLastWriteTime(path);
            RootReference.SetValueAsync(value);
        }
    }
}