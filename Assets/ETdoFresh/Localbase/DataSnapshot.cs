using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ETdoFresh.Localbase
{
    public class DataSnapshot
    {
        private JToken _jToken;
        private DatabaseReference _databaseReference;

        internal DataSnapshot(JToken jToken, DatabaseReference databaseReference)
        {
            _jToken = jToken;
            _databaseReference = databaseReference;
        }

        public bool HasChildren
        {
            get
            {
                return _jToken switch
                {
                    JObject jObject => jObject.HasValues,
                    JArray jArray => jArray.HasValues,
                    _ => false
                };
            }
        }

        public bool Exists => _jToken != null;

        public object Value => GetValue(false);

        public long ChildrenCount => _jToken switch
        {
            JObject jObject => jObject.Count,
            JArray jArray => jArray.Count,
            _ => 0
        };

        public DatabaseReference Reference => _databaseReference;

        public string Key => _databaseReference.Key;

        public IEnumerable<DataSnapshot> Children => _jToken switch
        {
            JObject jObject => jObject.Children()
                .Select(child => new DataSnapshot(child, _databaseReference.Child(child.Path))),
            JArray jArray => jArray.Children()
                .Select(child => new DataSnapshot(child, _databaseReference.Child(child.Path))),
            _ => Enumerable.Empty<DataSnapshot>()
        };

        // public object Priority =>

        public DataSnapshot Child(string path) => new(_jToken.SelectToken(path), _databaseReference.Child(path));

        public bool HasChild(string path) => _jToken.SelectToken(path) != null;

        public string GetRawJsonValue() => _jToken.ToString();

        public object GetValue(bool useExportFormat) => _jToken?.ToObject<object>();

        public override string ToString() => $"DataSnapshot {{ key = {Key}, value = {Value} }}";
    }
}