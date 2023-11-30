namespace InGameTextEditor.Operations
{
    /// <summary>
    /// MoveCaretOperation defines an operation moving the caret one step up,
    /// down, left, or right.
    /// </summary>
    public class MoveCaretOperation : IOperation
    {
        /// <summary>
        /// The four directions.
        /// </summary>
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        /// <summary>
        /// The direction in which the caret is to move.
        /// </summary>
        public readonly Direction direction;

        /// <summary>
        /// If <c>true</c>, expand the selection when moving the caret.
        /// </summary>
        public readonly bool select;

        /// <summary>
        /// If <c>true</c>, jump to the beginning or end of the next word. This
        /// only applies when moving the caret left or right.
        /// </summary>
        public readonly bool entireWord;

        /// <summary>
        /// Create a new MoveCaretOperation.
        /// </summary>
        /// <param name="direction">Direction in which to move the caret.
        /// </param>
        /// <param name="select">If set to <c>true</c>, expand the selection.
        /// </param>
        /// <param name="entireWord">If set to <c>true</c>, jump to beginning
        /// or end of word (only applies when moving the caret left or right).
        /// </param>
        public MoveCaretOperation(Direction direction, bool select, bool entireWord)
        {
            this.direction = direction;
            this.select = select;
            this.entireWord = entireWord;
        }
    }
}