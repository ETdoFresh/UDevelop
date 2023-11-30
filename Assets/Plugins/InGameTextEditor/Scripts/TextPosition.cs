namespace InGameTextEditor
{
    /// <summary>
    /// A text position represents a unique position in the text.
    /// </summary>
    public class TextPosition
    {
        /// <summary>
        /// The line index.
        /// </summary>
        public int lineIndex = 0;

        /// <summary>
        /// The column index
        /// </summary>
        public int colIndex = 0;

        /// <summary>
        /// If the optical representation of a text position is ambiguous (e.g.
        /// when a line is split into multiple line fragments),
        /// <c>preferNextLine</c> determines whether the text position should be
        /// shown on the next line.
        /// </summary>
        public bool preferNextLine = false;

        /// <summary>
        /// Creates a new text position.
        /// </summary>
        /// <param name="lineIndex">Line index.</param>
        /// <param name="colIndex">Col index.</param>
        /// <param name="preferNextLine">If set to <c>true</c> prefer next line.</param>
        public TextPosition(int lineIndex, int colIndex, bool preferNextLine = false)
        {
            this.lineIndex = lineIndex;
            this.colIndex = colIndex;
            this.preferNextLine = preferNextLine;
        }

        /// <summary>
        /// Returns a clone of this text position.
        /// </summary>
        /// <returns>The cloned text position.</returns>
        public TextPosition Clone()
        {
            return new TextPosition(lineIndex, colIndex, preferNextLine);
        }

        /// <summary>
        /// Checks for equality with another object.
        /// </summary>
        /// <param name="obj">The compared object.</param>
        /// <returns><c>true</c> if the specified object is equal to this text
        /// position; otherwise, <c>false</c>. Two text positions are equal when
        /// their line and col indices are equal. The <c>preferNextLine</c>
        /// parameter is ignored when comparing two text positions for equality.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is TextPosition))
                return false;
            return ((TextPosition) obj).lineIndex == lineIndex && ((TextPosition) obj).colIndex == colIndex;
        }

        /// <summary>
        /// Serves as a hash function for a <c>TextPosition</c> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in
        /// hashing algorithms and data structures such as a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -16351716;
            hashCode = hashCode * -1521134295 + lineIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + colIndex.GetHashCode();
            return hashCode;
        }
    }
}