namespace InGameTextEditor.Operations
{
    /// <summary>
    /// RedoOperation defines an operation reapplying the previously reverted
    /// action in the history. 
    /// </summary>
    public class RedoOperation : IOperation
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

            // apply action
            APPLY_ACTION,

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
        /// The operation currently being applied.
        /// </summary>
        public IOperation appliedOperation;
    }
}