using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InGameTextEditor.Format;

namespace InGameTextEditor
{
    /// <summary>
    /// A Line represents a single line of text in the editor and holds all text
    /// blocks required to render the text and the line number.
    /// If <c>TextEditor.wrapLines</c> is <c>true</c>, a line will be split into
    /// multiple line fragments, displayed underneath each other, to avoid
    /// horizontal scrolling.
    /// </summary>
    public class Line
    {
        /// <summary>
        /// The Label class holds information about a text label.
        /// </summary>
        public class Label
        {
            /// <summary>
            /// Delete condition. A label is automatically deleted based on the
            /// definded delete condition.
            /// </summary>
            public enum DeleteCondition
            {
                /// <summary>
                /// Label is not deleted automatically.
                /// </summary>
                NONE,

                /// <summary>
                /// Label is deleted when this line changes.
                /// </summary>
                THIS_LINE_CHANGES,

                /// <summary>
                /// Label is deleted when this line or any previous line
                /// changes.
                /// </summary>
                PREVIOUS_LINE_CHANGES,

                /// <summary>
                /// Label is delete when any line changes.
                /// </summary>
                ANYTHING_CHANGES
            }

            /// <summary>
            /// The label's start index
            /// </summary>
            public readonly int startIndex;

            /// <summary>
            /// The label's end index.
            /// </summary>
            public readonly int endIndex;

            /// <summary>
            /// The label's background sprite.
            /// </summary>
            public readonly Sprite backgroundSprite;

            /// <summary>
            /// The label's background color.
            /// </summary>
            public readonly Color backgroundColor;

            /// <summary>
            /// The label's icon sprite.
            /// </summary>
            public readonly Sprite iconSprite;

            /// <summary>
            /// The label's icon color.
            /// </summary>
            public readonly Color iconColor;

            /// <summary>
            /// The label's tooltip message.
            /// </summary>
            public readonly string tooltipMessage;

            /// <summary>
            /// The label's delete condition.
            /// </summary>
            public readonly DeleteCondition deleteCondition;

            /// <summary>
            /// The rectangles used to display the label.
            /// </summary>
            public readonly List<GameObject> labelRects = new List<GameObject>();

            /// <summary>
            /// The icon game object.
            /// </summary>
            public GameObject icon = null;

            /// <summary>
            /// Label constructor.
            /// </summary>
            /// <param name="startIndex">Start index.</param>
            /// <param name="endIndex">End index.</param>
            /// <param name="backgroundSprite">Background sprite.</param>
            /// <param name="backgroundColor">Background color.</param>
            /// <param name="iconSprite">Icon sprite.</param>
            /// <param name="iconColor">Icon color.</param>
            /// <param name="tooltipMessage">Tooltip message.</param>
            /// <param name="deleteCondition">Delete condition.</param>
            public Label(int startIndex, int endIndex, Sprite backgroundSprite, Color backgroundColor, Sprite iconSprite, Color iconColor, string tooltipMessage, DeleteCondition deleteCondition)
            {
                this.startIndex = startIndex;
                this.endIndex = endIndex;
                this.backgroundSprite = backgroundSprite;
                this.backgroundColor = backgroundColor;
                this.iconSprite = iconSprite;
                this.iconColor = iconColor;
                this.tooltipMessage = tooltipMessage;
                this.deleteCondition = deleteCondition;
            }

            /// <summary>
            /// Makes the label visible or invisible.
            /// </summary>
            /// <param name="visible">Visible if set to <c>true</c>.</param>
            public void SetVisible(bool visible)
            {
                foreach (GameObject labelRect in labelRects)
                    labelRect.SetActive(visible);
                if (icon != null)
                    icon.SetActive(visible);
            }

            /// <summary>
            /// Destroys this label and all of its game objects.
            /// </summary>
            public void Destroy()
            {
                foreach (GameObject labelRect in labelRects)
                {
                    labelRect.SetActive(false);
                    Object.Destroy(labelRect);
                }

                if (icon != null)
                    Object.Destroy(icon);
            }
        }

        // text block rendering part of the line's text
        class TextBlock
        {
            // text line
            public readonly Line line;

            // start index within this line's text
            public readonly int startIndex;

            // end index within this line's text
            public readonly int endIndex;

            // offset relative to line's offset
            public readonly Vector2 offset;

            // text to be displayed
            public readonly string text;

            // game object rendering the text
            public GameObject gameObject = null;

            // create new text block
            public TextBlock(Line line, int startIndex, int endIndex, Vector2 offset, string text)
            {
                this.line = line;
                this.startIndex = startIndex;
                this.endIndex = endIndex;
                this.offset = offset;
                this.text = text;
            }

            // show or hide text block
            public void SetVisible(bool visible)
            {
                if (gameObject != null)
                    gameObject.SetActive(visible);
                else if (visible)
                {
                    gameObject = line.CreateTextBlockGameObject(this);
                    gameObject.SetActive(true);
                }
            }

