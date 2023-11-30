namespace InGameTextEditor.Operations
{
    /// <summary>
    /// PasteOperation defines an operation inserting text from the system
    /// clipboard. If text has been selected before this operation, it
    /// will be deleted before the clipboard text is inserted.
    /// </summary>
    public class PasteOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation and read clipboard text
            START,

            // delete selected text
            DELETE,

            // insert clipboard text
            INSERT,

            // cleanup after insertion
            CLEANUP
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// The clipboard text.
        /// </summary>
        public string clipboardText;

        /// <summary>
        /// Operation deleting the selected text.
        /// </summary>
        public DeleteTextOperation deleteTextOp;

        /// <summary>
        /// Operation inserting the clipboard text.
        /// </summary>
        public InsertTextOperation insertTextOp;
    }
}