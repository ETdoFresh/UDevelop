namespace InGameTextEditor.Operations
{
    /// <summary>
    /// DeleteOperation defines an operation deleting the selected text, if
    /// anything has been selected. Without a valid selection, this operation
    /// deletes the character before or after the caret.
    /// </summary>
    public class DeleteOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation
            START,

            // executing the DeleteTextOperation
            DELETE,

            // cleanup after the DeleteTextOperation has completed
            CLEANUP
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// If deleting a single character (no valid selection), delete the
        /// character after the caret if <c>forward == true</c>; otherwise,
        /// delete the character before the caret.
        /// </summary>
        public bool forward;

        /// <summary>
        /// The DeleteTextOperation executed in the DELETE state.
        /// </summary>
        public DeleteTextOperation deleteTextOp;

        /// <summary>
        /// Creates a new DeleteOperation.
        /// </summary>
        /// <param name="forward">If deleting a single character (no valid
        /// selection), delete the character after the caret if
        /// <c>forward == true</c>; otherwise, delete the character before the
        /// caret.</param>
        public DeleteOperation(bool forward)
        {
            this.forward = forward;
        }
    }
}