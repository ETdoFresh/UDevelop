using System;

namespace CommandSystem.Commands
{
    [Serializable]
    public class UndoCommand : Command
    {
        public UndoCommand(string commandInput) : base(commandInput) { }

        private string _undoCommandInput;
        
        public override bool AddToHistory => false;
        public override string CommandOutput => $"Undo \"{_undoCommandInput}\" Complete!";

        public override void OnRun(params string[] args)
        {
            if (CommandData.HistoryIndex <= 0) return;
            CommandData.HistoryIndex--;
            CommandData.History[CommandData.HistoryIndex].OnUndo();
            _undoCommandInput = CommandData.History[CommandData.HistoryIndex].CommandInput; 
        }
    }
}