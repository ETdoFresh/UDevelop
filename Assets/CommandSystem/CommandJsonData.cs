using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandSystem
{
    public static class CommandJsonData
    {
        private const string JSON_PATH = "Assets/CommandSystem/Commands/CommandData.json";
        private static JObject _jsonObject;
        
        public static T Get<T>(string tokenPath)
        {
            _jsonObject ??= LoadJson();
            var token = _jsonObject.SelectToken(tokenPath);
            return token == null ? default : token.ToObject<T>();
        }

        public static List<string> GetKeys(string tokenPath)
        {
            _jsonObject ??= LoadJson();
            var token = _jsonObject.SelectToken(tokenPath);
            return token?.Select(x => ((JProperty)x).Name).ToList();
        }
        
        public static Dictionary<string, T> GetKeyAndValue<T>(string tokenPath, string valuePath)
        {
            _jsonObject ??= LoadJson();
            var token = _jsonObject.SelectToken(tokenPath);
            if (token == null) return null;
            var dictionary = new Dictionary<string, T>();
            foreach (var jToken in token)
            {
                var jProperty = (JProperty)jToken;
                var key = jProperty.Name;
                var valueToken = jProperty.Value.SelectToken(valuePath);
                var value = valueToken == null ? default : valueToken.ToObject<T>();
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        private static JObject LoadJson()
        {
            var jsonText = File.ReadAllText(JSON_PATH);
            return JsonConvert.DeserializeObject<JObject>(jsonText);
        }
    }
}