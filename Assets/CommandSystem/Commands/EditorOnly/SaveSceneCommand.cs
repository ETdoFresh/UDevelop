using System;

namespace CommandSystem.Commands.EditorOnly
{
    [Serializable]
    public class SaveSceneCommand : Command
    {
        public SaveSceneCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
#endif
        }
    }
}