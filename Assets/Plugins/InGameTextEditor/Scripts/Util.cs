using UnityEngine;

namespace InGameTextEditor
{
    /// <summary>
    /// The Util class contains static helper methods.
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Indicates whether the operating system is macOS.
        /// </summary>
        /// <returns><c>true</c>, if running on macOS, <c>false</c> otherwise.
        /// </returns>
        public static bool IsMacOS()
        {
            return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
        }

        /// <summary>
        /// Indicates whether the given character is printable.
        /// e.g. <c>IsPrintableCharacter('A')</c> returns <c>true</c> while
        /// <c>IsPrintableCharacter('\t')</c> returns <c>false</c>.
        /// </summary>
        /// <returns><c>true</c>, if <c>c</c> is a printable character,
        /// <c>false</c> otherwise.</returns>
        /// <param name="c">The character.</param>
        public static bool IsPrintableCharacter(char c)
        {
            // new line
            if (c == '\u000A')
                return true;

            // basic latin characters
            if (c >= '\u0020' && c <= '\u007E')
                return true;

            // latin-1 supplement
            if (c >= '\u00A0' && c <= '\u00FF')
                return true;

            // general punctuation (only simple quotation marks)
            if (c >= '\u2018' && c <= '\u201A')
                return true;

            return false;
        }

        /// <summary>
        /// Replaces all tabs with white spaces. For each tab character, this
        /// method inserts <c>x</c> white space characters so that
        /// <c>(offset + y + x) % tabStopWidth == 0</c>, where y is the length
        /// of the text before that tab character. If <c>text</c> does not
        /// contain any tab characters, this method returns <c>text</c>
        /// unchanged.
        /// </summary>
        /// <returns>The text with all tabs replaced.</returns>
        /// <param name="text">The text.</param>
        /// <param name="tabStopWidth">Tab stop width in characters.</param>
        /// <param name="offset">Text offset.</param>
        public static string ReplaceTabsWithSpaces(string text, int tabStopWidth, int offset = 0)
        {
            string[] textSegments = text.Split('\t');
            if (textSegments.Length <= 1)
                return text;

            text = "";
            for (int j = 0; j < textSegments.Length; j++)
            {
                text += textSegments[j];
                if (j < textSegments.Length - 1)
                {
                    string spaces = "";
                    for (int k = 0; k < tabStopWidth - (text.Length + offset) % tabStopWidth; k++)
                        spaces += " ";
                    text += spaces;
                }
            }

            return text;
        }
    }
}