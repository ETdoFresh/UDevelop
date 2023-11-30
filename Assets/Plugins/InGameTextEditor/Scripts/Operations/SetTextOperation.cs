namespace InGameTextEditor.Operations
{
    /// <summary>
    /// SetTextOperation defines an operation replacing the editor's content
    /// with the given text.
    /// </summary>
    public class SetTextOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // delete existing text
            DELETING,

            // insert new text
            INSERTING,

            // cleanup after insertion
            CLEANUP
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.DELETING;

        /// <summary>
        /// The remaining text to be inserted.
        /// </summary>
        public string remainingText;

        /// <summary>
        /// The vertical offset of the line currently being inserted.
        /// </summary>
        public float tmpOffset = 0f;

        /// <summary>
        /// Creates a new SetTextOperation.
        /// </summary>
        /// <param name="text">Text to be inserted.</param>
        public SetTextOperation(string text)
        {
            this.remainingText = string.Copy(text);
        }
    }
}