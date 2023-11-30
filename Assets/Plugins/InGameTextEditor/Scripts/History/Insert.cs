namespace InGameTextEditor.History
{
    /// <summary>
    /// The Insert class holds a text position, the text to be inserted, and the
    /// position of the caret after the text has been inserted.
    /// </summary>
    public class Insert : Action
    {
        /// <summary>
        /// The text position where text is inserted.
        /// </summary>
        public TextPosition startTextPosition;

        /// <summary>
        /// The position of the caret after the text has been inserted.
        /// </summary>
        public TextPosition endTextPosition;

        /// <summary>
        /// The text to be inserted.
        /// </summary>
        public string text;

        /// <summary>
        /// Creates a new Insert action. All parameters
        /// <c>startTextPosition</c>, <c>endTextPosition</c>, and text are
        /// cloned.
        /// </summary>
        /// <param name="startTextPosition">Text position where text is
        /// inserted.</param>
        /// <param name="endTextPosition">Position of the caret after the text
        /// has been inserted.</param>
        /// <param name="text">Text to be inserted.</param>
        public Insert(TextPosition startTextPosition, TextPosition endTextPosition, string text, State stateBefore, State stateAfter)
        {
            this.startTextPosition = startTextPosition.Clone();
            this.endTextPosition = endTextPosition.Clone();
            this.text = text + "";
            this.stateBefore = stateBefore;
            this.stateAfter = stateAfter;
        }
    }
}