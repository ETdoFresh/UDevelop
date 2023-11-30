namespace InGameTextEditor.Operations
{
    /// <summary>
    /// RebuildLinesOperation defines an operation rebuilding all text lines.
    /// </summary>
    public class RebuildLinesOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation
            START,

            // rebuild line
            REBUILD,

            // cleanup after all lines have been rebuilt
            CLEANUP
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// Vertical offset of the line currently being rebuilt.
        /// </summary>
        public float tmpOffset = 0f;

        /// <summary>
        /// Control variable when iterating over the lines.
        /// </summary>
        public int lineIndex = 0;
    }
}