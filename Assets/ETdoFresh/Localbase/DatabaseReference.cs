using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ETdoFresh.Localbase
{
    [Serializable]
    public class DatabaseReference : Query
    {
        private static Dictionary<LocalbaseDatabase, Dictionary<string, DatabaseReference>> _references = new();
        private static Dictionary<string, Data<ValueChangedEventArgs>> _valueChangedEvents = new();
        private static Dictionary<string, Data<ChildChangedEventArgs>> _childAddedEvents = new();
        private static Dictionary<string, Data<ChildChangedEventArgs>> _childChangedEvents = new();
        private static Dictionary<string, Data<ChildChangedEventArgs>> _childRemovedEvents = new();
        private static Dictionary<string, Data<ChildChangedEventArgs>> _childMovedEvents = new();
        private string _path;
        
        public Data<ValueChangedEventArgs> ValueChanged => _valueChangedEvents[_path];
        public Data<ChildChangedEventArgs> ChildAdded => _childAddedEvents[_path];
        public Data<ChildChangedEventArgs> ChildChanged => _childChangedEvents[_path];
        public Data<ChildChangedEventArgs> ChildRemoved => _childRemovedEvents[_path];
        public Data<ChildChangedEventArgs> ChildMoved => _childMovedEvents[_path];

        internal DatabaseReference(LocalbaseDatabase database, string path)
        {
            Database = database;
            _path = database.FormatPath(path);
            _references.TryAdd(database, new Dictionary<string, DatabaseReference>());
            var databaseJObject = database.JObject;
            var jToken = databaseJObject.SelectToken(_path);
            _valueChangedEvents.TryAdd(_path, new Data<ValueChangedEventArgs>(new ValueChangedEventArgs(new DataSnapshot(jToken, this))));
            _childAddedEvents.TryAdd(_path, new Data<ChildChangedEventArgs>(new ChildChangedEventArgs(new DataSnapshot(jToken, this), null)));
            _childChangedEvents.TryAdd(_path, new Data<ChildChangedEventArgs>(new ChildChangedEventArgs(new DataSnapshot(jToken, this), null)));
            _childRemovedEvents.TryAdd(_path, new Data<ChildChangedEventArgs>(new ChildChangedEventArgs(new DataSnapshot(jToken, this), null)));
            _childMovedEvents.TryAdd(_path, new Data<ChildChangedEventArgs>(new ChildChangedEventArgs(new DataSnapshot(jToken, this), null)));
            _references[database].TryAdd(_path, this);
        }

        public LocalbaseDatabase Database { get; private set; }

        public DatabaseReference Parent => IsRoot() ? null : new DatabaseReference(Database, GetParent());

        public DatabaseReference Root => new(Database, GetRoot());

        public DatabaseReference Child(string pathString) => new(Database, pathString);

        // public DatabaseReference Push() => new(Database, "push");

        public Task SetValueAsync(object value)
        {
            var objectJson = JsonConvert.SerializeObject(value);
            return SetRawJsonValueAsync(objectJson);
        }

        public Task SetRawJsonValueAsync(string jsonValue)
        {
            var path = _path;
            var databaseJson = Database.Json;
            var jsonValueObject = JToken.Parse(jsonValue);
            var databaseObject = string.IsNullOrEmpty(databaseJson) ? new JObject() : JObject.Parse(databaseJson);
            var pathObject = GetOrCreatePath(databaseObject, path); 
            pathObject.Replace(jsonValueObject);
            Database.Json = databaseObject.ToString(Formatting.Indented);
            _valueChangedEvents[path].Value = new ValueChangedEventArgs(new DataSnapshot(jsonValueObject, this));
            return Task.CompletedTask;
        }

        private JToken GetOrCreatePath(JObject databaseObject, string path)
        {
            if (string.IsNullOrEmpty(path)) return databaseObject;
            var jToken = databaseObject.SelectToken(path);
            if (jToken != null) return jToken;
            
            var pathParts = path.Split('.');
            var parentPath = string.Join('.', pathParts.Take(pathParts.Length - 1));
            var parentObject = GetOrCreatePath(databaseObject, parentPath);
            var childName = pathParts.Last();
            var childObject = new JObject();
            if (parentObject is JObject parentJObject)
            {
                if (parentJObject.Count == 0 && int.TryParse(childName, out var index))
                {
                    var parentArray = new JArray();
                    for (var i = 0; i <= index; i++) parentArray.Add(null);
                    parentArray[index] = childObject;
                    parentObject.Replace(parentArray);
                }
                else
                {
                    parentJObject.Add(childName, childObject);
                }
            }
            else if (parentObject is JArray parentJArray)
            {
                if (int.TryParse(childName, out var index))
                {
                    for (var i = parentJArray.Count; i <= index; i++) parentJArray.Add(null);
                    parentJArray[index] = childObject;
                }
                else
                {
                    parentJObject = new JObject();
                    for (var i = 0; i < parentJArray.Count; i++)
                    {
                        parentJObject.Add(i.ToString(), parentJArray[i]);
                    }
                    parentJObject.Add(childName, childObject);
                    parentObject.Replace(parentJObject);
                }
            }
            else
            {
                parentJObject = new JObject { { childName, childObject } };
                parentObject.Replace(parentJObject);
            }
            return childObject;
        }

        // public Task SetValueAsync(object value, object priority)

        // public Task SetRawJsonValueAsync(string jsonValue, object priority)

        // public Task SetPriorityAsync(object priority)

        public Task UpdateChildrenAsync(IDictionary<string, object> update)
        {
            return Task.WhenAll(update
                .Select(kvp => new { item = kvp, child = Child(kvp.Key) })
                .Select(tuple => tuple.child.SetValueAsync(tuple.item.Value))
                .ToList());
        }

        public Task RemoveValueAsync()
        {
            // TODO: Remove json at path
            return Task.CompletedTask;
        }

        public override string ToString() => base.ToString() + " " + _path;

        public string Key => IsRoot() ? null : _path.Split('.').Last();

        public bool IsRoot() => string.IsNullOrEmpty(_path);

        public string GetParent() => _path[.._path.LastIndexOf('.')];

        public string GetRoot() => _path.Split('.').First();

        public override bool Equals(object other) => other is DatabaseReference && ToString().Equals(other.ToString());

        public override int GetHashCode() => ToString().GetHashCode();
    }
}