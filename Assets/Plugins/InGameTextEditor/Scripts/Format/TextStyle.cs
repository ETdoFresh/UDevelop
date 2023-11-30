using UnityEngine;

namespace InGameTextEditor.Format
{
    /// <summary>
    /// Defines a text style used when formatting the text editor's content.
    /// </summary>
    [System.Serializable]
    public class TextStyle
    {
        /// <summary>
        /// Font style.
        /// </summary>
        public FontStyle fontStyle = FontStyle.Normal;

        /// <summary>
        /// Indicates whether the default font style should be overridden by
        /// this text style.
        /// </summary>
        public bool overrideFontStyle = false;

        /// <summary>
        /// Font color.
        /// </summary>
        public Color fontColor = new Color(0f, 0f, 0f, 1f);

        /// <summary>
        /// Indicates whether the default font color should be overridden by
        /// this text style.
        /// </summary>
        public bool overrideColor = false;

        /// <summary>
        /// Creates a new text style overriding the default font style but
        /// leaving the font color unchanged.
        /// </summary>
        /// <param name="fontStyle">Font style.</param>
        public TextStyle(FontStyle fontStyle)
        {
            this.fontStyle = fontStyle;
            overrideFontStyle = true;
        }

        /// <summary>
        /// Creates a new text style overriding the default font color but
        /// leaving the font style unchanged.
        /// </summary>
        /// <param name="fontColor">Font color.</param>
        public TextStyle(Color fontColor)
        {
            this.fontColor = fontColor;
            overrideColor = true;
        }

        /// <summary>
        /// Creates a new text style overriding both the default font style and
        /// the default font color.
        /// </summary>
        /// <param name="fontStyle">Font style.</param>
        /// <param name="fontColor">Font color.</param>
        public TextStyle(FontStyle fontStyle, Color fontColor)
        {
            this.fontStyle = fontStyle;
            this.fontColor = fontColor;
            overrideFontStyle = true;
            overrideColor = true;
        }

        /// <summary>
        /// Gets the richt text open tag for this text style.
        /// </summary>
        /// <value>The richt text open tag.</value>
        public string RichtTextOpenTag
        {
            get
            {
                string openTag = overrideColor ? "<color=#" + ColorUtility.ToHtmlStringRGBA(fontColor) + ">" : "";

                if (overrideFontStyle)
                {
                    switch (fontStyle)
                    {
                        case FontStyle.Bold:
                            openTag += "<b>";
                            break;
                        case FontStyle.Italic:
                            openTag += "<i>";
                            break;
                        case FontStyle.BoldAndItalic:
                            openTag += "<b><i>";
                            break;
                    }
                }

                return openTag;
            }
        }

        /// <summary>
        /// Gets the richt text close tag for this text style.
        /// </summary>
        /// <value>The richt text close tag.</value>
        public string RichtTextCloseTag
        {
            get
            {
                string closeTag = "";

                if (overrideFontStyle)
                {
                    switch (fontStyle)
                    {
                        case FontStyle.Bold:
                            closeTag += "</b>";
                            break;
                        case FontStyle.Italic:
                            closeTag += "</i>";
                            break;
                        case FontStyle.BoldAndItalic:
                            closeTag += "</i></b>";
                            break;
                    }
                }

                if (overrideColor)
                    closeTag += "</color>";

                return closeTag;
            }
        }
    }
}