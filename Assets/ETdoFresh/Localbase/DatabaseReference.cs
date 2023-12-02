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
        public class DatabaseReferenceEntry
        {
            public DatabaseReference databaseReference;
            public LocalbaseDatabase database;
            public string path;
            public object caller;
            public Data<ValueChangedEventArgs> valueChanged;
            public Data<ChildChangedEventArgs> childAdded;
            public Data<ChildChangedEventArgs> childChanged;
            public Data<ChildChangedEventArgs> childRemoved;
            public Data<ChildChangedEventArgs> childMoved;
        }

        public static List<DatabaseReferenceEntry> DatabaseReferenceEntries = new();
        public DatabaseReferenceEntry databaseReferenceEntry;
        
        public Data<ValueChangedEventArgs> ValueChanged => databaseReferenceEntry.valueChanged;
        public Data<ChildChangedEventArgs> ChildAdded => databaseReferenceEntry.childAdded;
        public Data<ChildChangedEventArgs> ChildChanged => databaseReferenceEntry.childChanged;
        public Data<ChildChangedEventArgs> ChildRemoved => databaseReferenceEntry.childRemoved;
        public Data<ChildChangedEventArgs> ChildMoved => databaseReferenceEntry.childMoved;
        public LocalbaseDatabase Database => databaseReferenceEntry.database;
        
        private string Path => databaseReferenceEntry.path;
        private object Caller => databaseReferenceEntry.caller;

        public DatabaseReference Parent => IsRoot() ? null : Create(Database, GetParent(), Caller);

        public DatabaseReference Root => Create(Database, GetRoot(), Caller);

        public static DatabaseReference Create(LocalbaseDatabase database, string path, object caller = null)
        {
            var existingDatabaseReference = DatabaseReferenceEntries
                .FirstOrDefault(entry => entry.database == database && entry.path == path && entry.caller == caller);
            if (existingDatabaseReference != null) return existingDatabaseReference.databaseReference;
            var databaseReference = new DatabaseReference();
            var jToken = database.JObject.SelectToken(path);
            databaseReference.databaseReferenceEntry = new DatabaseReferenceEntry
            {
                database = database,
                path = path,
                caller = caller,
                valueChanged = new Data<ValueChangedEventArgs>(
                    new ValueChangedEventArgs(new DataSnapshot(jToken, databaseReference))),
                childAdded = new Data<ChildChangedEventArgs>(
                    new ChildChangedEventArgs(new DataSnapshot(jToken, databaseReference), null)),
                childChanged = new Data<ChildChangedEventArgs>(
                    new ChildChangedEventArgs(new DataSnapshot(jToken, databaseReference), null)),
                childRemoved = new Data<ChildChangedEventArgs>(
                    new ChildChangedEventArgs(new DataSnapshot(jToken, databaseReference), null)),
                childMoved = new Data<ChildChangedEventArgs>(
                    new ChildChangedEventArgs(new DataSnapshot(jToken, databaseReference), null))
            };
            return databaseReference;
        }

        public void Destroy()
        {
            if (databaseReferenceEntry == null) return;
            DatabaseReferenceEntries.Remove(databaseReferenceEntry);
            databaseReferenceEntry.databaseReference = null;
            databaseReferenceEntry.database = null;
            databaseReferenceEntry.path = null;
            databaseReferenceEntry.caller = null;
            databaseReferenceEntry.valueChanged = null;
            databaseReferenceEntry.childAdded = null;
            databaseReferenceEntry.childChanged = null;
            databaseReferenceEntry.childRemoved = null;
            databaseReferenceEntry.childMoved = null;
            databaseReferenceEntry = null;
        }

        public DatabaseReference Child(string pathString) => Create(Database, $"{Path}.{pathString}", Caller);

        // public DatabaseReference Push() => new(Database, "push");

        public Task SetValueAsync(object value)
        {
            var objectJson = JsonConvert.SerializeObject(value);
            return SetRawJsonValueAsync(objectJson);
        }

        public Task SetRawJsonValueAsync(string jsonValue)
        {
            var databaseJson = Database.Json;
            var jsonValueObject = JToken.Parse(jsonValue);
            var databaseObject = string.IsNullOrEmpty(databaseJson) ? new JObject() : JObject.Parse(databaseJson);
            var pathObject = GetOrCreatePath(databaseObject, Path);
            pathObject.Replace(jsonValueObject);
            Database.Json = databaseObject.ToString(Formatting.Indented);
            ValueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(jsonValueObject, this));
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

        public override string ToString() => base.ToString() + " " + Path;

        public string Key => IsRoot() ? null : Path.Split('.').Last();

        public bool IsRoot() => string.IsNullOrEmpty(Path);

        public string GetParent() => Path[..Path.LastIndexOf('.')];

        public string GetRoot() => Path.Split('.').First();

        public override bool Equals(object other) => other is DatabaseReference && ToString().Equals(other.ToString());

        public override int GetHashCode() => ToString().GetHashCode();
    }
}