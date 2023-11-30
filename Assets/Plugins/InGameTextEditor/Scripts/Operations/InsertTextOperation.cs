namespace InGameTextEditor.Operations
{
    /// <summary>
    /// InsertCharacterOperation defines an operation inserting a given string
    /// after the given text position. If the string to be inserted
    /// contains line breaks, this operation will add one or more lines to the
    /// editor.
    /// </summary>
    public class InsertTextOperation : IOperation
    {
        /// <summary>
        /// All possible states while executing this operation.
        /// </summary>
        public enum State
        {
            // start of the operation
            START,

            // insert string without line breaks
            INSERT_SINGLE_LINE,

            // insert first line of multi-line string
            INSERT_FIRST_LINE,

            // insert intermediate line of multi-line string
            INSERT_INTERMEDIATE_LINE,

            // insert last line of multi-line string
            INSERT_LAST_LINE,

            // cleanup after insertion
            CLEANUP
        }

        /// <summary>
        /// The text position after which the text will be inserted.
        /// </summary>
        public readonly TextPosition textPosition;

        /// <summary>
        /// The text to be inserted.
        /// </summary>
        public readonly string text;

        /// <summary>
        /// If <c>true</c>, add Insert action to history.
        /// </summary>
        public readonly bool addToHistory = false;

        /// <summary>
        /// The current state.
        /// </summary>
        public State state = State.START;

        /// <summary>
        /// Editor's state before inserting the text.
        /// </summary>
        public History.State editorStateBefore = null;

        /// <summary>
        /// The start position of the inserted text.
        /// </summary>
        public TextPosition startTextPosition;

        /// <summary>
        /// If <c>true</c>, the longest line needs to be recalculated when the
        /// operation is completed.
        /// </summary>
        public bool recalculateLongestLineWidth = true;

        /// <summary>
        /// Line height before the text is inserted.
        /// </summary>
        public float oldLineHeight;

        /// <summary>
        /// Line width before the text is inserted
        /// </summary>
        public float oldLineWidth;

        /// <summary>
        /// Text before the inserted text.
        /// </summary>
        public string before;

        /// <summary>
        /// Text after the inserted text.
        /// </summary>
        public string after;

        /// <summary>
        /// Single text lines of the inserted text.
        /// </summary>
        public string[] textLines;

        /// <summary>
        /// Vertical offset of the new line currently being inserted.
        /// </summary>
        public float tmpOffset;

        /// <summary>
        /// Control variable counting the already inserted lines.
        /// </summary>
        public int lineIndex = 1;

        /// <summary>
        /// Creates a new InsertTextOperation.
        /// </summary>
        /// <param name="textPosition">Text position where the text should be
        /// inserted.</param>
        /// <param name="text">Text to be inserted.</param>
        /// <param name="addToHistory">If set to <c>true</c> add to Insert
        /// action to history.</param>
        public InsertTextOperation(TextPosition textPosition, string text, bool addToHistory)
        {
            this.textPosition = textPosition;
            this.text = text;
            this.addToHistory = addToHistory;
        }
    }
}