namespace InGameTextEditor.Operations
{
    /// <summary>
    /// InsertCharacterOperation defines an operation inserting a given
    /// character after the caret and subsequently placing the caret after the
    /// inserted character. If text has been selected before this operation, it
    /// will be deleted before the new character is inserted.
    /// </summary>
    public class InsertCharacterOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation
            START,

            // delete selected text (if valid selection exists)
            DELETE,

            // insert character
            INSERT,

            // cleanup after character has been inserted
            CLEANUP
        }

        /// <summary>
        /// The character to be inserted.
        /// </summary>
        public readonly char character;

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// The DeleteTextOperation deleting the selected text
        /// </summary>
        public DeleteTextOperation deleteTextOp;

        /// <summary>
        /// Creates a new InsertCharacterOparation.
        /// </summary>
        /// <param name="character">The character to be inserted.</param>
        public InsertCharacterOperation(char character)
        {
            this.character = character;
        }
    }
}