namespace InGameTextEditor.Operations
{
    /// <summary>
    /// UndoOperation defines an operation reverting the most recent action(s)
    /// in the history.
    /// </summary>
    public class UndoOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation
            START,

            // traverse history until next milestone
            TRAVERSE_HISTORY,

            // revert action
            REVERT_ACTION,

            // cleanup after action has been applied
            CLEANUP
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// The history event currently being processed.
        /// </summary>
        public History.Event e;

        /// <summary>
        /// The currently operation currently being reverted.
        /// </summary>
        public IOperation revertedOperation;
    }
}