using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CommandSystem
{
    public static class CommandRunner
    {
        private const float UpdateRate = 0;
        private static float _nextUpdate;
        private static Dictionary<string, JObject> aliasMap = new();
        private static Dictionary<string, List<CommandObject>> commandMap = new();

        public static Dictionary<string, JObject> AliasMap => aliasMap;
        public static Dictionary<string, List<CommandObject>> CommandMap => commandMap;

        public static OutputData Run(string commandString)
        {
            AttemptUpdateAliasMap();

            if (CommandObject.TryRun(commandString, out var outputData))
                return outputData;

            if (CommandJsonRunner.TryRun(commandString, out outputData))
                return outputData;

            throw new System.Exception($"Command not found\n{commandString}");
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
                foreach (var commandObject in commandObjects)
                foreach (var commandAliasToken in jsonFileAlias)
                {
                    var commandAlias = commandAliasToken.ToString();
                    if (!commandMap.ContainsKey(commandAlias))
                        commandMap[commandAlias] = new List<CommandObject>();
                    commandMap[commandAlias].Add(commandObject);
                }
            }
            
            var commandletFiles =
                UnityEditor.AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/CommandSystem/Commandlets" });
            foreach (var commandletFile in commandletFiles)
            {
                var commandletFilePath = UnityEditor.AssetDatabase.GUIDToAssetPath(commandletFile);
                var commandletFilename = System.IO.Path.GetFileNameWithoutExtension(commandletFilePath);
                var commandletFileText = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(commandletFilePath).text;
                commandletFileText = $"// Filename: {commandletFilename}\n" + commandletFileText;
                var commandObjects = CommandObject.FromCommandletString(commandletFileText);
                if (commandObjects == null) continue;
                foreach (var commandObject in commandObjects)
                foreach (var commandAlias in commandObject.Aliases)
                {
                    if (!commandMap.ContainsKey(commandAlias))
                        commandMap[commandAlias] = new List<CommandObject>();
                    commandMap[commandAlias].Add(commandObject);
                }
            }
#endif
        }

        public static void Update()
        {
            AttemptUpdateAliasMap();
        }
    }
}