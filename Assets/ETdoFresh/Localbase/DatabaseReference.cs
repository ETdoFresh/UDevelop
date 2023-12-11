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
        public LocalbaseDatabase Database => queryEntry.database;

        private string Path => queryEntry.path;
        private JToken MyJToken => Database?.JObject?.SelectToken(Path);

        public DatabaseReference Parent => IsRoot() ? null : Create(Database, GetParent());

        public DatabaseReference Root => Create(Database, GetRoot());

        public static DatabaseReference Create(LocalbaseDatabase database, string path, object caller = null)
        {
            var existingQueryEntry = QueryEntries
                .FirstOrDefault(entry => entry.database == database && entry.path == path);
            if (existingQueryEntry != null) return existingQueryEntry.databaseReference;
            var databaseReference = new DatabaseReference();
            var jToken = database.JObject.SelectToken(path);
            databaseReference.queryEntry = new QueryEntry
            {
                query = databaseReference,
                databaseReference = databaseReference,
                database = database,
                path = path,
                valueChanged = new Data<ValueChangedEventArgs>(
                    new ValueChangedEventArgs(new DataSnapshot(jToken, databaseReference))),
                childAdded = new Data<ChildChangedEventArgs>(null),
                childChanged = new Data<ChildChangedEventArgs>(null),
                childRemoved = new Data<ChildChangedEventArgs>(null),
                childMoved = new Data<ChildChangedEventArgs>(null)
            };
            QueryEntries.Add(databaseReference.queryEntry);
            return databaseReference;
        }

        public void Destroy()
        {
            if (queryEntry == null) return;
            QueryEntries.Remove(queryEntry);
            queryEntry.query = null;
            queryEntry.databaseReference = null;
            queryEntry.database = null;
            queryEntry.path = null;
            queryEntry.valueChanged = null;
            queryEntry.childAdded = null;
            queryEntry.childChanged = null;
            queryEntry.childRemoved = null;
            queryEntry.childMoved = null;
            queryEntry = null;
        }

        public bool HasChild(string pathString) => Child(pathString).MyJToken != null;

        public DatabaseReference Child(string pathString) => Create(Database, IsRoot() ? pathString : $"{Path}.{pathString}");
        
        public DatabaseReference Child(int index) => Create(Database, $"{Path}[{index}]");

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
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(snapshot);
            InvokeParentChildChangedEvents(Parent, new ChildChangedEventArgs(snapshot, null));

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
            queryEntry.childAdded.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(myJArray, this));
            InvokeParentChildChangedEvents(Parent, queryEntry.childAdded.Value);
        }

        public void AddArrayChild(object obj)
        {
            var settings = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            var jToken = JToken.FromObject(obj, settings);
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
            queryEntry.childAdded.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(myJObject, this));
            InvokeParentChildChangedEvents(Parent, queryEntry.childAdded.Value);
        }

        public void AddObjectChild(string key, object obj)
        {
            var settings = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            var jToken = JToken.FromObject(obj, settings);
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
            queryEntry.childRemoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(myJArray, this));
            InvokeParentChildChangedEvents(Parent, queryEntry.childRemoved.Value);
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
            queryEntry.childRemoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(myJObject, this));
            InvokeParentChildChangedEvents(Parent, queryEntry.childRemoved.Value);
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
            queryEntry.childMoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), oldIndex.ToString());
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(myJArray, this));
            InvokeParentChildChangedEvents(Parent, queryEntry.childMoved.Value);
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
            queryEntry.childMoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), oldKey);
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(myJObject, this));
            InvokeParentChildChangedEvents(Parent, queryEntry.childMoved.Value);
        }

        private void InvokeParentChildChangedEvents(DatabaseReference currentParent,
            ChildChangedEventArgs childChangedEventArgs)
        {
            while (currentParent != null)
            {
                currentParent.queryEntry.childChanged.Value = childChangedEventArgs;
                currentParent.queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(currentParent.MyJToken, currentParent));
                currentParent = currentParent.Parent;
            }
        }

        private void InvokeValueChangeOnAllReferences(JObject databaseObject)
        {
            foreach (var queryEntry in QueryEntries)
            {
                if (queryEntry.database != Database) continue;
                if (queryEntry.databaseReference.Equals(this)) continue;
                var databaseReference = queryEntry.databaseReference;
                var path = queryEntry.path;
                var jToken = databaseObject.SelectToken(path);
                databaseReference.queryEntry.valueChanged.Value =
                    new ValueChangedEventArgs(new DataSnapshot(jToken, databaseReference));
            }
        }

        public void DetectChanges(JToken previousJToken)
        {
            if (previousJToken is JObject previousJObject && MyJToken is JObject currentJObject)
            {
                var previousKeys = previousJObject.Properties().Select(p => p.Name).ToList();
                var currentKeys = currentJObject.Properties().Select(p => p.Name).ToList();
                var addedKeys = currentKeys.Except(previousKeys).ToList();
                var removedKeys = previousKeys.Except(currentKeys).ToList();
                var commonKeys = previousKeys.Intersect(currentKeys).ToList();

                foreach (var removedKey in removedKeys)
                {
                    var childJToken = previousJObject[removedKey];
                    queryEntry.childRemoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
                    InvokeParentChildChangedEvents(Parent, queryEntry.childRemoved.Value);
                    
                    var childPath = IsRoot() ? removedKey : $"{Path}.{removedKey}";
                    for (var i = QueryEntries.Count - 1; i >= 0; i--)
                    {
                        var databaseReferenceEntry = QueryEntries[i];
                        if (databaseReferenceEntry.database != Database) continue;
                        if (!databaseReferenceEntry.path.StartsWith(childPath)) continue;
                        databaseReferenceEntry.databaseReference.Destroy();
                    }
                }
                
                foreach (var addedKey in addedKeys)
                {
                    var childJToken = currentJObject[addedKey];
                    queryEntry.childAdded.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
                    InvokeParentChildChangedEvents(Parent, queryEntry.childAdded.Value);
                }

                foreach (var commonKey in commonKeys)
                {
                    var previousChildJToken = previousJObject[commonKey];
                    var childDatabaseReference = Child(commonKey);
                    childDatabaseReference.DetectChanges(previousChildJToken);
                }
            }
            else if (previousJToken is JArray previousJArray && MyJToken is JArray currentJArray)
            {
                var previousCount = previousJArray.Count;
                var currentCount = currentJArray.Count;
                var minCount = Math.Min(previousCount, currentCount);
                var maxCount = Math.Max(previousCount, currentCount);
                
                for (var i = 0; i < minCount; i++)
                {
                    var previousChildJToken = previousJArray[i];
                    var childDatabaseReference = Child(i);
                    childDatabaseReference.DetectChanges(previousChildJToken);
                }
                
                if (previousCount < currentCount)
                {
                    for (var i = minCount; i < maxCount; i++)
                    {
                        var childJToken = currentJArray[i];
                        queryEntry.childAdded.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
                        InvokeParentChildChangedEvents(Parent, queryEntry.childAdded.Value);
                    }
                }
                
                else if (previousCount > currentCount)
                {
                    for (var i = minCount; i < maxCount; i++)
                    {
                        var childJToken = previousJArray[i];
                        queryEntry.childRemoved.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
                        InvokeParentChildChangedEvents(Parent, queryEntry.childRemoved.Value);
                        
                        var childPath = IsRoot() ? i.ToString() : $"{Path}[{i}]";
                        for (var j = QueryEntries.Count - 1; j >= 0; j--)
                        {
                            var databaseReferenceEntry = QueryEntries[j];
                            if (databaseReferenceEntry.database != Database) continue;
                            if (!databaseReferenceEntry.path.StartsWith(childPath)) continue;
                            databaseReferenceEntry.databaseReference.Destroy();
                        }
                    }
                }
            }
            else if (!JToken.DeepEquals(previousJToken, MyJToken))
            {
                var snapshot = new DataSnapshot(MyJToken, this);
                queryEntry.valueChanged.Value = new ValueChangedEventArgs(snapshot);
                InvokeParentChildChangedEvents(Parent, new ChildChangedEventArgs(snapshot, null));
            }
        }

        public DatabaseReference Push()
        {
            var databaseJObject = Database.JObject;
            if (MyJToken is not JArray myJArray)
                throw new Exception(
                    $"[{nameof(DatabaseReference)}] {nameof(Push)} {nameof(MyJToken)} is not a JArray");

            var childJToken = new JObject();
            myJArray.Add(childJToken);
            Database.JObject = databaseJObject;
            queryEntry.childAdded.Value = new ChildChangedEventArgs(new DataSnapshot(childJToken, this), null);
            queryEntry.valueChanged.Value = new ValueChangedEventArgs(new DataSnapshot(myJArray, this));
            InvokeParentChildChangedEvents(Parent, queryEntry.childAdded.Value);
            return Child(myJArray.Count - 1);
        }
    }
}