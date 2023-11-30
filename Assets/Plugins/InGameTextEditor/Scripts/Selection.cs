using System.Collections.Generic;

namespace InGameTextEditor
{
    /// <summary>
    /// A selection represents a text selection with a unique start and end.
    /// </summary>
    public class Selection
    {
        /// <summary>
        /// The start of the selection.
        /// </summary>
        public TextPosition start;

        /// <summary>
        /// The end of the selection.
        /// </summary>
        public TextPosition end;

        /// <summary>
        /// Creates a new selection with a given start and end.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public Selection(TextPosition start, TextPosition end)
        {
            this.start = new TextPosition(start.lineIndex, start.colIndex, start.preferNextLine);
            this.end = new TextPosition(end.lineIndex, end.colIndex, end.preferNextLine);
        }

        /// <summary>
        /// Indicates whether this selection is valid (i.e. start index is
        /// different from end index.
        /// </summary>
        /// <value><c>true</c> if start and end are not equal, <c>false</c>
        /// otherwise.</value>
        public bool IsValid
        {
            get { return start.lineIndex != end.lineIndex || start.colIndex != end.colIndex; }
        }

        /// <summary>
        /// Indicates whether this selection is reversed (i.e. start index
        /// > end index</summary>
        /// <value><c>true</c>, if the end of the selection is before the
        /// start, <c>false</c> otherwise.</value>
        public bool IsReversed
        {
            get { return start.lineIndex > end.lineIndex || (start.lineIndex == end.lineIndex && start.colIndex > end.colIndex); }
        }

        /// <summary>
        /// Returns a clone of this selection. Both <c>start</c> and <c>end</c>
        /// are cloned as well.
        /// </summary>
        /// <returns>The cloned selection.</returns>
        public Selection Clone()
        {
            return new Selection(start.Clone(), end.Clone());
        }

        /// <summary>
        /// Checks for equality with another object.
        /// </summary>
        /// <param name="obj">The compared object.</param>
        /// <returns><c>true</c> if the specified object is equal to this
        /// selection; otherwise, <c>false</c>. Two selections are equal if they
        /// span the same part of the text (i.e. start and end are equal) even
        /// when one is reversed and the other is not.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Selection))
                return false;
            Selection other = (Selection) obj;
            return (other.start.Equals(start) && other.end.Equals(end)) || (other.start.Equals(end) && other.end.Equals(start));
        }

        /// <summary>
        /// Serves as a hash function for a <c>Selection</c> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in
        /// hashing algorithms and data structures such as a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 1075529825;
            hashCode = hashCode * -1521134295 + EqualityComparer<TextPosition>.Default.GetHashCode(start);
            hashCode = hashCode * -1521134295 + EqualityComparer<TextPosition>.Default.GetHashCode(end);
            return hashCode;
        }
    }
}