namespace InGameTextEditor.History
{
    /// <summary>
    /// The Action class hold the editor's states before and after the action is
    /// executed.
    /// </summary>
    public class Action : Event
    {
        /// <summary>
        /// The state before the action is executed.
        /// </summary>
        public State stateBefore = null;

        /// <summary>
        /// The state after the action is executed.
        /// </summary>
        public State stateAfter = null;
    }
}