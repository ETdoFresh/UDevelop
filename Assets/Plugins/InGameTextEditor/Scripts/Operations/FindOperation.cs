namespace InGameTextEditor.Operations
{
    /// <summary>
    /// FindOperation defines an operation searching for a given search string
    /// in the editor.
    /// </summary>
    public class FindOperation : IOperation
    {
        /// <summary>
        /// The search string to be found in the editor's content.
        /// </summary>
        public readonly string searchString;

        /// <summary>
        /// If <c>true</c>, for occurrences after the current selection or caret
        /// position; otherwise, search for occurrences before.
        /// </summary>
        public readonly bool forward;

        /// <summary>
        /// Creates a new FindOperation.
        /// </summary>
        /// <param name="searchString">Search string.</param>
        /// <param name="forward">Forward search.</param>
        public FindOperation(string searchString, bool forward)
        {
            this.searchString = searchString;
            this.forward = forward;
        }
    }
}