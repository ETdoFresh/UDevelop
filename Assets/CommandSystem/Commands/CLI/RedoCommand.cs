using System;

namespace CommandSystem.Commands
{
    [Serializable]
    public class RedoCommand : Command
    {
        public RedoCommand(string commandInput) : base(commandInput) { }
        
        private string _redoCommandInput;
        
        public override bool AddToHistory => false;
        public override string CommandOutput => $"Redo \"{_redoCommandInput}\" Complete!";
        
        public override void OnRun(params string[] args)
        {
            if (CommandData.HistoryIndex >= CommandData.History.Count) return;
            CommandData.History[CommandData.HistoryIndex].OnRedo();
            _redoCommandInput = CommandData.History[CommandData.HistoryIndex].CommandInput;
            CommandData.HistoryIndex++;
        }
    }
}