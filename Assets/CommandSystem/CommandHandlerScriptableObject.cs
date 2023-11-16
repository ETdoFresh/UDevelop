using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandSystem
{
    public class CommandHandlerScriptableObject : ScriptableObject
    {
        private static CommandHandlerScriptableObject _instance;

        private static CommandHandlerScriptableObject Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Resources.Load<CommandHandlerScriptableObject>("CommandData");
                if (_instance != null) return _instance;
#if UNITY_EDITOR
                _instance = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(CommandHandlerScriptableObject)}")
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<CommandHandlerScriptableObject>)
                    .FirstOrDefault();
                if (_instance != null) return _instance;
#endif
                _instance = CreateInstance<CommandHandlerScriptableObject>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
                return _instance;
            }
        }

        public static List<string> Inputs => Instance._inputs;
        public static List<string> Outputs => Instance._outputs;
        public static List<CommandCSharp> History => Instance._history;
        public static List<string> Display => Instance._display;
        public static int HistoryIndex { get => Instance._historyIndex; set => Instance._historyIndex = value; }

        [SerializeField] private List<string> _inputs = new();
        [SerializeField] private List<string> _outputs = new();
        [SerializeField] private List<CommandCSharp> _history = new();
        [SerializeField] private List<string> _display = new();
        [SerializeField] private int _historyIndex;

        public static void Clear()
        {
            Instance._inputs.Clear();
            Instance._outputs.Clear();
            Instance._history.Clear();
            Instance._display.Clear();
            Instance._historyIndex = 0;
        }
    }
}