            // destroy game object
            public void Destroy()
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                    Object.Destroy(gameObject);
                }
            }
        }

        // maximum number of characters that can be rendered in a single UI.Text component
        // if the text is too long, it will be split up into multiple blocks
        readonly int maxCharactersPerBlock = 10000;

        // line number
        int lineNumber;

        // text to be displayed
        string text;

        // vertical offset
        float verticalOffset;

        // text editor to which this line belongs
        TextEditor textEditor;

        // text blocks
        List<TextBlock> textBlocks = new List<TextBlock>();

        // game object with text component to display line number
        GameObject lineNumberGameObject;

        // labels
        List<Label> labels = new List<Label>();

        // line fragments (if a line is wrapped it consists of multiple line fragments)
        List<string> lineFragments = new List<string>();

        // the number of characters this line is indented
        int lineIndent = 0;

        // the number of characters the wrapped line segments are indented
        int wrappedLineIndent = 0;

        // contains the width of the given character
        List<float> characterWidth = new List<float>();

        // contains the offset of the given character
        List<Vector2> characterOffset = new List<Vector2>();

        // maps index of the rendered character to index of the original character
        List<int> indexMap = new List<int>();

        // maps index of the character to start index of the corresponding rendered character(s)
        List<int> reverseStartIndexMap = new List<int>();

        // maps index of the character to end index of the corresponding rendered character(s)
        List<int> reverseEndIndexMap = new List<int>();

        // the width of the entire line
        float lineWidth = 0f;

        // text format groups
        List<TextFormatGroup> textFormat = new List<TextFormatGroup>();

        // text format groups with indices adjusted to match indices of rendered characters
        List<TextFormatGroup> adjustedTextFormat = new List<TextFormatGroup>();

        // indicates whether this line is visible or not
        bool visible = false;

        // properties of this line
        Dictionary<string, object> properties = new Dictionary<string, object>();

        /// <summary>
        /// Creates a new line with a given line number and text.
        /// </summary>
        /// <param name="lineNumber">Line number.</param>
        /// <param name="text">Text displayed in this line.</param>
        /// <param name="verticalOffset">This line's vertical offset from the
        /// top of the text field</param>
        /// <param name="textEditor">Reference to text editor to which this line
        /// belongs.</param>
        public Line(int lineNumber, string text, float verticalOffset, TextEditor textEditor)
        {
            this.lineNumber = lineNumber;
            this.text = text;
            this.verticalOffset = verticalOffset;
            this.textEditor = textEditor;

            // create test blocks
            CreateTextBlocks();
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>The line number.</value>
        public int LineNumber
        {
            get { return lineNumber; }
            set
            {
                if (value != lineNumber)
                {
                    lineNumber = value;

                    // update line number text
                    if (lineNumberGameObject != null)
                        lineNumberGameObject.GetComponent<Text>().text = (lineNumber + 1).ToString();
                }
            }
        }

        public List<Label> Labels
        {
            get { return labels; }
        }

        /// <summary>
        /// Gets or sets the text displayed in this line.
        /// Setting a new text will reinstantiate all text blocks required to
        /// render the text and reset the text format.
        /// </summary>
        /// <value>The text displayed in this line.</value>
        public string Text
        {
            get { return text; }
            set
            {
                bool textChanged = !text.Equals(value);

                text = value;
                textFormat.Clear();
                CreateTextBlocks();

                // update line number text
                if (lineNumberGameObject != null)
                {
                    Object.Destroy(lineNumberGameObject);
                    if (visible)
                    {
                        lineNumberGameObject = CreateLineNumberGameObject();
                        lineNumberGameObject.SetActive(true);
                    }
                    else
                        lineNumberGameObject = null;
                }

                // redo labels
                RedoLabels(textChanged, false, false);

                // notify text editor about change
                textEditor.OnLineChanged(this);
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset.
        /// Setting a new vertical offset will move all text blocks.
        /// </summary>
        /// <value>The vertical offset of this line.</value>
        public float VerticalOffset
        {
            get { return verticalOffset; }
            set
            {
                if (!Mathf.Approximately(verticalOffset, value))
                {
                    float oldVerticalOffset = verticalOffset;
                    verticalOffset = value;

                    // move line number
                    if (lineNumberGameObject != null)
                        lineNumberGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(lineNumberGameObject.GetComponent<RectTransform>().anchoredPosition.x, -textEditor.mainMarginTop + verticalOffset);

                    // move main text blocks
                    foreach (TextBlock textBlock in textBlocks)
                    {
                        if (textBlock.gameObject != null)
                        {
                            Vector2 blockOffset = textBlock.offset;
                            textBlock.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(textBlock.gameObject.GetComponent<RectTransform>().anchoredPosition.x, -textEditor.mainMarginTop + verticalOffset + blockOffset.y);
                        }
                    }

                    // move labels
                    foreach (Label label in labels)
                    {
                        foreach (GameObject labelRect in label.labelRects)
                        {
                            Vector2 labelOffset = labelRect.GetComponent<RectTransform>().anchoredPosition;
                            labelRect.GetComponent<RectTransform>().anchoredPosition = new Vector2(labelOffset.x, labelOffset.y - oldVerticalOffset + verticalOffset);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The line width.
        /// </summary>
        /// <value>The total line width.</value>
        public float Width
        {
            get { return lineWidth; }
        }

        /// <summary>
        /// The line height.
        /// </summary>
        /// <value>The total line height.</value>
        public float Height
        {
            get { return lineFragments.Count * textEditor.CharacterHeight; }
        }

        /// <summary>
        /// Shows or hides this line.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (value != visible)
                {
                    visible = value;

                    // update text block visibility
                    foreach (TextBlock textBock in textBlocks)
                        textBock.SetVisible(visible);

                    // update label visibility
                    foreach (Label label in labels)
                        label.SetVisible(visible);

                    // update line number visibility
                    if (lineNumberGameObject != null)
                        lineNumberGameObject.SetActive(visible);
                    else if (visible)
                    {
                        lineNumberGameObject = CreateLineNumberGameObject();
                        lineNumberGameObject.SetActive(true);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the previous line.
        /// </summary>
        /// <value>The previous line or <c>null</c> if this is the first line.
        /// </value>
        public Line PreviousLine
        {
            get
            {
                if (lineNumber > 0)
                    return textEditor.Lines[lineNumber - 1];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the next line
        /// </summary>
        /// <value>The next line or <c>null</c> if this is the last line.
        /// </value>
        public Line NextLine
        {
            get
            {
                if (lineNumber < textEditor.Lines.Count - 1)
                    return textEditor.Lines[lineNumber + 1];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets this line's indent in number of characters.
        /// </summary>
        /// <value>The line indent.</value>
        public int LineIndent
        {
            get { return lineIndent; }
        }

        /// <summary>
        /// Sets a property with a given name and value.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        /// <typeparam name="T">Property type.</typeparam>
        public void SetProperty<T>(string name, T value)
        {
            if (properties.ContainsKey(name))
                properties[name] = value;
            else
                properties.Add(name, value);
        }

        /// <summary>
        /// Gets a property value for a given name.
        /// </summary>
        /// <returns>Property value.</returns>
        /// <param name="name">Property name.</param>
        /// <param name="defaultValue">Default value returned in case no
        /// property with the given name and type exists.</param>
        /// <typeparam name="T">Property type.</typeparam>
        public T GetProperty<T>(string name, T defaultValue)
        {
            object value;
            if (properties.TryGetValue(name, out value) && value is T)
                return (T) value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Returns a text position for the given mouse coordinates.
        /// </summary>
        /// <returns>The text position.</returns>
        /// <param name="coordinates">Mouse coordinates.</param>
        public TextPosition GetTextPosition(Vector2 coordinates)
        {
            // create new text position with this line number
            TextPosition textPosition = new TextPosition(lineNumber, 0);

            if (lineFragments.Count == 1)
            {
                // find col
                textPosition.colIndex = -1;
                for (int i = 0; i < text.Length; i++)
                {
                    float width = textEditor.CharacterWidth;
                    if (coordinates.x <= characterOffset[i].x + characterWidth[i] / 2f)
                    {
                        textPosition.colIndex = i;
                        break;
                    }
                }

                if (textPosition.colIndex == -1)
                    textPosition.colIndex = text.Length;
            }
            else
            {
                // find line fragment
                int lineFragmentIndex = -1;
                float tmpOffset = verticalOffset;
                for (int i = 0; i < lineFragments.Count; i++)
                {
                    float height = textEditor.CharacterHeight;
                    if (coordinates.y >= tmpOffset - height)
                    {
                        lineFragmentIndex = i;
                        break;
                    }

                    tmpOffset -= height;
                }

                if (lineFragmentIndex == -1)
                    lineFragmentIndex = lineFragments.Count - 1;

                // within line fragment, find col
                int colsBeforeLineFragment = 0;
                for (int i = 0; i < lineFragmentIndex; i++)
                    colsBeforeLineFragment += lineFragments[i].Length;
                textPosition.colIndex = -1;
                for (int i = colsBeforeLineFragment; i < colsBeforeLineFragment + lineFragments[lineFragmentIndex].Length; i++)
                {
                    if (coordinates.x <= characterOffset[i].x + characterWidth[i] / 2f)
                    {
                        textPosition.colIndex = i;
                        break;
                    }
                }

                if (textPosition.colIndex == -1)
                    textPosition.colIndex = colsBeforeLineFragment + lineFragments[lineFragmentIndex].Length;
                if (textPosition.colIndex == colsBeforeLineFragment && lineFragmentIndex > 0)
                    textPosition.preferNextLine = true;
            }

            return textPosition;
        }

        /// <summary>
        /// Given a text position, finds the position of the next word start.
        /// </summary>
        /// <returns>The next word start position.</returns>
        /// <param name="textPosition">Text position.</param>
        /// <param name="forward">If set to <c>true</c> search forward,
        /// otherwise search backward.</param>
        public TextPosition FindWordStart(TextPosition textPosition, bool forward)
        {
            TextPosition nextTextPosition = null;

            if (forward)
            {
                int nextColIndex = -1;
                for (int i = Mathf.Min(textPosition.colIndex, text.Length - 1); i < text.Length - 1; i++)
                {
                    if (textEditor.WordDelimiters.Contains(text[i]) && !textEditor.WordDelimiters.Contains(text[i + 1]))
                    {
                        nextColIndex = i + 1;
                        break;
                    }
                }

                if (nextColIndex == -1)
                    nextColIndex = text.Length;
                nextTextPosition = new TextPosition(lineNumber, nextColIndex);
            }
            else
            {
                int nextColIndex = -1;
                for (int i = Mathf.Min(textPosition.colIndex, text.Length - 1); i > 0; i--)
                {
                    if (!textEditor.WordDelimiters.Contains(text[i]) && textEditor.WordDelimiters.Contains(text[i - 1]))
                    {
                        nextColIndex = i;
                        break;
                    }
                }

                if (nextColIndex == -1)
                    nextColIndex = 0;
                nextTextPosition = new TextPosition(lineNumber, nextColIndex, true);
            }

            return nextTextPosition;
        }

        /// <summary>
        /// Given a text position, finds the position of the next word end.
        /// </summary>
        /// <returns>The next word end position.</returns>
        /// <param name="textPosition">Text position.</param>
        /// <param name="forward">If set to <c>true</c> search forward,
        /// otherwise search backward.</param>
        public TextPosition FindWordEnd(TextPosition textPosition, bool forward)
        {
            TextPosition nextTextPosition = null;

            if (forward)
            {
                int nextColIndex = -1;
                for (int i = Mathf.Min(textPosition.colIndex, text.Length - 1); i < text.Length - 1; i++)
                {
                    if (!textEditor.WordDelimiters.Contains(text[i]) && textEditor.WordDelimiters.Contains(text[i + 1]))
                    {
                        nextColIndex = i + 1;
                        break;
                    }
                }

                if (nextColIndex == -1)
                    nextColIndex = text.Length;
                nextTextPosition = new TextPosition(lineNumber, nextColIndex);
            }
            else
            {
                int nextColIndex = -1;
                for (int i = Mathf.Min(textPosition.colIndex, text.Length - 1); i > 0; i--)
                {
                    if (textEditor.WordDelimiters.Contains(text[i]) && !textEditor.WordDelimiters.Contains(text[i - 1]))
                    {
                        nextColIndex = i;
                        break;
                    }
                }

                if (nextColIndex == -1)
                    nextColIndex = 0;
                nextTextPosition = new TextPosition(lineNumber, nextColIndex, true);
            }

            return nextTextPosition;
        }

        /// <summary>
        /// Given a text position, finds the position of the next word start or
        /// end.
        /// </summary>
        /// <returns>The next word start or end position.</returns>
        /// <param name="textPosition">Text position.</param>
        /// <param name="forward">If set to <c>true</c> search forward,
        /// otherwise search backward.</param>
        public TextPosition FindWordStartOrEnd(TextPosition textPosition, bool forward)
        {
            TextPosition wordStart = FindWordStart(textPosition, forward);
            TextPosition wordEnd = FindWordEnd(textPosition, forward);

            if (wordStart.colIndex <= wordEnd.colIndex)
                return forward ? wordStart : wordEnd;
            else
                return forward ? wordEnd : wordStart;
        }

        /// <summary>
        /// Returns the caret position for a given text position.
        /// </summary>
        /// <returns>The caret position.</returns>
        /// <param name="textPosition">Text position.</param>
        public Vector2 GetCaretPosition(TextPosition textPosition)
        {
            // check line index
            if (textPosition.lineIndex != lineNumber)
                throw new UnityException("The given text position does not belong to this line.");
            else if (textPosition.colIndex < 0 || textPosition.colIndex > text.Length)
                throw new UnityException("Invalid col index: " + textPosition.colIndex);

            if (textPosition.colIndex == 0)
                return new Vector2(0f, verticalOffset);
            else if (textPosition.colIndex == text.Length)
                return new Vector2(characterOffset[textPosition.colIndex - 1].x + characterWidth[textPosition.colIndex - 1], verticalOffset + characterOffset[textPosition.colIndex - 1].y);
            else if (lineFragments.Count == 1)
                return new Vector2(characterOffset[textPosition.colIndex].x, verticalOffset + characterOffset[textPosition.colIndex].y);
            else
            {
                // find col in line fragment
                int lineFragmentIndex = 0;
                int colInLineFragment = textPosition.colIndex;
                while (colInLineFragment > lineFragments[lineFragmentIndex].Length)
                {
                    colInLineFragment -= lineFragments[lineFragmentIndex].Length;
                    lineFragmentIndex++;
                }

                if (colInLineFragment == lineFragments[lineFragmentIndex].Length && !textPosition.preferNextLine)
                    return new Vector2(characterOffset[textPosition.colIndex - 1].x + characterWidth[textPosition.colIndex - 1], verticalOffset + characterOffset[textPosition.colIndex - 1].y);
                else
                    return new Vector2(characterOffset[textPosition.colIndex].x, verticalOffset + characterOffset[textPosition.colIndex].y);
            }
        }

        /// <summary>
        /// Adds a label to this line.
        /// </summary>
        /// <param name="startColIndex">Start column index. For a label that
        /// annotates the entire line, set this to 0.</param>
        /// <param name="endColIndex">End column index. For a label that
        /// annotates the enitre line, set this to <c> int.MaxValue</c></param>
        /// <param name="backgroundSprite">Background sprite. If set to
        /// <c>null</c>, a rectangle filled with <c>backgroundColor</c> will be
        /// used to display this label.</param>
        /// <param name="backgroundColor">Background color. Multiplied with
        /// <c>backgroundSprite</c> (if defined), otherwise used to color the
        /// rectangle(s) displaying this label.</param>
        /// <param name="iconSprite">Icon sprite. If set to <c>null</c>, no icon
        /// will be displayed for this label</param>
        /// <param name="iconColor">Icon color. Multiplied with
        /// <c>iconSprite</c> (if defined).</param>
        /// <param name="tooltipMessage">The tooltip message. If set to
        /// <c>null</c>, no tooltip will be displayed for this label.</param>
        /// <param name="deleteCondition">Delete condition for this label.
        /// </param>
        public void AddLabel(int startColIndex, int endColIndex, Sprite backgroundSprite, Color backgroundColor, Sprite iconSprite, Color iconColor, string tooltipMessage, Label.DeleteCondition deleteCondition)
        {
            // clamp start and end index
            int clampedStartColIndex = Mathf.Clamp(startColIndex, 0, text.Length);
            int clampedEndColIndex = Mathf.Clamp(endColIndex, 0, text.Length);

            // create new label (with unclamped indices)
            Label label = new Label(startColIndex, endColIndex, backgroundSprite, backgroundColor, iconSprite, iconColor, tooltipMessage, deleteCondition);
            labels.Add(label);

            // only create label if indices span a valid range
            if (startColIndex < endColIndex)
            {
                // calculage top-left and bottom-right corner of label
                Vector2 startCaretPosition = GetCaretPosition(new TextPosition(lineNumber, clampedStartColIndex, true));
                Vector2 endCaretPosition = GetCaretPosition(new TextPosition(lineNumber, clampedEndColIndex));

                // calculate number of lines
                int numberOfLines = 1 + Mathf.RoundToInt((endCaretPosition.y - startCaretPosition.y) / textEditor.CharacterHeight);

                if (numberOfLines == 1)
                {
                    // add single rectangle
                    GameObject labelRect = AddLabelRect(startCaretPosition, endCaretPosition - new Vector2(0f, textEditor.CharacterHeight / textEditor.lineSpacing), backgroundSprite, backgroundColor);
                    label.labelRects.Add(labelRect);
                }
                else
                {
                    // calculate multiple rectangles
                    float maxLabelWidth = Mathf.Max(Mathf.FloorToInt(textEditor.HorizontalSpaceAvailable / textEditor.CharacterWidth) * textEditor.CharacterWidth, textEditor.LongestLineWidth);

                    // first line
                    GameObject firstLabelRect = AddLabelRect(startCaretPosition, new Vector2(maxLabelWidth, startCaretPosition.y - textEditor.CharacterHeight / textEditor.lineSpacing), backgroundSprite, backgroundColor);
                    label.labelRects.Add(firstLabelRect);

                    // intermediate lines
                    if (!Mathf.Approximately(startCaretPosition.y - textEditor.CharacterHeight, endCaretPosition.y))
                    {
                        GameObject intermediateLabelRect = AddLabelRect(new Vector2(0f, startCaretPosition.y - textEditor.CharacterHeight / textEditor.lineSpacing), new Vector2(maxLabelWidth, endCaretPosition.y), backgroundSprite, backgroundColor);
                        label.labelRects.Add(intermediateLabelRect);
                    }

                    // last line
                    GameObject lastLabelRect = AddLabelRect(new Vector2(0f, endCaretPosition.y), endCaretPosition - new Vector2(0f, textEditor.CharacterHeight / textEditor.lineSpacing), backgroundSprite, backgroundColor);
                    label.labelRects.Add(lastLabelRect);
                }
            }

            // add icon sprite
            if (iconSprite != null)
            {
                Vector2 startCaretPosition = GetCaretPosition(new TextPosition(lineNumber, clampedStartColIndex, true));
                label.icon = AddLabelIcon(iconSprite, iconColor, startCaretPosition.y);
            }
        }

        /// <summary>
        /// Removes all labels attached to this line.
        /// </summary>
        public void RemoveLabels()
        {
            foreach (Label label in labels)
                label.Destroy();
            labels.Clear();
        }

        // removes and redoes all labels attached to this line.
        void RedoLabels(bool thisLineChanged, bool previousLineChanged, bool nextLineChanged)
        {
            int labelCount = labels.Count;
            for (int i = 0; i < labelCount; i++)
            {
                Label label = labels[0];
                int startIndex = label.startIndex;
                int endIndex = label.endIndex;
                Sprite backgroundSprite = label.backgroundSprite;
                Color backgroundColor = label.backgroundColor;
                Sprite iconSprite = label.iconSprite;
                Color iconColor = label.iconColor;
                string message = label.tooltipMessage;
                Label.DeleteCondition deleteCondition = label.deleteCondition;

                bool deleteLabel = false;
                deleteLabel |= thisLineChanged && deleteCondition != Label.DeleteCondition.NONE;
                deleteLabel |= previousLineChanged && (deleteCondition == Label.DeleteCondition.PREVIOUS_LINE_CHANGES || deleteCondition == Label.DeleteCondition.ANYTHING_CHANGES);
                deleteLabel |= nextLineChanged && deleteCondition == Label.DeleteCondition.ANYTHING_CHANGES;

                // add new label
                if (!deleteLabel)
                    AddLabel(startIndex, endIndex, backgroundSprite, backgroundColor, iconSprite, iconColor, message, deleteCondition);

                // remove old label
                label.Destroy();
                labels.RemoveAt(0);
            }
        }

        // creates a new label rect from topLeft to bottomRight using the defined background and color
        GameObject AddLabelRect(Vector2 topLeft, Vector2 bottomRight, Sprite sprite, Color color)
        {
            // create label rect
            GameObject labelRect = new GameObject("Label");
            labelRect.transform.SetParent(textEditor.labelContainer);
            labelRect.AddComponent<Image>();
            labelRect.GetComponent<Image>().color = color;

            if (sprite != null)
            {
                labelRect.GetComponent<Image>().sprite = sprite;
                labelRect.GetComponent<Image>().type = Image.Type.Tiled;
            }

            labelRect.GetComponent<RectTransform>().localPosition = Vector3.zero;
            labelRect.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            labelRect.GetComponent<RectTransform>().localScale = Vector3.one;
            labelRect.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            labelRect.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            labelRect.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            labelRect.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            labelRect.GetComponent<RectTransform>().anchoredPosition = new Vector2(textEditor.mainMarginLeft + topLeft.x, -textEditor.mainMarginTop + topLeft.y);

            if (sprite != null)
            {
                // adapt size and scale based on sprite resolution
                float width = bottomRight.x - topLeft.x;
                float height = topLeft.y - bottomRight.y;
                float scale = height / sprite.texture.height;
                labelRect.GetComponent<RectTransform>().sizeDelta = new Vector2(width / scale, height / scale);
                labelRect.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1f);
            }
            else
                labelRect.GetComponent<RectTransform>().sizeDelta = new Vector2(bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);

            labelRect.transform.SetAsLastSibling();
            return labelRect;
        }

        // creates new game object displaying the label's icon
        GameObject AddLabelIcon(Sprite iconSprite, Color iconColor, float positionY)
        {
            // create icon
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(textEditor.lineLabelIconsContent);
            icon.AddComponent<Image>();
            icon.GetComponent<Image>().color = iconColor;
            icon.GetComponent<Image>().sprite = iconSprite;
            icon.GetComponent<Image>().type = Image.Type.Simple;
            icon.GetComponent<RectTransform>().localPosition = Vector3.zero;
            icon.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            icon.GetComponent<RectTransform>().localScale = Vector3.one;
            icon.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            icon.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            icon.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -textEditor.mainMarginTop + positionY);

            // adapt scale based on icon resolution
            float scale = (textEditor.CharacterHeight / textEditor.lineSpacing) / iconSprite.texture.height;
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(iconSprite.texture.width, iconSprite.texture.height);
            icon.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1f);
            icon.transform.SetAsFirstSibling();

            return icon;
        }

        /// <summary>
        /// Applies the given text format to this text line. 
        /// </summary>
        /// <param name="textFormatGroups">List of text format groups to be
        /// applied.</param>
        public void ApplyTextFormat(List<TextFormatGroup> textFormatGroups)
        {
            textFormat.Clear();
            adjustedTextFormat.Clear();
            textFormat.AddRange(textFormatGroups);
            textFormat.Sort(TextFormatGroup.Sort);
            for (int i = 0; i < textFormat.Count; i++)
            {
                if (textFormat[i].startIndex < 0 || textFormat[i].endIndex < 0)
                    throw new UnityException("Invalid index");
                if (i < textFormat.Count - 1 && textFormat[i].endIndex >= textFormat[i + 1].startIndex)
                    throw new UnityException("Overlapping text format groups");

                int adjustedStartIndex = textFormat[i].startIndex < text.Length ? reverseStartIndexMap[textFormat[i].startIndex] : int.MaxValue;
                int adjustedEndIndex = textFormat[i].endIndex < text.Length ? reverseEndIndexMap[textFormat[i].endIndex] : int.MaxValue;
                adjustedTextFormat.Add(new TextFormatGroup(adjustedStartIndex, adjustedEndIndex, textFormat[i].textStyle));
            }

            CreateTextBlocks();
        }

        public void OnPreviousLineChanged()
        {
            if (labels.Count > 0)
                RedoLabels(false, true, false);
        }

        public void OnNextLineChanged()
        {
            if (labels.Count > 0)
                RedoLabels(false, false, true);
        }

        /// <summary>
        /// Destroys this instance and all of its game objects.
        /// </summary>
        public void Destroy()
        {
            foreach (TextBlock textBlock in textBlocks)
                textBlock.Destroy();
            foreach (Label label in labels)
                label.Destroy();
            if (lineNumberGameObject != null)
            {
                lineNumberGameObject.SetActive(false);
                Object.Destroy(lineNumberGameObject);
            }
        }

        // creates all text blocks required to render this line's text.
        void CreateTextBlocks()
        {
            // remove old blocks
            foreach (TextBlock textBlock in textBlocks)
                textBlock.Destroy();
            textBlocks.Clear();
            lineFragments.Clear();
            characterWidth.Clear();
            characterOffset.Clear();
            indexMap.Clear();
            reverseStartIndexMap.Clear();
            reverseEndIndexMap.Clear();
            lineWidth = 0f;
            visible = false;

            // determine wrapped line indent
            lineIndent = 0;
            if (text.Length > 0)
            {
                if (text[0] == ' ')
                {
                    for (int i = 0; i < text.Length && text[i] == ' '; i++)
                        lineIndent++;
                }

                if (text[0] == '\t')
                {
                    for (int i = 0; i < text.Length && text[i] == '\t'; i++)
                        lineIndent += textEditor.tabStopWidth;
                }
            }

            // do not indent wrapped line segments if there is not enough space for at least one tab stop and one character
            wrappedLineIndent = lineIndent;
            if (wrappedLineIndent >= Mathf.FloorToInt(textEditor.HorizontalSpaceAvailable / textEditor.CharacterWidth) - textEditor.tabStopWidth)
                wrappedLineIndent = 0;

            // set minimum available space so that at least one tab stop and one character fit on a line fragment
            float horizontalSpaceAvailable = Mathf.Max(textEditor.HorizontalSpaceAvailable, (textEditor.tabStopWidth + wrappedLineIndent + 1) * textEditor.CharacterWidth);

            if (text.Length > 0)
            {
                // find separate words
                HashSet<int> wordBreaks = new HashSet<int>();
                for (int i = 0; i < text.Length; i++)
                {
                    // check for invalid characters
                    if (text[i] != ' ' && text[i] != '\t' && !Util.IsPrintableCharacter(text[i]))
                        throw new UnityException("Invalid character: (Unicode " + string.Format("0x{0:X4}", ((int) text[i])) + ")");

                    if (i > 0 && (text[i - 1] == ' ' || text[i - 1] == '\t') && !(text[i] == ' ' || text[i] == '\t'))
                        wordBreaks.Add(i);
                    else if (i < text.Length - 1 && (textEditor.WordDelimiters.Contains(text[i]) && text[i] != ' ' && text[i] != '\t') && (!textEditor.WordDelimiters.Contains(text[i + 1]) && text[i + 1] != ' ' && text[i + 1] != '\t'))
                        wordBreaks.Add(i);
                }

                // hash set with indices where a new line fragment starts
                HashSet<int> lineFragmentBreaks = new HashSet<int>();

                // handle line wrapping
                if (textEditor.wrapLines)
                {
                    // iterate over all characters in text
                    string tmpRenderedLineFragment = "";
                    int lastWordBreak = -1;
                    int wordsOnCurrentLineFragment = 0;
                    bool firstLineFragment = true;
                    for (int i = 0; i < text.Length; i++)
                    {
                        // replace tabs
                        string renderedCharacter = Util.ReplaceTabsWithSpaces(text[i].ToString(), textEditor.tabStopWidth, tmpRenderedLineFragment.Length + (firstLineFragment ? 0 : wrappedLineIndent));

                        // handle word breaks
                        if (wordBreaks.Contains(i))
                        {
                            lastWordBreak = i;
                            if (tmpRenderedLineFragment.Length > 0)
                                wordsOnCurrentLineFragment++;
                        }

                        // try to add character to current line fragment
                        if ((tmpRenderedLineFragment.Length + renderedCharacter.Length + (firstLineFragment ? 0 : wrappedLineIndent)) * textEditor.CharacterWidth <= horizontalSpaceAvailable)
                        {
                            // add character to current line fragment
                            tmpRenderedLineFragment += renderedCharacter;
                        }
                        else if (wordsOnCurrentLineFragment > 0)
                        {
                            // add break at word start
                            lineFragmentBreaks.Add(lastWordBreak);

                            // reset current line fragment
                            tmpRenderedLineFragment = "";
                            wordsOnCurrentLineFragment = 0;
                            firstLineFragment = false;

                            // repeat word in next iteration
                            i = lastWordBreak - 1;
                        }
                        else
                        {
                            // add break before this character
                            lineFragmentBreaks.Add(i);

                            // reset current line fragment
                            tmpRenderedLineFragment = "";
                            wordsOnCurrentLineFragment = 0;
                            firstLineFragment = false;

                            // repeat this character in next iteration
                            i--;
                        }
                    }
                }

                // add text
                string renderedText = "";
                string lineFragment = "";
                string renderedLineFragment = "";
                float horizontalOffset = 0f;
                for (int i = 0; i < text.Length; i++)
                {
                    // replace tabs
                    string renderedCharacter = Util.ReplaceTabsWithSpaces(text[i].ToString(), textEditor.tabStopWidth, renderedLineFragment.Length + (lineFragments.Count == 0 ? 0 : wrappedLineIndent));
                    lineFragment += text[i];
                    renderedLineFragment += renderedCharacter;

                    // update index maps
                    reverseStartIndexMap.Add(indexMap.Count);
                    for (int j = 0; j < renderedCharacter.Length; j++)
                        indexMap.Add(i);
                    reverseEndIndexMap.Add(indexMap.Count - 1);

                    // add width and offset for this character
                    characterWidth.Add(renderedCharacter.Length * textEditor.CharacterWidth);
                    characterOffset.Add(new Vector2(horizontalOffset, -lineFragments.Count * textEditor.CharacterHeight));
                    horizontalOffset += renderedCharacter.Length * textEditor.CharacterWidth;

                    // handle line fragment breaks
                    if (i == text.Length - 1 || lineFragmentBreaks.Contains(i + 1))
                    {
                        // create text blocks and update line width
                        CreateTextBlocksForLine(renderedText.Length, renderedText.Length + renderedLineFragment.Length - 1, renderedLineFragment, new Vector2((lineFragments.Count == 0 ? 0 : wrappedLineIndent) * textEditor.CharacterWidth, -lineFragments.Count * textEditor.CharacterHeight));
                        lineWidth = Mathf.Max(lineWidth, (renderedLineFragment.Length + (lineFragments.Count == 0 ? 0 : wrappedLineIndent)) * textEditor.CharacterWidth);

                        // add line fragment
                        renderedText += renderedLineFragment;
                        lineFragments.Add(lineFragment);
                        lineFragment = "";
                        renderedLineFragment = "";
                        horizontalOffset = wrappedLineIndent * textEditor.CharacterWidth;
                    }
                }
            }
            else
            {
                // add empty line fragment
                lineFragments.Add("");
            }
        }

        // creates text blocks required for rendering one line (or part of a line if wrapped)
        void CreateTextBlocksForLine(int startIndex, int endIndex, string lineText, Vector2 lineOffset)
        {
            if (lineText.Length <= maxCharactersPerBlock)
                textBlocks.Add(new TextBlock(this, startIndex, endIndex, lineOffset, lineText));
            else
            {
                string remainingText = lineText;
                Vector2 blockOffset = lineOffset;
                int textBlockStartIndex = startIndex;
                while (remainingText.Length > 0)
                {
                    string blockText = remainingText.Substring(0, Mathf.Min(maxCharactersPerBlock, remainingText.Length));

                    textBlocks.Add(new TextBlock(this, textBlockStartIndex, textBlockStartIndex + blockText.Length - 1, blockOffset, blockText));
                    textBlockStartIndex += blockText.Length;
                    remainingText = remainingText.Substring(blockText.Length);
                    blockOffset = new Vector2(blockOffset.x + blockText.Length * textEditor.CharacterWidth, blockOffset.y);
                }
            }
        }

        // creates a game object displaying the given text block
        GameObject CreateTextBlockGameObject(TextBlock textBlock)
        {
            // replace tab by white space and '<' by escape character
            string blockText = textBlock.text.Replace('\t', ' ').Replace('<', '\u001B');

            // apply text format
            string formattedText = "";
            int formattedTextIndex = 0;
            foreach (TextFormatGroup textFormatGroup in adjustedTextFormat)
            {
                if (textFormatGroup.startIndex < textBlock.startIndex && textFormatGroup.endIndex >= textBlock.startIndex)
                {
                    // first group
                    formattedText = textFormatGroup.textStyle.RichtTextOpenTag;
                }

                if (textFormatGroup.startIndex >= textBlock.startIndex && textFormatGroup.startIndex <= textBlock.endIndex)
                {
                    formattedText += blockText.Substring(formattedTextIndex, textFormatGroup.startIndex - textBlock.startIndex - formattedTextIndex) + textFormatGroup.textStyle.RichtTextOpenTag;
                    formattedTextIndex = textFormatGroup.startIndex - textBlock.startIndex;
                }

                if (textFormatGroup.endIndex >= textBlock.startIndex && textFormatGroup.endIndex <= textBlock.endIndex)
                {
                    formattedText += blockText.Substring(formattedTextIndex, textFormatGroup.endIndex - textBlock.startIndex - formattedTextIndex + 1) + textFormatGroup.textStyle.RichtTextCloseTag;
                    formattedTextIndex = textFormatGroup.endIndex - textBlock.startIndex + 1;
                }

                if (textFormatGroup.startIndex <= textBlock.endIndex && textFormatGroup.endIndex > textBlock.endIndex)
                {
                    // last group
                    formattedText += blockText.Substring(formattedTextIndex, textBlock.endIndex - textBlock.startIndex - formattedTextIndex + 1) + textFormatGroup.textStyle.RichtTextCloseTag;
                    formattedTextIndex = textBlock.endIndex + 1;
                }
            }

            // add rest of text if not enclosed in text format group
            if (formattedTextIndex <= textBlock.endIndex)
                formattedText += blockText.Substring(formattedTextIndex, textBlock.endIndex - textBlock.startIndex - formattedTextIndex + 1);

            // replace each escape character by '<' followed by an escape character and add text to component
            blockText = formattedText.Replace("\u001B", "<\u001B");

            // create new game object and attach it to the editor's text container
            GameObject go = new GameObject("Text");
            go.transform.SetParent(textEditor.textContainer);

            // set text properties
            go.AddComponent<Text>();
            go.GetComponent<Text>().font = textEditor.font;
            go.GetComponent<Text>().fontSize = textEditor.fontSize;
            go.GetComponent<Text>().lineSpacing = textEditor.lineSpacing;
            go.GetComponent<Text>().fontStyle = textEditor.mainFontStyle;
            go.GetComponent<Text>().color = textEditor.mainFontColor;
            go.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
            go.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            go.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            go.GetComponent<Text>().text = blockText;

            // set transform properties
            go.GetComponent<RectTransform>().localPosition = Vector3.zero;
            go.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            go.GetComponent<RectTransform>().localScale = Vector3.one;
            go.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(textEditor.mainMarginLeft + textBlock.offset.x, -textEditor.mainMarginTop + verticalOffset + textBlock.offset.y);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
            go.transform.SetAsLastSibling();

            return go;
        }

        // creates a game object displaying the line number
        GameObject CreateLineNumberGameObject()
        {
            // create new game object and attach it to the editor's line number panel
            GameObject go = new GameObject("Line Number");
            go.transform.SetParent(textEditor.lineNumberContent);

            // set text properties
            go.AddComponent<Text>();
            go.GetComponent<Text>().font = textEditor.font;
            go.GetComponent<Text>().fontSize = textEditor.fontSize;
            go.GetComponent<Text>().lineSpacing = textEditor.lineSpacing;
            go.GetComponent<Text>().fontStyle = textEditor.lineNumberFontStyle;
            go.GetComponent<Text>().color = textEditor.lineNumberFontColor;
            go.GetComponent<Text>().alignment = TextAnchor.UpperRight;
            go.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            go.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            go.GetComponent<Text>().text = (lineNumber + 1).ToString();

            // set transform properties
            go.GetComponent<RectTransform>().localPosition = Vector3.zero;
            go.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            go.GetComponent<RectTransform>().localScale = Vector3.one;
            go.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-textEditor.lineNumberMarginRight, -textEditor.mainMarginTop + verticalOffset);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
            go.transform.SetAsLastSibling();

            return go;
        }
    }
}