namespace InGameTextEditor.Operations
{
    /// <summary>
    /// CutOperation defines an operation copying the selected text into the
    /// system clipboard and then deleting the selected content.
    /// </summary>
    public class CutOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation (copy into clip board)
            START,

            // execute the DeleteTextOperation
            DELETE,

            // cleanup after the DeleteTextOperation has completed
            CLEANUP
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// The DeleteTextOperation executed after the text has been copied to
        /// the clip board.
        /// </summary>
        public DeleteTextOperation deleteTextOp;
    }
}