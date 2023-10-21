namespace CommandSystem.Commands
{
    public class PlayCommand : Command
    {
        public override bool AddToHistory => false;

        public PlayCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true;
#endif
        }
    }
}