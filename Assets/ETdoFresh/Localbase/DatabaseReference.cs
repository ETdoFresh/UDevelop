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
        private JToken MyJToken => Database?.JObject?.SelectToken(Path);

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
                databaseReference = databaseReference,
                database = database,
                path = path,
                caller = caller,
                valueChanged = new Data<ValueChangedEventArgs>(
                    new ValueChangedEventArgs(new DataSnapshot(jToken, databaseReference))),
                childAdded = new Data<ChildChangedEventArgs>(null),
                childChanged = new Data<ChildChangedEventArgs>(null),
                childRemoved = new Data<ChildChangedEventArgs>(null),
                childMoved = new Data<ChildChangedEventArgs>(null)
            };
            DatabaseReferenceEntries.Add(databaseReference.databaseReferenceEntry);
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
        
        public bool HasChild(string pathString) => Child(pathString).MyJToken != null;

        public DatabaseReference Child(string pathString) => Create(Database, $"{Path}.{pathString}", Caller);

        // public DatabaseReference Push() => new(Database, "push");

        public Task SetValueAsync(object value)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var objectJson = JsonConvert.SerializeObject(value, settings);
            return SetRawJsonValueAsync(objectJson);
        }

        public Task SetRawJsonValueAsync(string jsonValue)
        {
            var databaseJson = Database.Json;
            var jsonValueObject = JToken.Parse(jsonValue);
            var databaseObject = string.IsNullOrEmpty(databaseJson) ? new JObject() : JObject.Parse(databaseJson);
            var pathObject = GetOrCreatePath(databaseObject, Path);

            if (IsRoot() && jsonValueObject is JObject jObject)
                databaseObject = jObject;
            else if (!IsRoot())
                pathObject.Replace(jsonValueObject);

            Database.Json = databaseObject.ToString(Formatting.Indented);
            var snapshot = new DataSnapshot(jsonValueObject, this);
            ValueChanged.Value = new ValueChangedEventArgs(snapshot);
            InvokeParentChildChangedEvents(Parent, new ChildChangedEventArgs(snapshot, null));
            
            if (IsRoot())
                InvokeValueChangeOnAllReferences(databaseObject);

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

        public string GetParent() => Path.Contains('.') ? Path[..Path.LastIndexOf('.')] : "";

        public string GetRoot() => "";

        public override bool Equals(object other) => other is DatabaseReference && ToString().Equals(other.ToString());

        public override int GetHashCode() => ToString().GetHashCode();

        private bool IsChildOf(DatabaseReference other)
        {
            if (other == null) return false;
            if (other.IsRoot()) return true;
            if (IsRoot()) return false;
            return Path.StartsWith(other.Path);
        }

        public void AddArrayChild(JToken childJToken)
        {
            var databaseJObject = Database.JObject;
            if (MyJToken is not JArray myJArray)
                throw new Exception(
                    $"[{nameof(DatabaseReference)}] {nameof(AddArrayChild)} {nameof(MyJToken)} is not a JArray");

            myJArray.Add(childJToken);
            Database.JObject = databaseJObject;
            ChildAdded.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            InvokeParentChildChangedEvents(Parent, ChildAdded.Value);
        }

        public void AddArrayChild(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            var json = JsonConvert.SerializeObject(obj, settings);
            var jToken = JToken.Parse(json);
            AddArrayChild(jToken);
        }

        public void AddObjectChild(string key, JToken childJToken)
        {
            var databaseJObject = Database.JObject;
            if (MyJToken is not JObject myJObject)
                throw new Exception(
                    $"[{nameof(DatabaseReference)}] {nameof(AddObjectChild)} {nameof(MyJToken)} is not a JObject");

            myJObject.Add(key, childJToken);
            Database.JObject = databaseJObject;
            ChildAdded.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            InvokeParentChildChangedEvents(Parent, ChildAdded.Value);
        }

        public void AddObjectChild(string key, object obj)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            var json = JsonConvert.SerializeObject(obj, settings);
            var jToken = JToken.Parse(json);
            AddObjectChild(key, jToken);
        }

        public void RemoveArrayChild(int index)
        {
            var databaseJObject = Database.JObject;
            if (MyJToken is not JArray myJArray)
                throw new Exception(
                    $"[{nameof(DatabaseReference)}] {nameof(RemoveArrayChild)} {nameof(MyJToken)} is not a JArray");

            var childJToken = myJArray[index];
            myJArray.RemoveAt(index);
            Database.JObject = databaseJObject;
            ChildRemoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            InvokeParentChildChangedEvents(Parent, ChildRemoved.Value);
        }

        public void RemoveObjectChild(string key)
        {
            var databaseJObject = Database.JObject;
            if (MyJToken is not JObject myJObject)
                throw new Exception(
                    $"[{nameof(DatabaseReference)}] {nameof(RemoveObjectChild)} {nameof(MyJToken)} is not a JObject");

            var childJToken = myJObject[key];
            myJObject.Remove(key);
            Database.JObject = databaseJObject;
            ChildRemoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            InvokeParentChildChangedEvents(Parent, ChildRemoved.Value);
        }

        public void MoveArrayChild(int oldIndex, int newIndex)
        {
            var databaseJObject = Database.JObject;
            if (MyJToken is not JArray myJArray)
                throw new Exception(
                    $"[{nameof(DatabaseReference)}] {nameof(MoveArrayChild)} {nameof(MyJToken)} is not a JArray");

            var childJToken = myJArray[oldIndex];
            myJArray.RemoveAt(oldIndex);
            myJArray.Insert(newIndex, childJToken);
            Database.JObject = databaseJObject;
            ChildMoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), oldIndex.ToString());
            InvokeParentChildChangedEvents(Parent, ChildMoved.Value);
        }

        public void MoveObjectChild(string oldKey, string newKey)
        {
            var databaseJObject = Database.JObject;
            if (MyJToken is not JObject myJObject)
                throw new Exception(
                    $"[{nameof(DatabaseReference)}] {nameof(MoveObjectChild)} {nameof(MyJToken)} is not a JObject");

            var childJToken = myJObject[oldKey];
            myJObject.Remove(oldKey);
            myJObject.Add(newKey, childJToken);
            Database.JObject = databaseJObject;
            ChildMoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), oldKey);
            InvokeParentChildChangedEvents(Parent, ChildMoved.Value);
        }

        private void InvokeParentChildChangedEvents(DatabaseReference currentParent,
            ChildChangedEventArgs childChangedEventArgs)
        {
            while (currentParent != null)
            {
                currentParent.ChildChanged.Value = childChangedEventArgs;
                currentParent = currentParent.Parent;
            }
        }
        
        private void InvokeValueChangeOnAllReferences(JObject databaseObject)
        {
            foreach (var databaseReferenceEntry in DatabaseReferenceEntries)
            {
                if (databaseReferenceEntry.database != Database) continue;
                if (databaseReferenceEntry.databaseReference.Equals(this)) continue;
                var databaseReference = databaseReferenceEntry.databaseReference;
                var path = databaseReferenceEntry.path;
                var jToken = databaseObject.SelectToken(path);
                databaseReference.ValueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(jToken, databaseReference));
            }
        }
    }
}