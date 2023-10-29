namespace CommandSystem.Commands.EditorOnly
{
    public class StopCommand : Command
    {
        public override bool AddToHistory => false;

        public StopCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    public class PauseCommand : Command
    {
        public override bool AddToHistory => false;

        public PauseCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
        }
    }
}