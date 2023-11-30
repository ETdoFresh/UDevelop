using UnityEngine;

namespace InGameTextEditor.Format
{
    /// <summary>
    /// Defines how a part of a line is formatted
    /// </summary>
    public class TextFormatGroup
    {
        /// <summary>
        /// Start index within the line.
        /// </summary>
        public int startIndex;

        /// <summary>
        /// End index within the line.
        /// </summary>
        public int endIndex;

        /// <summary>
        /// Text style for this part of the line.
        /// </summary>
        public TextStyle textStyle;

        /// <summary>
        /// Creates a new TextFormatGroup.
        /// </summary>
        /// <param name="startIndex">Start index.</param>
        /// <param name="endIndex">End index.</param>
        /// <param name="textStyle">Text style.</param>
        public TextFormatGroup(int startIndex, int endIndex, TextStyle textStyle)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.textStyle = textStyle;
            if (startIndex > endIndex)
                throw new UnityException("startIndex must not be greater than endIndex");
        }

        /// <summary>
        /// Compares two text format groups by their start indices.
        /// </summary>
        /// <returns>-1, if <c>textFormatGroup1</c> starts before
        /// <c>textFormatGroup2</c>; 1, if <c>textFormatGroup1</c> starts after
        /// <c>textFormatGroup2</c>; 0, if both groups atart at the same index.
        /// </returns>
        /// <param name="textFormatGroup1">Text format group 1.</param>
        /// <param name="textFormatGroup2">Text format group 2.</param>
        public static int Sort(TextFormatGroup textFormatGroup1, TextFormatGroup textFormatGroup2)
        {
            if (textFormatGroup1 == null)
                return 1;
            else if (textFormatGroup2 == null)
                return -1;
            else if (textFormatGroup1.startIndex < textFormatGroup2.startIndex)
                return -1;
            else if (textFormatGroup1.startIndex > textFormatGroup2.startIndex)
                return 1;
            else
                return 0;
        }
    }
}