using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CommandSystem
{
    public static class CommandRunner
    {
        private const float UpdateRate = 0;
        private static float _nextUpdate;
        private static Dictionary<string, CommandObject[]> commandMap = new();
        
        public static Dictionary<string, CommandObject[]> CommandMap => commandMap;

        public static Dictionary<string, ArgData> Run(string commandString)
        {
            AttemptUpdateAliasMap();
            return CommandObject.RunCommandString(commandString, null);
        }

        private static void AttemptUpdateAliasMap()
        {
            if (Application.isPlaying)
            {
                if (Time.time > _nextUpdate)
                {
                    _nextUpdate = Time.time + UpdateRate;
                    UpdateAliasMap();
                }
            }
            else
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.timeSinceStartup > _nextUpdate)
                {
                    _nextUpdate = (float)UnityEditor.EditorApplication.timeSinceStartup + UpdateRate;
                    UpdateAliasMap();
                }
#endif
            }
        }

        private static void UpdateAliasMap()
        {
            commandMap.Clear();
#if UNITY_EDITOR
            var jsonFiles =
                UnityEditor.AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/CommandSystem/Json" });
            foreach (var jsonFile in jsonFiles)
            {
                var jsonFilePath = UnityEditor.AssetDatabase.GUIDToAssetPath(jsonFile);
                var jsonFileText = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(jsonFilePath).text;
                var jsonFileJObject = JObject.Parse(jsonFileText);
                var jsonFileAlias = jsonFileJObject["Aliases"];
                if (jsonFileAlias == null) continue;
                var commandObjects = CommandObject.FromJsonString(jsonFileText);
                if (commandObjects == null) continue;
                var groupedCommandObjects = new Dictionary<string, List<CommandObject>>();
                foreach (var commandObject in commandObjects)
                {
                    var commandObjectNameAndVersion = $"{commandObject.Name} {commandObject.Version}";
                    if (!groupedCommandObjects.ContainsKey(commandObjectNameAndVersion))
                        groupedCommandObjects[commandObjectNameAndVersion] = new List<CommandObject>();
                    groupedCommandObjects[commandObjectNameAndVersion].Add(commandObject);
                }
                foreach (var groupedCommandObject in groupedCommandObjects)
                {
                    var aliases = groupedCommandObject.Value.SelectMany(x => x.Aliases);
                    var commandObjectsForAlias = groupedCommandObject.Value.ToArray();
                    foreach (var alias in aliases) 
                        commandMap[alias] = commandObjectsForAlias;
                }
            }
            
            var commandletFiles =
                UnityEditor.AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/CommandSystem/Commandlets" });
            foreach (var commandletFile in commandletFiles)
            {
                var commandletFilePath = UnityEditor.AssetDatabase.GUIDToAssetPath(commandletFile);
                var commandletFilename = System.IO.Path.GetFileNameWithoutExtension(commandletFilePath);
                var commandletFileText = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(commandletFilePath).text;
                commandletFileText = $"Filename: {commandletFilename}\n" + commandletFileText;
                var commandObjects = CommandObject.FromCommandletString(commandletFileText);
                if (commandObjects == null) continue;
                var groupedCommandObjects = new Dictionary<string, List<CommandObject>>();
                foreach (var commandObject in commandObjects)
                {
                    var commandObjectNameAndVersion = $"{commandObject.Name} {commandObject.Version}";
                    if (!groupedCommandObjects.ContainsKey(commandObjectNameAndVersion))
                        groupedCommandObjects[commandObjectNameAndVersion] = new List<CommandObject>();
                    groupedCommandObjects[commandObjectNameAndVersion].Add(commandObject);
                }
                foreach (var groupedCommandObject in groupedCommandObjects)
                {
                    var aliases = groupedCommandObject.Value.SelectMany(x => x.Aliases);
                    var commandObjectsForAlias = groupedCommandObject.Value.ToArray();
                    foreach (var alias in aliases) 
                        commandMap[alias] = commandObjectsForAlias;
                }
            }
#endif
        }
    }
}