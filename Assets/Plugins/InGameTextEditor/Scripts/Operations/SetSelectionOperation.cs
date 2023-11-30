namespace InGameTextEditor.Operations
{
    /// <summary>
    /// SetSelectionOperation defines an operation applying the given selection
    /// to the editor.
    /// </summary>
    public class SetSelectionOperation : IOperation
    {
        /// <summary>
        /// The selection to be applied.
        /// </summary>
        public readonly Selection selection;

        /// <summary>
        /// Creates a new SetSelectionOperation.
        /// </summary>
        /// <param name="selection">Selection to be applied.</param>
        public SetSelectionOperation(Selection selection)
        {
            this.selection = selection;
        }
    }
}