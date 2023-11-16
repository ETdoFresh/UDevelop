namespace CommandSystem.Commands.EditorOnly
{
    public class StopCommandCSharp : CommandCSharp
    {
        // public override bool AddToHistory => false;

        public StopCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    public class PauseCommandCSharp : CommandCSharp
    {
        // public override bool AddToHistory => false;

        public PauseCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
        }
    }
}