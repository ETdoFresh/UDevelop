namespace InGameTextEditor.Operations
{
    /// <summary>
    /// ModifyIndentOperation defines an operation modifying (increasing or
    /// decreasing) the indent of the selected lines.
    /// </summary>
    public class ModifyIndentOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation
            START,

            // modify indent
            MODIFY_INDENT,

            // cleanup after indent has been increased
            CLEANUP
        }

        /// <summary>
        /// Indicates whether the indent should be increased (<c>true</c>) or
        /// decreased (<c>false</c>).
        /// </summary>
        public readonly bool increase;

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// Index of first line of selection.
        /// </summary>
        public int startLineIndex;

        /// <summary>
        /// Index of last line of selection.
        /// </summary>
        public int endLineIndex;

        /// <summary>
        /// Control variable when iterating over lines.
        /// </summary>
        public int lineIndex;

        /// <summary>
        /// Creates a new ModifyIndentOperation.
        /// </summary>
        /// <param name="increase">If set to <c>true</c> increase, otherwise
        /// decrease indent.</param>
        public ModifyIndentOperation(bool increase)
        {
            this.increase = increase;
        }
    }
}