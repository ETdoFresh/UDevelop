using UnityEngine;

namespace InGameTextEditor.Format
{
    /// <summary>
    /// A TextFormatter can be added to the text editor as an optional component
    /// to format the content of the editor. <c>OnLineChanged</c> is called when
    /// a line has changed and needs to be formatted.
    /// </summary>
    public abstract class TextFormatter : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether this text formatter has been initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public abstract bool Initialized { get; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Is called by text editor when the given line has changed.
        /// </summary>
        /// <param name="line">Line that has changed.</param>
        public abstract void OnLineChanged(Line line);
    }
}