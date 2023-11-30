namespace InGameTextEditor.Operations
{
    /// <summary>
    /// PlaceCaretOperation defines an operation placing the caret to the given
    /// text position.
    /// </summary>
    public class PlaceCaretOperation : IOperation
    {
        /// <summary>
        /// The text position where the caret should be placed.
        /// </summary>
        public readonly TextPosition textPosition;

        /// <summary>
        /// Creates a new PlaceCaretOperation.
        /// </summary>
        /// <param name="textPosition">Text position where the caret should be
        /// placed.</param>
        public PlaceCaretOperation(TextPosition textPosition)
        {
            this.textPosition = textPosition;
        }
    }
}