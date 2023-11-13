using System;

namespace CommandSystem.Commands.EditorOnly
{
    [Serializable]
    public class PlayCommandCSharp : CommandCSharp
    {
        public PlayCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true;
#endif
        }
    }
}