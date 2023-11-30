namespace InGameTextEditor.History
{
    /// <summary>
    /// This class holds all information about the state of the editor before
    /// or after an operation (or a series of operations) is applied.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Caret text position.
        /// </summary>
        public TextPosition caretTextPosition;

        /// <summary>
        /// Text selection.
        /// </summary>
        public Selection selection;

        /// <summary>
        /// Creates a new state. <c>caretTextPosition</c> and <c>selection</c>,
        /// if it is not null, are cloned.
        /// </summary>
        /// <param name="caretTextPosition">Caret text position.</param>
        /// <param name="selection">Selection.</param>
        public State(TextPosition caretTextPosition, Selection selection)
        {
            this.caretTextPosition = caretTextPosition.Clone();
            if (selection != null)
                this.selection = selection.Clone();
        }
    }
}