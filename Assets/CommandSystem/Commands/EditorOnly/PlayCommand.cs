namespace CommandSystem.Commands
{
    public class PlayCommand : Command
    {
        public PlayCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true;
#endif
        }
    }
}