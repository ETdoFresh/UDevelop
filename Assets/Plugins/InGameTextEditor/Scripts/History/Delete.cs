namespace InGameTextEditor.History
{
    /// <summary>
    /// The Delete class holds a selection which is to be deleted.
    /// </summary>
    public class Delete : Action
    {
        /// <summary>
        /// The selection to be deleted.
        /// </summary>
        public Selection selection;

        /// <summary>
        /// The deleted text.
        /// </summary>
        public string deletedText;

        /// <summary>
        /// Creates a new Delete action. Both parameters <c>selection</c> and
        /// <c>deletedText</c> are cloned.
        /// </summary>
        /// <param name="selection">Selection to be deleted.</param>
        /// <param name="selection">The text that is deleted.</param>
        public Delete(Selection selection, string deletedText, State stateBefore, State stateAfter)
        {
            this.selection = selection.IsReversed ? new Selection(selection.end, selection.start) : selection.Clone();
            this.deletedText = deletedText + "";
            this.stateBefore = stateBefore;
            this.stateAfter = stateAfter;
        }
    }
}