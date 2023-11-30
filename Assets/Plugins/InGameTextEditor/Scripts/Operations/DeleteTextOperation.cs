namespace InGameTextEditor.Operations
{
    /// <summary>
    /// DeleteTextOperation defines an operation deleting the selected text.
    /// </summary>
    public class DeleteTextOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation
            START,

            // if selection contains only one line
            DELETE_SINGLE_LINE,

            // delete first line of multi-line selection
            DELETE_FIRST_LINE,

            // delete intermediate or last line of multi-line selection
            DELETE_INTERMEDIATE_OR_LAST_LINE,

            // cleanup after deletion
            CLEANUP
        }

        /// <summary>
        /// Selection of text to be deleted.
        /// </summary>
        public readonly Selection deleteSelection;

        /// <summary>
        /// If <c>true</c>, add Delete action to history.
        /// </summary>
        public readonly bool addToHistory;

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// Editor's state before deleting the text.
        /// </summary>
        public History.State editorStateBefore;

        /// <summary>
        /// The text to be deleted.
        /// </summary>
        public string selectedText;

        /// <summary>
        /// If <c>true</c>, the longest line needs to be recalculated when the
        /// operation is completed.
        /// </summary>
        public bool recalculateLongestLineWidth = false;

        /// <summary>
        /// Text in the first line of the selection before the selection start
        /// text position.
        /// </summary>
        public string before;

        /// <summary>
        /// Text in the last line of the selection after the selection end text
        /// position.
        /// </summary>
        public string after;

        /// <summary>
        /// Control variable when iterating over the multi-line selection.
        /// </summary>
        public int lineIndex = 0;

        /// <summary>
        /// Creates a new DeleteTextOperation.
        /// </summary>
        /// <param name="deleteSelection">Selection to be deleted.</param>
        /// <param name="addToHistory">If set to <c>true</c> add Delete action
        /// to history.</param>
        public DeleteTextOperation(Selection deleteSelection, bool addToHistory)
        {
            this.deleteSelection = deleteSelection.IsReversed ? new Selection(deleteSelection.end, deleteSelection.start) : deleteSelection;
            this.addToHistory = addToHistory;
        }
    }
}