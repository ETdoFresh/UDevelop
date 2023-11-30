namespace InGameTextEditor.History
{
    /// <summary>
    /// An event is an element in the editor's history. Each event has a
    /// predecessor and a successor.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The previous event in the history.
        /// </summary>
        public Event previous = null;

        /// <summary>
        /// The next event in the history.
        /// </summary>
        public Event next = null;
    }
}