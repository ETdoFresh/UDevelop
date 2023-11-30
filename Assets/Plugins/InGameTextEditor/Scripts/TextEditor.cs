using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InGameTextEditor.Format;
using InGameTextEditor.History;
using InGameTextEditor.Operations;

namespace InGameTextEditor
{
    /// <summary>
    /// The TextEditor class controls the appearance and behavior of the text
    /// editor and all of its components. The game object to which this script
    /// is attached must be, directly or indirectly, attached to a Canvas
    /// component. Operations altering the content of the editor (e.g. insert,
    /// delete, etc.) are enqueued in an operations queue and executed in
    /// parallel to the game loop in order not to freeze the application when an
    /// operation takes longer to complete. All modifications are tracked in a
    /// history which can be rolled back or forward.
    /// </summary>
    [ExecuteInEditMode]
    public class TextEditor : MonoBehaviour
    {
        [Header("Text")]
        /// <summary>
        /// Font used for displaying the text in the content panel as well as
        /// the line numbers. Must be mono-spaced font.
        /// </summary>
        public Font font;

        /// <summary>
        /// Font size used for text and line numbers. Must be 1 or greater.
        /// </summary>
        public int fontSize = 12;

        /// <summary>
        /// Line spacing in text and line numbers. Must be 1.0 or greater.
        /// </summary>
        public float lineSpacing = 1f;

        /// <summary>
        /// Font style used for text.
        /// </summary>
        public FontStyle mainFontStyle = FontStyle.Normal;

        /// <summary>
        /// Color used for text.
        /// </summary>
        public Color mainFontColor = new Color(0f, 0f, 0f);

        /// <summary>
        /// Background color in main panel.
        /// </summary>
        public Color mainBackgroundColor = new Color(1f, 1f, 1f, 0.7f);

        /// <summary>
        /// Left margin in main panel. Must be 0.0 or greater.
        /// </summary>
        public float mainMarginLeft = 5f;

        /// <summary>
        /// Right margin in main panel. Must be 0.0 or greater.
        /// </summary>
        public float mainMarginRight = 5f;

        /// <summary>
        /// Top margin in main panel. Must be 0.0 or greater.
        /// </summary>
        public float mainMarginTop = 5f;

        /// <summary>
        /// Bottom margin in main panel. Must be 0.0 or greater.
        /// </summary>
        public float mainMarginBottom = 5f;

        /// <summary>
        /// Default text displayed when application starts.
        /// </summary>
        [TextArea]
        public string defaultText = "";

        /// <summary>
        /// Wrap lines so there's no need for horizontal scrolling.
        /// </summary>
        public bool wrapLines = false;

        /// <summary>
        /// Indent new lines with the same indent as the previous line.
        /// </summary>
        public bool indentNewLines = true;

        [Header("Line Numbers")]
        /// <summary>
        /// Show line numbers.
        /// </summary>
        public bool showLineNumbers = true;

        /// <summary>
        /// Font style used for line numbers.
        /// </summary>
        public FontStyle lineNumberFontStyle = FontStyle.Normal;

        /// <summary>
        /// Color used for line numbers.
        /// </summary>
        public Color lineNumberFontColor = new Color(0.3f, 0.3f, 0.3f);

        /// <summary>
        /// Background color in line numbers panel.
        /// </summary>
        public Color lineNumberBackgroundColor = new Color(0f, 0f, 0f, 0.2f);

        /// <summary>
        /// Minimum width of line numbers panel. Must be 0.0 or greater.
        /// </summary>
        public float lineNumberMinWidth = 40f;

        /// <summary>
        /// Left margin in line numbers panel. Must be 0.0 or greater.
        /// </summary>
        public float lineNumberMarginLeft = 5f;

        /// <summary>
        /// Right margin in line numbers panel. Must be 0.0 or greater.
        /// </summary>
        public float lineNumberMarginRight = 5f;

        [Header("Line Label Icons")]
        /// <summary>
        /// Show line label icons.
        /// </summary>
        public bool showLineLabelIcons = false;

        /// <summary>
        /// Background color in line label icons panel.
        /// </summary>
        public Color lineLabelIconsBackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);

        /// <summary>
        /// Line label icons panel width
        /// </summary>
        public float lineLabelIconsWidth = 20f;

        [Header("Tooltip")]
        /// <summary>
        /// The tooltip font.
        /// </summary>
        public Font tooltipFont;

        /// <summary>
        /// The tooltip font size.
        /// </summary>
        public int tooltipFontSize = 12;

        /// <summary>
        /// The tooltip line spacing.
        /// </summary>
        public float tooltipLineSpacing = 1f;

        /// <summary>
        /// The tooltip font style.
        /// </summary>
        public FontStyle tooltipFontStyle = FontStyle.Normal;

        /// <summary>
        /// The tooltip font color.
        /// </summary>
        public Color tooltipFontColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        /// <summary>
        /// The tooltip background color.
        /// </summary>
        public Color tooltipBackgroundColor = new Color(0f, 0f, 0f, 0.8f);

        /// <summary>
        /// The left margin of the tooltip.
        /// </summary>
        public float tooltipMarginLeft = 8f;

        /// <summary>
        /// The right margin of the tooltip.
        /// </summary>
        public float tooltipMarginRight = 8f;

        /// <summary>
        /// The top margin of the tooltip.
        /// </summary>
        public float tooltipMarginTop = 8f;

        /// <summary>
        /// The bottom margin of the tooltip.
        /// </summary>
        public float tooltipMarginBottom = 8f;

        /// <summary>
        /// If the tooltip is shown above the cursor, its bottom end will be
        /// placed <c>tooltipAboveCursor</c> units above the cursor.
        /// </summary>
        public float tooltipAboveCursor = 3f;

        /// <summary>
        /// If the tooltip is shown below the cursor, its top end will be placed
        /// <c>tooltipBelowCursor</c> units below the cursor.
        /// </summary>
        public float tooltipBelowCursor = 15f;

        /// <summary>
        /// The tooltip will start to appear <c>tooltipDelay</c> seconds after
        /// it has been activated.
        /// </summary>
        public float tooltipDelay = 0.2f;

        /// <summary>
        /// The tooltip will fade in over the duration of
        /// <c>tooltipFadeDuration</c> seconds. Set this to 0.0 if the tooltip
        /// should appear immediately.
        /// </summary>
        public float tooltipFadeDuration = 0.05f;

        [Header("Behavior")]
        /// <summary>
        /// Disable input. If <c>true</c>, all operations altering the editor's
        /// content are disabled.
        /// </summary>
        public bool disableInput = false;

        /// <summary>
        /// Deactivate editor when application loses focus.
        /// </summary>
        public bool deactivateOnLostApplicationFocus = true;

        /// <summary>
        /// Deactivate editor when mouse click outside of editor occurs.
        /// </summary>
        public bool deactivateOnClickOutsideOfEditor = false;

        /// <summary>
        /// Update editor layout <c>resizeTimeout</c> seconds after the editor
        /// window has been resized. A value of 0 results in an immediate
        /// update. 
        /// </summary>
        public float resizeTimeout = 0.1f;

        /// <summary>
        /// Show lock mask when editor is busy.
        /// </summary>
        public bool showLockMask = true;

        [Header("Scrolling")]
        /// <summary>
        /// Show vertical scrollbar.
        /// </summary>
        public bool showVerticalScrollbar = true;

        /// <summary>
        /// Show horizontal scrollbar.
        /// </summary>
        public bool showHorizontalScrollbar = true;

        /// <summary>
        /// Width of the scrollbars. Must be 0.0 or greater.
        /// </summary>
        public float scrollbarWidth = 20f;

        /// <summary>
        /// Vertical scroll speed when using scrollwheel or trackpad. Must be
        /// 0.0 or greater.
        /// </summary>
        public float verticalWheelScrollSpeed = 20f;

        /// <summary>
        /// Horizontal scroll speed when using scrollwheel or trackpad. Must be
        /// 0.0 or greater.
        /// </summary>
        public float horizontalWheelScrollSpeed = 20f;

        /// <summary>
        /// Vertical scroll speed when trying to select text outside of the
        /// visible area. Must be 0.0 or greater.
        /// </summary>
        public float verticalDragScrollSpeed = 5f;

        /// <summary>
        /// Horizontal scroll speed when trying to select text outside of the
        /// visible area. Must be 0.0 or greater.
        /// </summary>
        public float horizontalDragScrollSpeed = 5f;

        /// <summary>
        /// Invert vertical scroll direction.
        /// </summary>
        public bool invertVerticalScrollDirection = false;

        /// <summary>
        /// Invert horizontal scroll direction.
        /// </summary>
        public bool invertHorizontalScrollDirection = false;

        [Header("Caret")]
        /// <summary>
        /// Caret color.
        /// </summary>
        public Color caretColor = new Color(0f, 0f, 0f, 1f);

        /// <summary>
        /// Caret width. Must be 0.0 or greater.
        /// </summary>
        public float caretWidth = 1f;

        /// <summary>
        /// Caret blink rate. Caret will change its visibility every
        /// <c>(1 / caretBlinkRate)</c> seconds. Must be 0.1 or greater.
        /// </summary>
        public float caretBlinkRate = 2f;

        [Header("Selection")]
        /// <summary>
        /// Color of text selection when editor is active.
        /// </summary>
        public Color selectionActiveColor = new Color(0f, 0.7f, 1f, 0.5f);

        /// <summary>
        /// Color of text selection when editor is inactive.
        /// </summary>
        public Color selectionInactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        /// <summary>
        /// Maximum time between mouse clicks to be considered double
        /// (or triple) click. Must be 0.1 or greater.
        /// </summary>
        public float doubleClickInterval = 0.5f;

        /// <summary>
        /// List of characters delimiting words. This is used for selections of
        /// entire words (using double click or Alt key selection).
        /// The new line character <c>'\n'</c> always works as a word delimiter
        /// despite of not being part of this list. Updating this list at
        /// runtime will not affect the editor's behavior as the list is
        /// converted to a hash set on startup. If you need to adapt the list at
        /// runtime, consider accessing the hash set via <c>WordDelimiters</c>
        /// directly.
        /// </summary>
        public List<char> wordDelimiters = new List<char>(new char[] { ' ', '\t', '.', ',', ':', ';', '!', '?', '_', '+', '-', '*', '/', '%', '=', '@', '|', '(', ')', '[', ']', '{', '}', '<', '>', '"' });

        [Header("Tab Stops")]
        /// <summary>
        /// Width of a tab stop. Must be 1 or greater.
        /// </summary>
        public int tabStopWidth = 4;

        /// <summary>
        /// Instead of a tab character, use <c>tabStopWidth</c> white spaces.
        /// </summary>
        public bool replaceTabsBySpaces = false;

        [Header("History")]
        /// <summary>
        /// Enable history (undo and redo).
        /// </summary>
        public bool enableHistory = true;

        /// <summary>
        /// The maximum length of the history. Must be 0 or greater. A value of
        /// 0 means unlimited history length.
        /// </summary>
        public int maxHistoryLength = 500;

        [Header("Keyboard")]
        /// <summary>
        /// Use default keyboard shortcuts (Ctrl+C, Ctrl+X, Ctrl+V, Ctrl+A,
        /// Ctrl+Y, Ctrl+Z).
        /// </summary>
        public bool useDefaultKeyboardShortcuts = true;

        /// <summary>
        /// Use arrow keys to move caret and extend selection.
        /// </summary>
        public bool useArrowKeys = true;

        /// <summary>
        /// Threshold (in seconds) to trigger repeated key stroke when holding
        /// down a key. Must be 0.1 or greater.
        /// </summary>
        public float keyRepeatThreshold = 0.5f;

        /// <summary>
        /// Key repeat rate. A key stroke will be repeated every
        /// <c>(1 / keyRepeatRate)</c> seconds when the key is held down for
        /// more than <c>keyRepeatThreshold</c> seconds. Must be 0.1 or greater.
        /// </summary>
        public float keyRepeatRate = 30f;

        [Header("Internal")]
        /// <summary>
        /// Main panel transform. This transform holds the mainContent transform
        /// and masks everything outside of its boundaries.
        /// </summary>
        public Transform mainPanel;

        /// <summary>
        /// Main content transform. This transform holds all text blocks, all
        /// selection blocks, all labels, and the caret. It is moved based on
        /// the vertical and horizontal scroll values.
        /// </summary>
        public Transform mainContent;

        /// <summary>
        /// The text container. This transform holds all text blocks.
        /// </summary>
        public Transform textContainer;

        /// <summary>
        /// The selection container. This transform holds all selection rects.
        /// </summary>
        public Transform selectionContainer;

        /// <summary>
        /// The label container. This transform holds all text labels.
        /// </summary>
        public Transform labelContainer;

        /// <summary>
        /// Main text. It holds the default text when in edit mode and is hidden
        /// when the application is running.
        /// </summary>
        public Text mainText;

        /// <summary>
        /// The line number panel. This transform holds the lineNumberContent
        /// transform and masks everything outside of its boundaries.
        /// </summary>
        public Transform lineNumberPanel;

        /// <summary>
        /// Line number content transform. This transform holds all line number
        /// text blocks. It is moved based on the vertical scroll value.
        /// </summary>
        public Transform lineNumberContent;

        /// <summary>
        /// The line number text. It holds the line numbers displayed in edit
        /// mode and is hidden when the applications is running.
        /// </summary>
        public Text lineNumberText;

        /// <summary>
        /// The line label icons panel. This transform holds the
        /// lineLabelIconsContent transform and masks everything outside of its
        /// boundaries.
        /// </summary>
        public Transform lineLabelIconsPanel;

        /// <summary>
        /// Line label icons content transform. This transform holds all line
        /// label icons. It is moved based on the vertical scroll value.
        /// </summary>
        public Transform lineLabelIconsContent;

        /// <summary>
        /// Caret game object. It is displayed at the caret position and
        /// displayed or hidden based on whether a text selection is active and
        /// blinks with the frequency set with <c>caretBlinkRate</c>.
        /// </summary>
        public GameObject caret;

        /// <summary>
        /// Vertical scrollbar. Hidden when <c>showVerticalScrollbar</c> is
        /// <c>false</c>.
        /// </summary>
        public Scrollbar verticalScrollbar;

        /// <summary>
        /// Vertical scrollbar. Hidden when <c>showHorizontalScrollbar</c> is
        /// <c>false</c>.
        /// </summary>
        public Scrollbar horizontalScrollbar;

        /// <summary>
        /// Lock mask. Shown on top of editor when busy.
        /// </summary>
        public GameObject lockMask;

        /// <summary>
        /// The tooltip window.
        /// </summary>
        public GameObject tooltip;

        /// <summary>
        /// The tooltip text.
        /// </summary>
        public Text tooltipText;

        // list of lines, each representing one line of text in the editor
        List<Line> lines = new List<Line>();

        // width of longest line
        float longestLineWidth = 0f;

        // vertical space required to display entire text including top and bottom margin
        float verticalSpaceRequired = 0f;

        // vertical space available (based on size of main panel)
        float verticalSpaceAvailable = 0f;

        // horizontal space required to display longest text row including left and right margin
        float horizontalSpaceRequired = 0f;

        // horizontal space available (base on size of main panel)
        float horizontalSpaceAvailable = 0f;

        // vertical offset used for vertical scrolling (mainContent is shifted verticalOffset units to the top)
        float verticalOffset = 0f;

        // horizontal offset used for horizontal scrolling (mainContent is shifted horizontalOffset units to the left)
        float horizontalOffset = 0f;

        // width (in units) of a single character
        [SerializeField]
        [HideInInspector]
        float characterWidth;

        // height (in units) of a single character multiplied by lineSpacing
        [SerializeField]
        [HideInInspector]
        float characterHeight;

        // indicates if the text editor has been resized
        bool resize = false;

        // time the editor window has been resized
        float resizeTime = 0f;

        // caret position
        TextPosition caretTextPosition = null;

        // drag start position
        TextPosition dragStartTextPosition = null;

        // the last time (in seconds since the application start) the visiblility of the caret was changed
        float caretBlinkTime = 0f;

        // current visibility state of the caret
        bool caretVisible = false;

        // row index of caret's position (caretRow == 0 means the caret is in the first row)
        float preferredCaretX = 0;

        // last time (in seconds since the application start) the left mouse button was clicked (set to -1000 initially in order not to trigger a double click on the first click occurs too soon after the application start)
        float clickTime = -1000f;

        // number of consecutive clicks (where the interval between clicks is below doubleClickInterval) 
        int clicks = 0;

        // mouse position of last click (double or triple clicks are only triggered when the mouse position has not changed between the clicks)
        Vector2 clickPoint = Vector2.zero;

        // indicates whether the current click is a double click
        bool doubleClick = false;

        // indicates whether the current click is a triple click
        bool tripleClick = false;

        // current selection
        Selection selection = null;

        // game objects (rectangular images) used to render current selection
        List<GameObject> selectionRects = new List<GameObject>();

        // scale factor used when canvas size is dynamically adjusted
        float canvasScale = 1f;

        // control or command key used for keyboard shortcuts
        bool ctrlOrCmdPressed = false;

        // most recently pressed key
        KeyCode keyHold;

        // indicates whether keyHold is currently held down
        bool keyHoldDown = false;

        // the time (since application start) keyHold was pressed
        float keyHoldTime = 0f;

        // the time (sice application start) keyHold was repeated
        float keyRepeatTime = 0f;

        // indicates whether the editor is active (the editor set active upon a mouse click inside the editor, it is set inactive upon a mouse click outside of the editor or when the application loses its focus)
        bool editorActive = false;

        // counts the number of frames the edit has been active
        int editorActiveFrames = 0;

        // indicates whether the mouse pointer is hovering over the editor (including line number panel and scrollbars)
        bool mouseHoverEditor = false;

        // indicates whether the mouse pointer is hovering over the main panel
        bool mouseHoverMainPanel = false;

        // indicates whether the mouse pointer is hovering over the line numbers panel
        bool mouseHoverLineNumberPanel = false;

        // indicates whether the current drag movement is dragging the contents of the editor
        bool draggingEditor = false;

        // tooltip visibility
        float tooltipVisibility = 0f;
        float tooltipActivationTime = 0f;

        // operations queue
        Queue<IOperation> operations = new Queue<IOperation>();

        // max time in seconds operations are allowed to use during one frame
        float maxOperationTimePerFrame = 0.01f;

        // max time in seconds text formatting is allowed to use during one frame
        float maxFormatTimePerFrame = 0.01f;

        // stopwatch to measure execution time of queued operations
        System.Diagnostics.Stopwatch operationsStopWatch = new System.Diagnostics.Stopwatch();

        // current event in the editor history
        History.Event currentEvent = null;

        // pointer to line in editor that is currently being formatted. After each operation, all lines before textFormatLinePointer have the correct format.
        int textFormatLinePointer = 0;

        // hash set representation of wordDelimiters list (for lookup performance)
        readonly HashSet<char> wordDelimitersHashSet = new HashSet<char>();

        /// <summary>
        /// Gets or sets the text. Setting the text using this property enqueues
        /// a SetTextOperation in the operations queue. If you need to set the
        /// text immediately, use the <c>SetText</c> method with
        /// <c>immedietely = true</c>. Setting the text also purges the editor's
        /// history.
        /// </summary>
        /// <value>The text held by the editor.</value>
        public string Text
        {
            get
            {
                // return selected text from start to end
                return GetSelectedText(new Selection(new TextPosition(0, 0), new TextPosition(lines.Count - 1, lines[lines.Count - 1].Text.Length)));
            }
            set
            {
                // do not set text in edit mode
                if (!Application.isPlaying)
                    return;

                // set text
                SetText(value);
            }
        }

        /// <summary>
        /// Gets or sets the current selection.
        /// </summary>
        /// <value>The selection.</value>
        public Selection Selection
        {
            get { return selection; }
            set { SetSelection(value); }
        }

        /// <summary>
        /// Gets or sets the caret position.
        /// </summary>
        /// <value>The caret position.</value>
        public TextPosition CaretPosition
        {
            get { return caretTextPosition; }
            set { PlaceCaret(value); }
        }

        /// <summary>
        /// Gets all lines.
        /// </summary>
        /// <value>The lines.</value>
        public ReadOnlyCollection<Line> Lines
        {
            get { return new ReadOnlyCollection<Line>(lines); }
        }

        /// <summary>
        /// Gets the character width based on the current font and font size.
        /// </summary>
        /// <value>The width of the character.</value>
        public float CharacterWidth
        {
            get { return characterWidth; }
        }

        /// <summary>
        /// Gets the character width based on the current font, font size, and
        /// line spacing.
        /// </summary>
        /// <value>The height of the character.</value>
        public float CharacterHeight
        {
            get { return characterHeight; }
        }

        /// <summary>
        /// Gets the available horizontal space without scrolling.
        /// </summary>
        /// <value>The horizontal space available.</value>
        public float HorizontalSpaceAvailable
        {
            get { return horizontalSpaceAvailable; }
        }

        /// <summary>
        /// Gets the available vertical space without scrolling.
        /// </summary>
        /// <value>The vertical space available.</value>
        public float VerticalSpaceAvailable
        {
            get { return verticalSpaceAvailable; }
        }

        /// <summary>
        /// Gets the width of the longest line.
        /// </summary>
        /// <value>The width of the longest line.</value>
        public float LongestLineWidth
        {
            get { return longestLineWidth; }
        }

        /// <summary>
        /// Gets the word delimiters. 
        /// </summary>
        /// <value>All characters delimiting words.</value>
        public HashSet<char> WordDelimiters
        {
            get { return wordDelimitersHashSet; }
        }

        /// <summary>
        /// Gets or sets the editor's activity status. If the editor is
        /// inactive, no mouse or keyboard inputs are detected but the
        /// operations in the operations queue are still being processed.
        /// </summary>
        /// <value><c>true</c> if editor is active; otherwise, <c>false</c>.
        /// </value>
        public bool EditorActive
        {
            get { return editorActive; }
            set
            {
                editorActive = value;
                if (editorActive)
                {
                    // select attached game object to be selected
                    EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(gameObject);
                }
                else
                {
                    // reset active frame count
                    editorActiveFrames = 0;

                    // reset all click and drag states
                    doubleClick = false;
                    tripleClick = false;
                    draggingEditor = false;
                }

                UpdateSelectionAndCaret();
            }
        }

        /// <summary>
        /// Sets the text held by the editor. This purges the editor's history.
        /// </summary>
        /// <param name="text">The text to be set.</param>
        /// <param name="immediately">If set to <c>true</c>, the text will be
        /// immediately inserted, blocking the execution of the game loop. If
        /// set to <c>false</c>, a SetTextOperation will be added to the
        /// operations queue.</param>
        public void SetText(string text, bool immediately = false)
        {
            // replace all variations of line feed characters by new line character
            text = text.Replace("\u000D\u000A", "\n").Replace("\u000B", "\n").Replace("\u000C", "\n").Replace("\u000D", "\n").Replace("\u0085", "\n").Replace("\u2028", "\n").Replace("\u2029", "\n");

            // reset longest line width
            longestLineWidth = 0f;

            // create operation
            SetTextOperation op = new SetTextOperation(text);

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);
        }

        /// <summary>
        /// Inserts text at a give position.
        /// </summary>
        /// <param name="textPosition">Text position.</param>
        /// <param name="text">Text.</param>
        /// <param name="addToHistory">Add insert action to history.</param>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void InsertText(TextPosition textPosition, string text, bool addToHistory = false, bool immediately = false)
        {
            // create operation
            InsertTextOperation op = new InsertTextOperation(textPosition, text, addToHistory);

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);
        }

        /// <summary>
        /// Deletes the text in the given text selection.
        /// </summary>
        /// <param name="deleteSelection">Selection of text to be deleted.
        /// </param>
        /// <param name="addToHistory">Add delete action to history.</param>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void DeleteText(Selection deleteSelection, bool addToHistory = false, bool immediately = false)
        {
            // create operation
            DeleteTextOperation op = new DeleteTextOperation(deleteSelection, addToHistory);

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);
        }

        /// <summary>
        /// Copies the selected text to the clipboard. If no text is selected,
        /// this method does nothing.
        /// </summary> 
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void Copy(bool immediately = false)
        {
            // create operation
            CopyOperation op = new CopyOperation();

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);

            // activate editor
            EditorActive = true;
        }

        /// <summary>
        /// Copies the selected text to the clipboard and deletes the selection
        /// from the editor. If no text is selected, this method does nothing.
        /// </summary>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void Cut(bool immediately = false)
        {
            if (!disableInput)
            {
                // create operation
                CutOperation op = new CutOperation();

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);

                // activate editor
                EditorActive = true;
            }
        }

        /// <summary>
        /// Pastes the content of the clipboard to the carets position. If a
        /// selection is active, the selected text will be deleted and the
        /// pasted text inserted in its place.
        /// </summary>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void Paste(bool immediately = false)
        {
            if (!disableInput)
            {
                // create operation
                PasteOperation op = new PasteOperation();

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);

                // activate editor
                EditorActive = true;
            }
        }

        /// <summary>
        /// Reverts the history back to the state after the last key action.
        /// </summary>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void Undo(bool immediately = false)
        {
            if (!disableInput)
            {
                // create operation
                UndoOperation op = new UndoOperation();

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);

                // activate editor
                EditorActive = true;
            }
        }

        /// <summary>
        /// Redoes everything in the future history (if available) until, and
        /// including, the next key action.
        /// </summary>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void Redo(bool immediately = false)
        {
            if (!disableInput)
            {
                // create operation
                RedoOperation op = new RedoOperation();

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);

                // activate editor
                EditorActive = true;
            }
        }

        /// <summary>
        /// Moves the caret in the given direction.
        /// </summary>
        /// <param name="direction">Direction.</param>
        /// <param name="select">Expand selection while moving caret.</param>
        /// <param name="entireWord">Jump to next start or end of word.</param>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void MoveCaret(MoveCaretOperation.Direction direction, bool select = false, bool entireWord = false, bool immediately = false)
        {
            if (!disableInput)
            {
                // create operation
                MoveCaretOperation op = new MoveCaretOperation(direction, select, entireWord);

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);
            }
        }

        /// <summary>
        /// Selects the entire content of the editor.
        /// </summary>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void SelectAll(bool immediately = false)
        {
            // create operation
            SelectAllOperation op = new SelectAllOperation();

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);

            // activate editor
            EditorActive = true;
        }

        /// <summary>
        /// Gets the selected text.
        /// </summary>
        /// <returns>Selected text.</returns>
        /// <param name="textSelection">Text selection.</param>
        public string GetSelectedText(Selection textSelection)
        {
            // return empty string if selection is invalid
            if (textSelection == null || !textSelection.IsValid)
                return "";

            // reverse selection
            if (textSelection.IsReversed)
                textSelection = new Selection(textSelection.end, textSelection.start);

            // return single line or append all lines within text selection
            if (textSelection.start.lineIndex == textSelection.end.lineIndex)
                return lines[textSelection.start.lineIndex].Text.Substring(textSelection.start.colIndex, textSelection.end.colIndex - textSelection.start.colIndex);
            else
            {
                // use StringBuilder to append lines
                StringBuilder stringBuilder = new StringBuilder();

                // append first line
                stringBuilder.Append(lines[textSelection.start.lineIndex].Text.Substring(textSelection.start.colIndex));
                stringBuilder.Append("\n");

                // append intermediate lines
                for (int i = textSelection.start.lineIndex + 1; i < textSelection.end.lineIndex; i++)
                {
                    stringBuilder.Append(lines[i].Text);
                    stringBuilder.Append("\n");
                }

                // append last line
                stringBuilder.Append(lines[textSelection.end.lineIndex].Text.Substring(0, textSelection.end.colIndex));

                // return selected text
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Selects the next occurrence of the given search string.
        /// </summary>
        /// <param name="searchString">Search string.</param>
        /// <param name="forward">Search forward.</param>
        /// <param name="immediately">Execute operation immediately instead of
        /// using the operations queue.</param>
        public void Find(string searchString, bool forward, bool immediately = false)
        {
            // create operation
            FindOperation op = new FindOperation(searchString, forward);

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);
        }

        /// <summary>
        /// Shows a tooltip message next to the given position.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="position">The position where to display the tooltip.
        /// </param>
        public void ShowTooltip(string message, Vector2 position)
        {
            // disable tooltip if message is different
            if (!tooltipText.text.Equals(message))
                tooltip.SetActive(false);

            // set tooltip text
            tooltipText.font = tooltipFont;
            tooltipText.fontSize = tooltipFontSize;
            tooltipText.lineSpacing = tooltipLineSpacing;
            tooltipText.fontStyle = tooltipFontStyle;
            tooltipText.text = message;

            // calculate tooltip dimensions
            float tooltipTextWidth = tooltipText.cachedTextGenerator.GetPreferredWidth(message, tooltipText.GetGenerationSettings(tooltipText.cachedTextGenerator.rectExtents.size * 0.5f));
            float tooltipTextHeight = tooltipText.cachedTextGenerator.GetPreferredHeight(message, tooltipText.GetGenerationSettings(tooltipText.cachedTextGenerator.rectExtents.size * 0.5f));
            tooltipText.GetComponent<RectTransform>().offsetMin = new Vector2(tooltipMarginLeft, tooltipMarginBottom);
            tooltipText.GetComponent<RectTransform>().offsetMax = new Vector2(-tooltipMarginRight, -tooltipMarginTop);
            tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipTextWidth + tooltipMarginLeft + tooltipMarginRight, tooltipTextHeight + tooltipMarginTop + tooltipMarginBottom);

            // position tooltip
            bool fitsLeft = position.x - tooltipTextWidth - tooltipMarginLeft - tooltipMarginRight > 0f;
            bool fitsRight = position.x + tooltipTextWidth + tooltipMarginLeft + tooltipMarginRight < GetComponent<RectTransform>().rect.width;
            bool fitsAbove = position.y + tooltipTextHeight + tooltipMarginTop + tooltipMarginBottom + tooltipAboveCursor < 0f;
            bool fitsBelow = position.y - tooltipTextHeight - tooltipMarginTop - tooltipMarginBottom - tooltipBelowCursor > -GetComponent<RectTransform>().rect.height;
            float tooltipPositionX = (fitsRight || !fitsLeft) ? position.x : position.x - tooltipTextWidth - tooltipMarginLeft - tooltipMarginRight;
            float tooltipPositionY = (fitsBelow || !fitsAbove) ? position.y - tooltipBelowCursor : position.y + tooltipTextHeight + tooltipMarginTop + tooltipMarginBottom + tooltipAboveCursor;
            tooltip.GetComponent<RectTransform>().localPosition = new Vector2(tooltipPositionX, tooltipPositionY);

            // reset tooltip activation time
            if (!tooltip.activeSelf)
                tooltipActivationTime = Time.time;

            // set tooltip active
            tooltip.SetActive(true);
        }

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        public void HideTooltip()
        {
            // disable tooltip
            tooltip.SetActive(false);
        }

        /// <summary>
        /// Removes all line labels.
        /// </summary>
        public void RemoveLabels()
        {
            foreach (Line line in lines)
                line.RemoveLabels();
        }

        /// <summary>
        /// Updates the layout of the editor.
        /// Must be invoked whenever anything related to the layout of the
        /// editor (size, position, scroll position, background color, etc.) has
        /// changed.
        /// </summary>
        public void UpdateLayout()
        {
            // set background color
            GetComponent<Image>().color = mainBackgroundColor;
            lineNumberPanel.GetComponent<Image>().color = lineNumberBackgroundColor;
            lineLabelIconsPanel.GetComponent<Image>().color = lineLabelIconsBackgroundColor;

            // setup scrollbars
            verticalScrollbar.gameObject.SetActive(showVerticalScrollbar);
            verticalScrollbar.GetComponent<RectTransform>().offsetMin = new Vector2(-scrollbarWidth, showHorizontalScrollbar ? scrollbarWidth : 0f);
            horizontalScrollbar.gameObject.SetActive(showHorizontalScrollbar);
            horizontalScrollbar.GetComponent<RectTransform>().offsetMax = new Vector2(showVerticalScrollbar ? -scrollbarWidth : 0f, scrollbarWidth);

            // setup line number panel
            lineNumberPanel.gameObject.SetActive(showLineNumbers);
            float lineNumberPanelWidth = Mathf.Max(lineNumberMinWidth, Mathf.FloorToInt(1f + (lines.Count > 0 ? Mathf.Log10(lines.Count) : 0)) * characterWidth + lineNumberMarginLeft + lineNumberMarginRight);
            lineNumberPanel.GetComponent<RectTransform>().offsetMin = new Vector2(0f, showHorizontalScrollbar ? scrollbarWidth : 0f);
            lineNumberPanel.GetComponent<RectTransform>().offsetMax = new Vector2(lineNumberPanelWidth, 0f);
            lineNumberText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-lineNumberMarginRight, -mainMarginTop);

            // setup line label panel
            lineLabelIconsPanel.gameObject.SetActive(showLineLabelIcons);
            lineLabelIconsPanel.GetComponent<RectTransform>().offsetMin = new Vector2(showLineNumbers ? lineNumberPanelWidth : 0f, showHorizontalScrollbar ? scrollbarWidth : 0f);
            lineLabelIconsPanel.GetComponent<RectTransform>().offsetMax = new Vector2(showLineNumbers ? lineNumberPanelWidth + lineLabelIconsWidth : lineLabelIconsWidth, 0f);

            // setup main panel
            mainPanel.GetComponent<RectTransform>().offsetMax = new Vector2(showVerticalScrollbar ? -scrollbarWidth : 0f, 0f);
            mainPanel.GetComponent<RectTransform>().offsetMin = new Vector2(showLineNumbers ? (showLineLabelIcons ? lineNumberPanelWidth + lineLabelIconsWidth : lineNumberPanelWidth) : (showLineLabelIcons ? lineLabelIconsWidth : 0f), showHorizontalScrollbar ? scrollbarWidth : 0f);

            // calculate required space
            if (Application.isPlaying)
            {
                verticalSpaceRequired = lines.Count == 0 ? characterHeight : -lines[lines.Count - 1].VerticalOffset + lines[lines.Count - 1].Height;
                horizontalSpaceRequired = longestLineWidth;
            }
            else
            {
                string[] textLines = mainText.text.Split('\n');
                verticalSpaceRequired = textLines.Length * characterHeight;
                horizontalSpaceRequired = 0f;
                for (int i = 0; i < textLines.Length; i++)
                    horizontalSpaceRequired = Mathf.Max(horizontalSpaceRequired, textLines[i].Length * characterWidth);
            }

            mainText.GetComponent<RectTransform>().anchoredPosition = new Vector2(mainMarginLeft, -mainMarginTop);
            lineNumberText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-lineNumberMarginRight, -mainMarginTop);

            // update
            UpdateScrollableContent();
            UpdateScrollbars();
            UpdateSelectionAndCaret();
        }

        /// <summary>
        /// Updates the font of the editor and calculates character width and
        /// height. Must be invoked whenever the font, font style, font size,
        /// font color, or line spacing has changed.
        /// </summary>
        public void UpdateFont()
        {
            // set font for main text
            mainText.font = font;
            mainText.fontSize = fontSize;
            mainText.lineSpacing = lineSpacing;
            mainText.fontStyle = mainFontStyle;
            mainText.color = mainFontColor;
            mainText.alignment = TextAnchor.UpperLeft;
            mainText.horizontalOverflow = HorizontalWrapMode.Overflow;
            mainText.verticalOverflow = VerticalWrapMode.Overflow;

            // set font for line number text
            lineNumberText.font = font;
            lineNumberText.fontSize = fontSize;
            lineNumberText.lineSpacing = lineSpacing;
            lineNumberText.fontStyle = lineNumberFontStyle;
            lineNumberText.color = lineNumberFontColor;
            lineNumberText.alignment = TextAnchor.UpperRight;
            lineNumberText.horizontalOverflow = HorizontalWrapMode.Overflow;
            lineNumberText.verticalOverflow = VerticalWrapMode.Overflow;

            // build a string with 1000 rows and 1000 cols and measure its dimensions
            int length = 1000;
            string tmpString = "";
            for (int i = 0; i < length; i++)
                tmpString += " ";
            for (int i = 1; i < length; i++)
                tmpString += "\n ";
            characterWidth = mainText.cachedTextGeneratorForLayout.GetPreferredWidth(tmpString, mainText.GetGenerationSettings(mainText.cachedTextGenerator.rectExtents.size * 0.5f)) / length;
            characterHeight = mainText.cachedTextGeneratorForLayout.GetPreferredHeight(tmpString, mainText.GetGenerationSettings(mainText.cachedTextGenerator.rectExtents.size * 0.5f)) / ((length - 1) * lineSpacing + 1) * lineSpacing;

            // check if font is monospaced
            for (char c = '\u0020'; c <= '\uF000'; c++)
            {
                if (!Util.IsPrintableCharacter(c))
                    continue;
                float tmpCharacterWidth = mainText.cachedTextGeneratorForLayout.GetPreferredWidth(c.ToString(), mainText.GetGenerationSettings(mainText.cachedTextGenerator.rectExtents.size * 0.5f));
                if (!Mathf.Approximately(characterWidth, tmpCharacterWidth))
                    throw new UnityException("Not a monospaced font.\nCharacter " + c.ToString() + " (Unicode " + string.Format("0x{0:X4}", ((int)c)) + ") has width " + tmpCharacterWidth.ToString("F3") + " but the calculated character width is " + characterWidth.ToString("F3") + ".\nPlease select a font where all printable characters have the same width.\n");
            }

            // adjust character size according to canvas scale
            characterWidth /= canvasScale;
            characterHeight /= canvasScale;

            // rebuild all lines (immediately if in edit mode)
            RebuildLines(!Application.isPlaying);
        }

        /// <summary>
        /// Updates the text format line pointer. This method is invoked by a
        /// line when its text has been updated.
        /// </summary>
        /// <param name="line">The updated line.</param>
        public void OnLineChanged(Line line)
        {
            // reset text format pointer
            textFormatLinePointer = Mathf.Min(textFormatLinePointer, line.LineNumber);
        }

        /// <summary>
        /// Invoked when the handle on the vertical scrollbar is moved.
        /// </summary>
        public void OnVerticalScrollbarUpdated()
        {
            if (verticalSpaceRequired > verticalSpaceAvailable)
                verticalOffset = verticalScrollbar.value * (verticalSpaceRequired - verticalSpaceAvailable);
            UpdateScrollableContent();
        }

        /// <summary>
        /// Invoked when the handle on the horizontal scrollbar is moved.
        /// </summary>
        public void OnHorizontalScrollbarUpdated()
        {
            if (horizontalSpaceRequired > horizontalSpaceAvailable)
                horizontalOffset = horizontalScrollbar.value * (horizontalSpaceAvailable - horizontalSpaceRequired);
            UpdateScrollableContent();
        }

        // initializes editor on application start
        void Start()
        {
            // do not execute these steps in edit mode
            if (!Application.isPlaying)
                return;

            //Application.targetFrameRate = 5;

            // create word delimiters hash set
            foreach (char c in wordDelimiters)
                wordDelimitersHashSet.Add(c);

            // enable IME composition
            Input.imeCompositionMode = IMECompositionMode.On;

            // invert scroll direction on macOS
            invertHorizontalScrollDirection = (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor);

            // disable text blocks used for editor preview
            mainText.gameObject.SetActive(false);
            lineNumberText.gameObject.SetActive(false);

            // add empty first line
            lines.Add(new Line(0, "", 0f, this));

            // place caret at start
            caretTextPosition = new TextPosition(0, 0);

            // calculate available space
            verticalSpaceAvailable = mainPanel.GetComponent<RectTransform>().rect.height - mainMarginTop - mainMarginBottom;
            horizontalSpaceAvailable = mainPanel.GetComponent<RectTransform>().rect.width - mainMarginLeft - mainMarginRight;

            // update font
            UpdateFont();

            // update layout
            UpdateLayout();

            // set default text
            SetText(defaultText, true);

            // hide lock mask
            lockMask.SetActive(false);
        }

        // update editor (each frame)
        void Update()
        {
            if (!Application.isPlaying)
            {
                // calculate available and required space when in edit mode
                verticalSpaceAvailable = mainPanel.GetComponent<RectTransform>().rect.height - mainMarginTop - mainMarginBottom;
                horizontalSpaceAvailable = mainPanel.GetComponent<RectTransform>().rect.width - mainMarginLeft - mainMarginRight;

                // update font
                UpdateFont();

                // do not execute rest of Update method
                return;
            }

            // update font when canvas scale changes
            if (!Mathf.Approximately(GetComponentInParent<Canvas>().scaleFactor, canvasScale))
            {
                canvasScale = GetComponentInParent<Canvas>().scaleFactor;
                UpdateFont();
            }

            // process operations queue
            operationsStopWatch.Reset();
            operationsStopWatch.Start();
            do
            {
                if (operations.Count == 0)
                    break;
                IOperation op = operations.Peek();
                if (ExecuteOperation(op))
                    operations.Dequeue();
            } while (operationsStopWatch.ElapsedMilliseconds < maxOperationTimePerFrame * 1000f);

            // check for resize
            if (!Mathf.Approximately(verticalSpaceAvailable, mainPanel.GetComponent<RectTransform>().rect.height - mainMarginTop - mainMarginBottom) || !Mathf.Approximately(horizontalSpaceAvailable, mainPanel.GetComponent<RectTransform>().rect.width - mainMarginLeft - mainMarginRight))
            {
                verticalSpaceAvailable = mainPanel.GetComponent<RectTransform>().rect.height - mainMarginTop - mainMarginBottom;
                horizontalSpaceAvailable = mainPanel.GetComponent<RectTransform>().rect.width - mainMarginLeft - mainMarginRight;

                // set resize and resize time
                resize = true;
                resizeTime = Time.time;
            }

            // show or hide lock mask
            lockMask.SetActive(showLockMask && operations.Count > 0);

            // only execute these steps if the editor is not busy
            if (operations.Count == 0)
            {
                // format text
                operationsStopWatch.Reset();
                operationsStopWatch.Start();
                do
                {
                    if (textFormatLinePointer >= lines.Count)
                        break;

                    // loop over all text editors attached to this game object
                    foreach (TextFormatter textFormatter in GetComponents<TextFormatter>())
                    {
                        // check if text formatter has been initialized
                        if (!textFormatter.Initialized)
                            textFormatter.Init();

                        // notify text formatter that line has changed
                        textFormatter.OnLineChanged(lines[textFormatLinePointer]);
                    }

                    textFormatLinePointer++;
                } while (operationsStopWatch.ElapsedMilliseconds < maxFormatTimePerFrame * 1000f);

                // update layout after resize timeout
                if (resize && (Mathf.Approximately(resizeTimeout, 0f) || Time.time > resizeTime + resizeTimeout))
                {
                    // rebuild all lines if wrap lines is enabled
                    if (wrapLines)
                        RebuildLines();

                    // update layout and reset resize indicator
                    UpdateLayout();
                    resize = false;
                }

                // show or hide caret
                if (editorActive && !disableInput && selection == null)
                {
                    if (Time.time > caretBlinkTime + 1f / caretBlinkRate)
                    {
                        caretVisible = !caretVisible;
                        caretBlinkTime = Time.time;
                    }
                }
                else
                {
                    caretVisible = false;
                    caretBlinkTime = 0f;
                }

                caret.GetComponent<Image>().enabled = caretVisible;

                // determine camera that renders the GUI elements
                Camera guiCamera = null;
                if (GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace)
                    guiCamera = Camera.main;
                else
                    guiCamera = GetComponentInParent<Canvas>().worldCamera;

                // calculate mouse position
                Vector2 mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainContent.GetComponent<RectTransform>(), Input.mousePosition, guiCamera, out mousePosition);

                // subtract margin (so mouse position 0,0 is at the top-left corner of the text's first character)
                mousePosition += new Vector2(-mainMarginLeft, mainMarginTop);

                // detect if mouse is hovering over editor
                mouseHoverEditor = RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, guiCamera);
                mouseHoverMainPanel = RectTransformUtility.RectangleContainsScreenPoint(mainPanel.GetComponent<RectTransform>(), Input.mousePosition, guiCamera);
                mouseHoverLineNumberPanel = RectTransformUtility.RectangleContainsScreenPoint(lineNumberPanel.GetComponent<RectTransform>(), Input.mousePosition, guiCamera);

                // follow caret when mouse button released
                if (Input.GetMouseButtonUp(0) && mouseHoverMainPanel && !doubleClick && !tripleClick)
                    FollowCaret();

                // release mouse button
                if (!Input.GetMouseButton(0))
                {
                    doubleClick = false;
                    tripleClick = false;
                    draggingEditor = false;
                }

                // activate / deactivate editor and count clicks
                if (Input.GetMouseButtonDown(0))
                {
                    // activate or deactivate on click
                    if (mouseHoverEditor)
                        EditorActive = true;
                    else if (deactivateOnClickOutsideOfEditor)
                        EditorActive = false;

                    // detect dragging of scrollbars
                    if (mouseHoverMainPanel || mouseHoverLineNumberPanel)
                        draggingEditor = true;

                    // detect multiple click
                    if (Time.time - clickTime < doubleClickInterval && mousePosition == clickPoint)
                        clicks++;
                    else
                        clicks = 1;

                    // store time and mouse position of this click
                    clickTime = Time.time;
                    clickPoint = mousePosition;

                    // set double or triple click
                    doubleClick = (clicks == 2);
                    tripleClick = (clicks == 3);
                }

                // scrolling by mouse wheel or trackpad
                if (mouseHoverEditor && Input.mouseScrollDelta != Vector2.zero)
                {
                    // update vertical offset based on vertical scrolling
                    verticalOffset += Input.mouseScrollDelta.y * verticalWheelScrollSpeed * (invertVerticalScrollDirection ? 1f : -1f);
                    verticalOffset = Mathf.Clamp(verticalOffset, 0f, verticalSpaceRequired - verticalSpaceAvailable);

                    // update horizontal offset based on horizontal scrolling
                    horizontalOffset -= Input.mouseScrollDelta.x * horizontalWheelScrollSpeed * (invertHorizontalScrollDirection ? -1f : 1f);
                    horizontalOffset = Mathf.Clamp(horizontalOffset, horizontalSpaceAvailable - horizontalSpaceRequired, 0f);

                    // update
                    UpdateScrollableContent();
                    UpdateScrollbars();
                }

                // click
                if ((mouseHoverMainPanel || mouseHoverLineNumberPanel) && Input.GetMouseButtonDown(0))
                    Click(mousePosition);

                // drag
                if (editorActive && draggingEditor && Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
                    Drag(mousePosition);

                // hide invisible lines and show visible lines
                int minLineVisible = int.MaxValue;
                int maxLineVisible = -1;
                foreach (Line line in lines)
                {
                    if (verticalOffset + line.VerticalOffset - line.Height <= 0f && verticalOffset + line.VerticalOffset >= -verticalSpaceAvailable)
                    {
                        line.Visible = true;
                        minLineVisible = Mathf.Min(minLineVisible, line.LineNumber);
                        maxLineVisible = Mathf.Max(maxLineVisible, line.LineNumber);
                    }
                    else
                        line.Visible = false;
                }

                // calculate mouse position
                Vector2 tooltipPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, guiCamera, out tooltipPosition);

                // show tooltip
                bool showTooltip = false;
                string tooltipMessage = null;
                if (!draggingEditor)
                {
                    for (int i = minLineVisible; i <= maxLineVisible && !showTooltip; i++)
                    {
                        foreach (Line.Label label in lines[i].Labels)
                        {
                            // show only the first tooltip
                            if (showTooltip)
                                break;

                            // ignore label without message
                            if (string.IsNullOrEmpty(label.tooltipMessage))
                                continue;

                            // show tooltip for this label if mouse cursor is over label icon
                            if (showLineLabelIcons && label.icon != null && RectTransformUtility.RectangleContainsScreenPoint(label.icon.GetComponent<RectTransform>(), Input.mousePosition, guiCamera))
                            {
                                showTooltip = true;
                                tooltipMessage = label.tooltipMessage;
                                break;
                            }

                            // show tooltip for this label if mouse cursor is within one of the label's rect's boundaries
                            foreach (GameObject labelRect in label.labelRects)
                            {
                                if (RectTransformUtility.RectangleContainsScreenPoint(labelRect.GetComponent<RectTransform>(), Input.mousePosition, guiCamera))
                                {
                                    showTooltip = true;
                                    tooltipMessage = label.tooltipMessage;
                                    break;
                                }
                            }
                        }
                    }
                }

                // show or hide tooltip
                if (showTooltip)
                    ShowTooltip(tooltipMessage, tooltipPosition);
                else
                    HideTooltip();
            }
            else
            {
                // reset drag and multiple clicks
                doubleClick = false;
                tripleClick = false;
                draggingEditor = false;
                clicks = 1;

                // hide tooltip
                HideTooltip();
            }

            // handle keyboard input
            if (editorActive && editorActiveFrames > 1)
            {
                // check for modifier keys
                ctrlOrCmdPressed = false;
                if (Util.IsMacOS())
                {
                    // macOS key binding
                    if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
                        ctrlOrCmdPressed = true;
                }
                else
                {
                    // Windows key binding
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        ctrlOrCmdPressed = true;
                }

                // register functional key strokes (arrow keys, tab, forward delete, and keys used in shortcuts)
                if (Input.GetKeyDown(KeyCode.A))
                {
                    keyHold = KeyCode.A;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    keyHold = KeyCode.C;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (Input.GetKeyDown(KeyCode.X))
                {
                    keyHold = KeyCode.X;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    keyHold = KeyCode.V;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    keyHold = KeyCode.Z;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (Input.GetKeyDown(KeyCode.Y))
                {
                    keyHold = KeyCode.Y;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    keyHold = KeyCode.Tab;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    keyHold = KeyCode.Delete;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (useArrowKeys && Input.GetKeyDown(KeyCode.UpArrow))
                {
                    keyHold = KeyCode.UpArrow;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (useArrowKeys && Input.GetKeyDown(KeyCode.DownArrow))
                {
                    keyHold = KeyCode.DownArrow;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (useArrowKeys && Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    keyHold = KeyCode.LeftArrow;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (useArrowKeys && Input.GetKeyDown(KeyCode.RightArrow))
                {
                    keyHold = KeyCode.RightArrow;
                    keyHoldDown = true;
                    keyHoldTime = Time.time;
                    keyRepeatTime = 0f;
                    HandleKeyStroke(keyHold);
                }

                if (!Input.GetKey(keyHold))
                    keyHoldDown = false;

                // loop through keyboard buffer to process text input
                if (!ctrlOrCmdPressed && !disableInput)
                {
                    foreach (char c in Input.inputString)
                    {
                        switch (c)
                        {
                            case '\u000A':
                            case '\u000B':
                            case '\u000C':
                            case '\u000D':
                            case '\u0085':
                            case '\u2028':
                            case '\u2029':
                                InsertCharacter('\n');
                                break;
                            case '\u0008':
                                Delete(false);
                                break;
                            default:
                                if (Util.IsPrintableCharacter(c))
                                    InsertCharacter(c);
                                break;
                        }
                    }
                }
            }

            // increase active frame count
            if (editorActive && editorActiveFrames < int.MaxValue)
                editorActiveFrames++;

            // change tooltip visibility
            if (tooltip.activeSelf)
            {
                if (tooltipFadeDuration > 0f)
                    tooltipVisibility = Mathf.Lerp(0f, 1f, (Time.time - tooltipDelay - tooltipActivationTime) / tooltipFadeDuration);
                else
                    tooltipVisibility = Time.time - tooltipDelay > tooltipActivationTime ? 1f : 0f;
            }
            else
                tooltipVisibility = 0f;

            tooltip.GetComponent<Image>().color = new Color(tooltipBackgroundColor.r, tooltipBackgroundColor.g, tooltipBackgroundColor.b, tooltipBackgroundColor.a * tooltipVisibility);
            tooltipText.color = new Color(tooltipFontColor.r, tooltipFontColor.g, tooltipFontColor.b, tooltipFontColor.a * tooltipVisibility);
        }

        // fixed update (fixed timestep as defined in TimeManager) 
        void FixedUpdate()
        {
            // do not execute in edit mode
            if (!Application.isPlaying)
                return;

            // trigger repeated key strokes when key is held down
            if (keyHoldDown && Time.time >= keyHoldTime + keyRepeatThreshold && Time.time >= keyRepeatTime + 1f / keyRepeatRate)
            {
                HandleKeyStroke(keyHold, true);
                keyRepeatTime = Time.time;
            }
        }

        // update editor appereance whenever a value has changed
        // is only executed in Unity Editor
        void OnValidate()
        {
            // force min font size of 1
            fontSize = Mathf.Max(fontSize, 1);

            // force min line spacing of 1.0
            lineSpacing = Mathf.Max(lineSpacing, 1f);

            // force min main margins of 0.0
            mainMarginLeft = Mathf.Max(mainMarginLeft, 0f);
            mainMarginRight = Mathf.Max(mainMarginRight, 0f);
            mainMarginTop = Mathf.Max(mainMarginTop, 0f);
            mainMarginBottom = Mathf.Max(mainMarginBottom, 0f);

            // force line number min width of 0.0 or greater
            lineNumberMinWidth = Mathf.Max(lineNumberMinWidth, 0f);

            // force min line number margins of 0.0
            lineNumberMarginLeft = Mathf.Max(lineNumberMarginLeft, 0f);
            lineNumberMarginRight = Mathf.Max(lineNumberMarginRight, 0f);

            // force min scrollbar width of 0.0
            scrollbarWidth = Mathf.Max(scrollbarWidth, 0f);

            // force min scroll speed of 0.0
            verticalWheelScrollSpeed = Mathf.Max(verticalWheelScrollSpeed, 0f);
            horizontalWheelScrollSpeed = Mathf.Max(horizontalWheelScrollSpeed, 0f);
            verticalDragScrollSpeed = Mathf.Max(verticalDragScrollSpeed, 0f);
            horizontalDragScrollSpeed = Mathf.Max(horizontalDragScrollSpeed, 0f);

            // force min caret width of 0.0
            caretWidth = Mathf.Max(caretWidth, 0f);

            // force min caret blink rate of 0.1
            caretBlinkRate = Mathf.Max(caretBlinkRate, 0.1f);

            // force min double click interval of 0.1
            doubleClickInterval = Mathf.Max(doubleClickInterval, 0.1f);

            // force min tab stop width of 1
            tabStopWidth = Mathf.Max(tabStopWidth, 1);

            // force min key repeat threshold of 0.1
            keyRepeatThreshold = Mathf.Max(keyRepeatThreshold, 0.1f);

            // force min key repeat rate of 0.1
            keyRepeatRate = Mathf.Max(keyRepeatRate, 0.1f);

            // force min max history length of 0
            maxHistoryLength = Mathf.Max(maxHistoryLength, 0);

            // force line label width of 0.0 or greater
            lineLabelIconsWidth = Mathf.Max(lineLabelIconsWidth, 0f);

            // force min tooltip font size of 1
            tooltipFontSize = Mathf.Max(tooltipFontSize, 1);

            // force min tooltip line spacing of 0.0
            tooltipLineSpacing = Mathf.Max(tooltipLineSpacing, 0f);

            // force min tooltip margins of 0.0
            tooltipMarginLeft = Mathf.Max(tooltipMarginLeft, 0f);
            tooltipMarginRight = Mathf.Max(tooltipMarginRight, 0f);
            tooltipMarginTop = Mathf.Max(tooltipMarginTop, 0f);
            tooltipMarginBottom = Mathf.Max(tooltipMarginBottom, 0f);

            // force min tooltip above and below cursor of 0.0
            tooltipAboveCursor = Mathf.Max(tooltipAboveCursor, 0f);
            tooltipBelowCursor = Mathf.Max(tooltipBelowCursor, 0f);

            // force min tooltip delay of 0.0
            tooltipDelay = Mathf.Max(tooltipDelay, 0f);

            // force min tooltip fade duration of 0.0
            tooltipFadeDuration = Mathf.Max(tooltipFadeDuration, 0f);

            // update font when in play mode
            if (Application.isPlaying)
                UpdateFont();
        }

        // invoked when the application gains or loses focus
        void OnApplicationFocus(bool focus)
        {
            // deactivate editor on lost focus
            if (!focus && deactivateOnLostApplicationFocus)
                EditorActive = false;
        }

        // handles key stroke of given key
        // is invoked on keydown event or every (1 / keyRepeatRate) seconds when a key is held down for more than keyRepeatThreshold seconds
        void HandleKeyStroke(KeyCode keyCode, bool repeatedStroke = false)
        {
            // handle arrow keys
            bool select = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool entireWord = (Util.IsMacOS() && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) || (!Util.IsMacOS() && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)));
            switch (keyCode)
            {
                case KeyCode.UpArrow:
                    MoveCaret(MoveCaretOperation.Direction.UP, select);
                    break;
                case KeyCode.DownArrow:
                    MoveCaret(MoveCaretOperation.Direction.DOWN, select);
                    break;
                case KeyCode.LeftArrow:
                    MoveCaret(MoveCaretOperation.Direction.LEFT, select, entireWord);
                    break;
                case KeyCode.RightArrow:
                    MoveCaret(MoveCaretOperation.Direction.RIGHT, select, entireWord);
                    break;
            }

            // handle tab and forward delete
            if (keyCode == KeyCode.Tab)
            {
                if (selection != null && selection.IsValid)
                {
                    if (!repeatedStroke)
                        ModifyIndent(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift));
                }
                else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                    InsertCharacter('\t');
            }
            else if (keyCode == KeyCode.Delete)
                Delete(true);

            // handle keyboard shortcuts
            if (ctrlOrCmdPressed && useDefaultKeyboardShortcuts)
            {
                switch (keyCode)
                {
                    case KeyCode.C:
                        Copy();
                        break;
                    case KeyCode.X:
                        if (!disableInput)
                            Cut();
                        break;
                    case KeyCode.V:
                        if (!disableInput)
                            Paste();
                        break;
                    case KeyCode.A:
                        SelectAll();
                        break;
                    case KeyCode.Z:
                        if (!disableInput)
                        {
                            // use Shift+Cmd+Z for Redo on macOS
                            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                                Undo();
                            else if (Util.IsMacOS())
                                Redo();
                        }

                        break;
                    case KeyCode.Y:
                        if (!disableInput)
                        {
                            // use Ctrl+Y for Redo on Windows and Linux
                            if (!Util.IsMacOS())
                                Redo();
                        }

                        break;
                }
            }
        }

        // mouse click at mousePosition coordinates (relative to top left corner of main content)
        void Click(Vector2 mousePosition)
        {
            // get text position for mouse coordinates
            TextPosition textPosition = GetTextPositionForCoordinates(mousePosition);

            // shift click
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (selection != null)
                {
                    // extend existing selection
                    selection.end = textPosition;
                }
                else
                {
                    // create new selection
                    selection = new Selection(caretTextPosition, textPosition);
                    caretTextPosition = textPosition;
                }
            }
            else
            {
                caretTextPosition = textPosition;
                preferredCaretX = lines[textPosition.lineIndex].GetCaretPosition(textPosition).x;

                if (doubleClick)
                {
                    // select entire word
                    TextPosition selectionStart = lines[caretTextPosition.lineIndex].FindWordStartOrEnd(caretTextPosition, false);
                    TextPosition selectionEnd = lines[caretTextPosition.lineIndex].FindWordStartOrEnd(caretTextPosition, true);
                    selection = new Selection(selectionStart, selectionEnd);
                    dragStartTextPosition = selectionStart;
                }
                else if (tripleClick)
                {
                    // select entire line
                    TextPosition selectionStart = new TextPosition(caretTextPosition.lineIndex, 0);
                    TextPosition selectionEnd = new TextPosition(caretTextPosition.lineIndex, lines[caretTextPosition.lineIndex].Text.Length);
                    selection = new Selection(selectionStart, selectionEnd);
                    dragStartTextPosition = selectionStart;
                }
                else
                {
                    // disable selection
                    selection = null;
                    dragStartTextPosition = textPosition;
                }
            }

            // reset caret blinking
            caretBlinkTime = 0f;
            caretVisible = false;

            // update
            UpdateSelectionAndCaret();

            // add milestone to history
            AddMilestoneToHistory();
        }

        // drag (to mousePosition)
        void Drag(Vector2 mousePosition)
        {
            // clamp mouse position to visible text area
            Vector2 clampedMousePosition = new Vector2(Mathf.Clamp(mousePosition.x, -horizontalOffset - mainMarginLeft - characterWidth, -horizontalOffset + horizontalSpaceAvailable + mainMarginRight + characterWidth), Mathf.Clamp(mousePosition.y, -verticalOffset - verticalSpaceAvailable - mainMarginBottom - characterHeight, -verticalOffset + mainMarginTop + characterHeight));

            // find text position of drag end
            TextPosition dragEndTextPosition = GetTextPositionForCoordinates(clampedMousePosition);

            // if drag end and drag start are the same, remove selection
            if (dragStartTextPosition.lineIndex == dragEndTextPosition.lineIndex && dragStartTextPosition.colIndex == dragEndTextPosition.colIndex && !doubleClick && !tripleClick)
            {
                caretTextPosition = dragStartTextPosition;
                selection = null;
            }

            // set preferred horizontal caret position to drag end
            preferredCaretX = lines[dragEndTextPosition.lineIndex].GetCaretPosition(dragEndTextPosition).x;

            // handle double and triple clicks
            if (doubleClick)
            {
                // select entire word
                TextPosition selectionStart;
                TextPosition selectionEnd;
                if (dragStartTextPosition.lineIndex < dragEndTextPosition.lineIndex || (dragStartTextPosition.lineIndex == dragEndTextPosition.lineIndex && dragStartTextPosition.colIndex <= dragEndTextPosition.colIndex))
                {
                    selectionStart = lines[dragStartTextPosition.lineIndex].FindWordStartOrEnd(dragStartTextPosition, false);
                    selectionEnd = lines[dragEndTextPosition.lineIndex].FindWordStartOrEnd(dragEndTextPosition, true);
                }
                else
                {
                    selectionStart = lines[dragStartTextPosition.lineIndex].FindWordStartOrEnd(dragStartTextPosition, true);
                    selectionEnd = lines[dragEndTextPosition.lineIndex].FindWordStartOrEnd(dragEndTextPosition, false);
                }

                selection = new Selection(selectionStart, selectionEnd);
            }
            else if (tripleClick)
            {
                // select entire line
                TextPosition selectionStart;
                TextPosition selectionEnd;
                if (dragStartTextPosition.lineIndex < dragEndTextPosition.lineIndex || (dragStartTextPosition.lineIndex == dragEndTextPosition.lineIndex && dragStartTextPosition.colIndex <= dragEndTextPosition.colIndex))
                {
                    selectionStart = new TextPosition(dragStartTextPosition.lineIndex, 0);
                    selectionEnd = new TextPosition(dragEndTextPosition.lineIndex, lines[dragEndTextPosition.lineIndex].Text.Length);
                }
                else
                {
                    selectionStart = new TextPosition(dragStartTextPosition.lineIndex, lines[dragStartTextPosition.lineIndex].Text.Length);
                    selectionEnd = new TextPosition(dragEndTextPosition.lineIndex, 0);
                }

                selection = new Selection(selectionStart, selectionEnd);
            }
            else
            {
                // update selection
                selection = new Selection(dragStartTextPosition, dragEndTextPosition);
            }

            // scroll if we drag the selection beyond the boundaries of the currently visible text
            float scrollUp = 0f;
            if (mousePosition.y > -verticalOffset + mainMarginTop)
                scrollUp = mousePosition.y + verticalOffset - mainMarginTop;
            else if (mousePosition.y < -verticalOffset - verticalSpaceAvailable - mainMarginBottom)
                scrollUp = mousePosition.y + verticalOffset + verticalSpaceAvailable + mainMarginBottom;
            float scrollRight = 0f;
            if (mousePosition.x < -horizontalOffset - mainMarginLeft)
                scrollRight = -horizontalOffset - mousePosition.x - mainMarginLeft;
            else if (mousePosition.x > -horizontalOffset + horizontalSpaceAvailable + mainMarginRight && lines[selection.end.lineIndex].Width > -horizontalOffset + horizontalSpaceAvailable)
                scrollRight = -horizontalOffset + horizontalSpaceAvailable - mousePosition.x + mainMarginRight;
            verticalOffset -= scrollUp * verticalDragScrollSpeed * Time.deltaTime;
            verticalOffset = Mathf.Clamp(verticalOffset, 0f, verticalSpaceRequired - verticalSpaceAvailable);
            horizontalOffset += scrollRight * horizontalDragScrollSpeed * Time.deltaTime;
            horizontalOffset = Mathf.Clamp(horizontalOffset, horizontalSpaceAvailable - horizontalSpaceRequired, 0f);

            // update
            UpdateLayout();
        }

        // inserts the given character at the current caret position
        // if an active selection exists, the selected text is deleted and the character c is inserted at its place
        void InsertCharacter(char c, bool immediately = false)
        {
            // abort if c is a non-printable character (exception for tab)
            if (!Util.IsPrintableCharacter(c) && c != '\t')
                return;

            if (!disableInput)
            {
                // creation operation
                InsertCharacterOperation op = new InsertCharacterOperation(c);

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);
            }
        }

        // deletes selected text (if selection is active) or single character directly before or after the caret
        void Delete(bool forward, bool immediately = false)
        {
            if (!disableInput)
            {
                // create operation
                DeleteOperation op = new DeleteOperation(forward);

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);
            }
        }

        // increases or decreases indent of the selected lines
        void ModifyIndent(bool increase, bool immediately = false)
        {
            if (!disableInput)
            {
                // create operation
                ModifyIndentOperation op = new ModifyIndentOperation(increase);

                // enqueue operation or execute immediately
                if (immediately)
                {
                    while (!ExecuteOperation(op))
                    {
                        // wait for execution to complete
                    }
                }
                else
                    operations.Enqueue(op);
            }
        }

        // places the caret at the given text position
        void PlaceCaret(TextPosition textPosition, bool immediately = false)
        {
            // create operation
            PlaceCaretOperation op = new PlaceCaretOperation(textPosition);

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);
        }

        // scrolls content to position where caret or selection end is visible
        // must be invoked whenever the caret position or selection has changed
        void FollowCaret()
        {
            if (selection != null)
            {
                // follow selection start
                /*Vector2 selectionStartPosition = lines[selection.start.lineIndex].GetCaretPosition(selection.start);
                if(selectionStartPosition.x < -horizontalOffset)
                    horizontalOffset = -selectionStartPosition.x;
                else if(selectionStartPosition.x > -horizontalOffset + horizontalSpaceAvailable)
                    horizontalOffset = -selectionStartPosition.x + horizontalSpaceAvailable;
                if(selectionStartPosition.y < -verticalOffset - verticalSpaceAvailable + characterHeight)
                    verticalOffset = -selectionStartPosition.y - verticalSpaceAvailable + characterHeight;
                else if(selectionStartPosition.y > -verticalOffset)
                    verticalOffset = -selectionStartPosition.y;*/

                // follow selection end
                Vector2 selectionEndPosition = lines[selection.end.lineIndex].GetCaretPosition(selection.end);
                if (selectionEndPosition.x < -horizontalOffset)
                    horizontalOffset = -selectionEndPosition.x;
                else if (selectionEndPosition.x > -horizontalOffset + horizontalSpaceAvailable)
                    horizontalOffset = -selectionEndPosition.x + horizontalSpaceAvailable;
                if (selectionEndPosition.y < -verticalOffset - verticalSpaceAvailable + characterHeight)
                    verticalOffset = -selectionEndPosition.y - verticalSpaceAvailable + characterHeight;
                else if (selectionEndPosition.y > -verticalOffset)
                    verticalOffset = -selectionEndPosition.y;
            }
            else
            {
                // follow caret
                Vector2 caretPosition = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition);

                // set drag start to caret position
                dragStartTextPosition = caretTextPosition;

                if (caretPosition.x < -horizontalOffset)
                    horizontalOffset = -caretPosition.x;
                else if (caretPosition.x > -horizontalOffset + horizontalSpaceAvailable)
                    horizontalOffset = -caretPosition.x + horizontalSpaceAvailable;
                if (caretPosition.y < -verticalOffset - verticalSpaceAvailable + characterHeight)
                    verticalOffset = -caretPosition.y - verticalSpaceAvailable + characterHeight;
                else if (caretPosition.y > -verticalOffset)
                    verticalOffset = -caretPosition.y;
            }

            // update
            UpdateScrollableContent();
            UpdateScrollbars();
        }


        // sets the selection
        void SetSelection(Selection textSelection, bool immediately = false)
        {
            // check if selection is valid
            if (textSelection == null || !textSelection.IsValid)
                throw new UnityException("Invalid selection");

            // create operation
            SetSelectionOperation op = new SetSelectionOperation(textSelection);

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);
        }

        // creates a new selection rect from topLeft to bottomRight
        void AddSelectionRect(Vector2 topLeft, Vector2 bottomRight)
        {
            // create selection background
            GameObject selectionRect = new GameObject("Selection");
            selectionRect.transform.SetParent(selectionContainer);
            selectionRect.AddComponent<Image>();
            selectionRect.GetComponent<Image>().color = (editorActive && !disableInput) ? selectionActiveColor : selectionInactiveColor;
            selectionRect.GetComponent<RectTransform>().localPosition = Vector3.zero;
            selectionRect.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            selectionRect.GetComponent<RectTransform>().localScale = Vector3.one;
            selectionRect.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            selectionRect.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            selectionRect.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            selectionRect.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            selectionRect.GetComponent<RectTransform>().anchoredPosition = new Vector2(mainMarginLeft + topLeft.x, -mainMarginTop + topLeft.y);
            selectionRect.GetComponent<RectTransform>().sizeDelta = new Vector2(bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
            selectionRect.transform.SetAsLastSibling();
            selectionRects.Add(selectionRect);
        }

        // rebuilds all text lines
        void RebuildLines(bool immediately = false)
        {
            // creation operation
            RebuildLinesOperation op = new RebuildLinesOperation();

            // enqueue operation or execute immediately
            if (immediately)
            {
                while (!ExecuteOperation(op))
                {
                    // wait for execution to complete
                }
            }
            else
                operations.Enqueue(op);
        }

        // gets closest text position for given mouse coordinates
        TextPosition GetTextPositionForCoordinates(Vector2 coordinates)
        {
            if (coordinates.y > 0f)
            {
                // beginning of document if coordinates are above top edge
                return new TextPosition(0, 0);
            }
            else if (coordinates.y < lines[lines.Count - 1].VerticalOffset - lines[lines.Count - 1].Height)
            {
                // end of document if coordinates are below bottom edge
                return new TextPosition(lines.Count - 1, lines[lines.Count - 1].Text.Length);
            }
            else
            {
                // find line with linear search
                int lineIndex = -1;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (coordinates.y >= lines[i].VerticalOffset - lines[i].Height)
                    {
                        lineIndex = i;
                        break;
                    }
                }

                if (lineIndex == -1)
                    lineIndex = lines.Count - 1;

                // get text position on this line
                return lines[lineIndex].GetTextPosition(coordinates);
            }
        }

        // updates caret and selection marking
        // must be invoked whenever the selection has changed
        void UpdateSelectionAndCaret()
        {
            // remove all selection rects
            foreach (GameObject selectionRect in selectionRects)
                Destroy(selectionRect);

            if (selection != null && selection.IsValid)
            {
                Vector2 startPos = lines[selection.start.lineIndex].GetCaretPosition(selection.start);
                Vector2 endPos = lines[selection.end.lineIndex].GetCaretPosition(selection.end);

                // switch startPos and endPos if selection is reversed
                if ((Mathf.Approximately(startPos.y, endPos.y) && startPos.x > endPos.x) || (!Mathf.Approximately(startPos.y, endPos.y) && startPos.y < endPos.y))
                {
                    Vector2 tmpPos = endPos;
                    endPos = startPos;
                    startPos = tmpPos;
                }

                if (Mathf.Approximately(startPos.y, endPos.y))
                {
                    // single line
                    AddSelectionRect(startPos, new Vector2(endPos.x, endPos.y - characterHeight));
                }
                else
                {
                    float maxSelectionWidth = Mathf.Max(Mathf.FloorToInt(horizontalSpaceAvailable / characterWidth) * characterWidth, longestLineWidth);

                    // first line
                    AddSelectionRect(startPos, new Vector2(maxSelectionWidth, startPos.y - characterHeight));

                    // intermediate lines
                    if (!Mathf.Approximately(startPos.y - characterHeight, endPos.y))
                        AddSelectionRect(new Vector2(0f, startPos.y - characterHeight), new Vector2(maxSelectionWidth, endPos.y));

                    // last line
                    AddSelectionRect(new Vector2(0f, endPos.y), new Vector2(endPos.x, endPos.y - characterHeight));
                }
            }
            else
            {
                selection = null;

                // update caret
                Vector2 pos = caretTextPosition != null && caretTextPosition.lineIndex < lines.Count ? lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition) : Vector2.zero;
                caret.GetComponent<Image>().color = caretColor;
                caret.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(mainMarginLeft, -mainMarginTop);

                if (Application.isPlaying)
                    caret.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(caretWidth, 1f / canvasScale), characterHeight / lineSpacing);
            }
        }

        // moves the scrollable content
        // must be invoked whenever the content is scrolled (vertically or horizontally)
        void UpdateScrollableContent()
        {
            verticalOffset = Mathf.Max(0f, Mathf.Min(verticalOffset, verticalSpaceRequired - verticalSpaceAvailable));
            horizontalOffset = Mathf.Min(0f, Mathf.Max(horizontalOffset, horizontalSpaceAvailable - horizontalSpaceRequired));

            mainContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset);
            lineNumberContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, verticalOffset);
            lineLabelIconsContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, verticalOffset);
        }

        // updates the scrollbars
        // must be invoked whenever the content is scrolled (vertically or horizontally)
        void UpdateScrollbars()
        {
            // vertical scrollbar
            if (verticalSpaceRequired <= verticalSpaceAvailable)
            {
                verticalScrollbar.size = 1f;
                verticalScrollbar.value = 0f;
                verticalScrollbar.interactable = false;
            }
            else
            {
                verticalScrollbar.size = verticalSpaceAvailable / verticalSpaceRequired;
                verticalScrollbar.value = verticalOffset / (verticalSpaceRequired - verticalSpaceAvailable);
                verticalScrollbar.interactable = true;
            }

            // horizontal scrollbar
            if (horizontalSpaceRequired <= horizontalSpaceAvailable)
            {
                horizontalScrollbar.size = 1f;
                horizontalScrollbar.value = 0f;
                horizontalScrollbar.interactable = false;
            }
            else
            {
                horizontalScrollbar.size = horizontalSpaceAvailable / horizontalSpaceRequired;
                horizontalScrollbar.value = horizontalOffset / (horizontalSpaceAvailable - horizontalSpaceRequired);
                horizontalScrollbar.interactable = true;
            }
        }

        // gets current editor state
        State GetEditorState()
        {
            return new State(caretTextPosition, selection);
        }

        // applies a given editor state
        void ApplyEditorState(State state)
        {
            caretTextPosition = state.caretTextPosition.Clone();
            selection = state.selection != null ? state.selection.Clone() : null;
            UpdateSelectionAndCaret();
        }

        // adds editor event to history
        void AddEventToHistory(History.Event newEvent)
        {
            // abort if history is disabled
            if (!enableHistory)
                return;

            // link new event
            if (currentEvent != null)
            {
                currentEvent.next = newEvent;
                newEvent.previous = currentEvent;
            }

            currentEvent = newEvent;
        }

        // adds milestone event to history
        // only if most current entry is not already a milestone
        void AddMilestoneToHistory()
        {
            if (!enableHistory)
                return;

            // abort if current event already is a milestone
            if (currentEvent != null && currentEvent is Milestone)
                return;

            // add new milestone to history
            AddEventToHistory(new Milestone());

            // limit history
            if (maxHistoryLength > 0)
            {
                // count number of actions
                int historyLength = 0;
                History.Event e = currentEvent;
                while (e != null)
                {
                    // count only milestones
                    if (e is Milestone)
                        historyLength++;

                    // cut link to previous event if history is too long
                    if (historyLength > maxHistoryLength)
                    {
                        if (e.previous != null)
                            e.previous.next = null;
                        e.previous = null;
                        break;
                    }

                    e = e.previous;
                }
            }
        }

        // executes a given operation and returns true if it has completed
        bool ExecuteOperation(IOperation op)
        {
            // invoke method based on operation type
            if (op is CopyOperation)
                return ExecuteCopyOperation((CopyOperation)op);
            else if (op is CutOperation)
                return ExecuteCutOperation((CutOperation)op);
            else if (op is DeleteOperation)
                return ExecuteDeleteOperation((DeleteOperation)op);
            else if (op is DeleteTextOperation)
                return ExecuteDeleteTextOperation((DeleteTextOperation)op);
            else if (op is FindOperation)
                return ExecuteFindOperation((FindOperation)op);
            else if (op is InsertCharacterOperation)
                return ExecuteInsertCharacterOperation((InsertCharacterOperation)op);
            else if (op is InsertTextOperation)
                return ExecuteInsertTextOperation((InsertTextOperation)op);
            else if (op is ModifyIndentOperation)
                return ExecuteModifyIndentOperation((ModifyIndentOperation)op);
            else if (op is MoveCaretOperation)
                return ExecuteMoveCaretOperation((MoveCaretOperation)op);
            else if (op is PasteOperation)
                return ExecutePasteOperation((PasteOperation)op);
            else if (op is PlaceCaretOperation)
                return ExecutePlaceCaretOperation((PlaceCaretOperation)op);
            else if (op is RebuildLinesOperation)
                return ExecuteRebuildLinesOperation((RebuildLinesOperation)op);
            else if (op is RedoOperation)
                return ExecuteRedoOperation((RedoOperation)op);
            else if (op is SelectAllOperation)
                return ExecuteSelectAllOperation((SelectAllOperation)op);
            else if (op is SetSelectionOperation)
                return ExecuteSetSelectionOperation((SetSelectionOperation)op);
            else if (op is SetTextOperation)
                return ExecuteSetTextOperation((SetTextOperation)op);
            else if (op is UndoOperation)
                return ExecuteUndoOperation((UndoOperation)op);
            else
                throw new UnityException("Invalid operation");
        }

        // executes set text operation
        bool ExecuteSetTextOperation(SetTextOperation op)
        {
            if (op.state == SetTextOperation.State.DELETING)
            {
                // reset longest line width
                longestLineWidth = 0f;

                // remove all lines
                if (lines.Count > 0)
                {
                    lines[0].Destroy();
                    lines.RemoveAt(0);
                }
                else
                    op.state = SetTextOperation.State.INSERTING;
            }

            if (op.state == SetTextOperation.State.INSERTING)
            {
                if (op.remainingText.Length > 0)
                {
                    // get text until next line break
                    string textRow = "";
                    int nextLinebreakIndex = op.remainingText.IndexOf('\n');
                    if (nextLinebreakIndex >= 0)
                    {
                        textRow = op.remainingText.Substring(0, nextLinebreakIndex);
                        op.remainingText = op.remainingText.Substring(nextLinebreakIndex + 1);
                    }
                    else
                    {
                        textRow = op.remainingText;
                        op.remainingText = "";
                    }

                    // remove all non-printable characters
                    for (int i = 0; i < textRow.Length; i++)
                    {
                        if (!Util.IsPrintableCharacter(textRow[i]) && textRow[i] != '\n' && textRow[i] != '\t')
                        {
                            textRow = textRow.Remove(i, 1);
                            i--;
                        }
                    }

                    // replace tabs
                    if (replaceTabsBySpaces)
                        textRow = Util.ReplaceTabsWithSpaces(textRow, tabStopWidth);

                    // insert new line
                    Line line = new Line(lines.Count, textRow, op.tmpOffset, this);
                    lines.Add(line);
                    longestLineWidth = Mathf.Max(longestLineWidth, line.Width);
                    op.tmpOffset -= line.Height;
                }
                else
                {
                    // add empty line if no lines have been added
                    if (lines.Count == 0)
                        lines.Add(new Line(0, "", 0f, this));

                    // continue with cleanup
                    op.state = SetTextOperation.State.CLEANUP;
                }
            }

            if (op.state == SetTextOperation.State.CLEANUP)
            {
                // update layout
                UpdateLayout();

                // purge history
                currentEvent = null;
                AddMilestoneToHistory();

                // reset formatting pointer
                textFormatLinePointer = 0;

                // operation complete
                return true;
            }

            return false;
        }

        // executes insert text operation
        bool ExecuteInsertTextOperation(InsertTextOperation op)
        {
            if (op.state == InsertTextOperation.State.START)
            {
                // skip insertion if text is null or empty
                if (string.IsNullOrEmpty(op.text))
                {
                    op.state = InsertTextOperation.State.CLEANUP;
                    return false;
                }

                // store editor state before insertion
                op.editorStateBefore = enableHistory && op.addToHistory ? GetEditorState() : null;
                op.startTextPosition = op.textPosition.Clone();

                if (op.text.Contains("\n"))
                {
                    // store old line height and width
                    op.oldLineHeight = lines[op.textPosition.lineIndex].Height;
                    op.oldLineWidth = lines[op.textPosition.lineIndex].Width;

                    // split existing line
                    op.before = lines[op.textPosition.lineIndex].Text.Substring(0, op.textPosition.colIndex);
                    op.after = lines[op.textPosition.lineIndex].Text.Substring(op.textPosition.colIndex);

                    // split input
                    op.textLines = op.text.Split('\n');

                    // continue with insert first line
                    op.state = InsertTextOperation.State.INSERT_FIRST_LINE;
                }
                else
                {
                    // continue with insert single line
                    op.state = InsertTextOperation.State.INSERT_SINGLE_LINE;
                }
            }

            if (op.state == InsertTextOperation.State.INSERT_SINGLE_LINE)
            {
                // store old line height and width
                float oldLineHeight = lines[op.textPosition.lineIndex].Height;
                float oldLineWidth = lines[op.textPosition.lineIndex].Width;
                int oldLineLength = lines[op.textPosition.lineIndex].Text.Length;

                // insert text
                lines[op.textPosition.lineIndex].Text = replaceTabsBySpaces ? Util.ReplaceTabsWithSpaces(lines[op.textPosition.lineIndex].Text.Insert(op.textPosition.colIndex, op.text), tabStopWidth) : lines[op.textPosition.lineIndex].Text.Insert(op.textPosition.colIndex, op.text);

                // update longestLineWidth
                longestLineWidth = Mathf.Max(longestLineWidth, lines[op.textPosition.lineIndex].Width);
                if (lines[op.textPosition.lineIndex].Width >= oldLineWidth)
                    op.recalculateLongestLineWidth = false;

                // if line height has changed, update vertical offset in all following lines
                if (!Mathf.Approximately(lines[op.textPosition.lineIndex].Height, oldLineHeight))
                {
                    float tmpOffset = lines[op.textPosition.lineIndex].VerticalOffset - lines[op.textPosition.lineIndex].Height;
                    for (int i = op.textPosition.lineIndex + 1; i < lines.Count; i++)
                    {
                        lines[i].VerticalOffset = tmpOffset;
                        tmpOffset -= lines[i].Height;
                    }
                }

                // move caret to end of inserted text
                caretTextPosition = new TextPosition(op.textPosition.lineIndex, op.textPosition.colIndex + lines[op.textPosition.lineIndex].Text.Length - oldLineLength);
                preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;

                // notify previous and next lines
                for (int i = 0; i < op.textPosition.lineIndex; i++)
                    lines[i].OnNextLineChanged();
                for (int i = op.textPosition.lineIndex + 1; i < lines.Count; i++)
                    lines[i].OnPreviousLineChanged();

                // modify selection
                if (selection != null && selection.start.lineIndex == op.startTextPosition.lineIndex && selection.start.colIndex >= op.startTextPosition.colIndex)
                    selection.start.colIndex += lines[op.textPosition.lineIndex].Text.Length - oldLineLength;
                if (selection != null && selection.end.lineIndex == op.startTextPosition.lineIndex && selection.end.colIndex >= op.startTextPosition.colIndex)
                    selection.end.colIndex += lines[op.textPosition.lineIndex].Text.Length - oldLineLength;

                // continue with cleanup
                op.state = InsertTextOperation.State.CLEANUP;
            }

            if (op.state == InsertTextOperation.State.INSERT_FIRST_LINE)
            {
                // add first line
                lines[op.textPosition.lineIndex].Text = replaceTabsBySpaces ? Util.ReplaceTabsWithSpaces(op.before + op.textLines[0], tabStopWidth) : op.before + op.textLines[0];
                longestLineWidth = Mathf.Max(longestLineWidth, lines[op.textPosition.lineIndex].Width);
                if (lines[op.textPosition.lineIndex].Width >= op.oldLineWidth)
                    op.recalculateLongestLineWidth = false;
                op.tmpOffset = lines[op.textPosition.lineIndex].VerticalOffset - lines[op.textPosition.lineIndex].Height;
                op.state = InsertTextOperation.State.INSERT_INTERMEDIATE_LINE;

                // notify previous lines
                for (int i = 0; i < op.textPosition.lineIndex; i++)
                    lines[i].OnNextLineChanged();
            }

            if (op.state == InsertTextOperation.State.INSERT_INTERMEDIATE_LINE)
            {
                // add intermediate lines
                if (op.lineIndex < op.textLines.Length - 1)
                {
                    Line intermediateLine = new Line(op.textPosition.lineIndex + op.lineIndex, replaceTabsBySpaces ? Util.ReplaceTabsWithSpaces(op.textLines[op.lineIndex], tabStopWidth) : op.textLines[op.lineIndex], op.tmpOffset, this);
                    lines.Insert(op.textPosition.lineIndex + op.lineIndex, intermediateLine);
                    longestLineWidth = Mathf.Max(longestLineWidth, intermediateLine.Width);
                    if (intermediateLine.Width >= op.oldLineWidth)
                        op.recalculateLongestLineWidth = false;
                    op.tmpOffset -= intermediateLine.Height;
                    op.lineIndex++;
                }
                else
                    op.state = InsertTextOperation.State.INSERT_LAST_LINE;
            }

            if (op.state == InsertTextOperation.State.INSERT_LAST_LINE)
            {
                // insert last line
                Line lastLine = new Line(op.textPosition.lineIndex + op.textLines.Length - 1, replaceTabsBySpaces ? Util.ReplaceTabsWithSpaces(op.textLines[op.textLines.Length - 1] + op.after, tabStopWidth) : op.textLines[op.textLines.Length - 1] + op.after, op.tmpOffset, this);
                lines.Insert(op.textPosition.lineIndex + op.textLines.Length - 1, lastLine);
                longestLineWidth = Mathf.Max(longestLineWidth, lastLine.Width);
                if (lastLine.Width >= op.oldLineWidth)
                    op.recalculateLongestLineWidth = false;

                // update vertical offset and line number in all following lines
                float tmpOffset = lastLine.VerticalOffset - lastLine.Height;
                for (int i = op.textPosition.lineIndex + op.textLines.Length; i < lines.Count; i++)
                {
                    lines[i].VerticalOffset = tmpOffset;
                    tmpOffset -= lines[i].Height;
                    lines[i].LineNumber = i;

                    // notify next line
                    lines[i].OnPreviousLineChanged();
                }

                // move caret to end of inserted text
                caretTextPosition = new TextPosition(op.textPosition.lineIndex + op.textLines.Length - 1, (replaceTabsBySpaces ? Util.ReplaceTabsWithSpaces(op.textLines[op.textLines.Length - 1], tabStopWidth) : op.textLines[op.textLines.Length - 1]).Length);
                preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;

                op.state = InsertTextOperation.State.CLEANUP;
            }

            if (op.state == InsertTextOperation.State.CLEANUP)
            {
                // recalculate longest line width
                if (op.recalculateLongestLineWidth)
                {
                    longestLineWidth = 0f;
                    for (int i = 0; i < lines.Count; i++)
                        longestLineWidth = Mathf.Max(longestLineWidth, lines[i].Width);
                }

                // update layout
                UpdateLayout();

                // add insert action to history
                if (enableHistory && op.addToHistory)
                    AddEventToHistory(new Insert(op.startTextPosition, caretTextPosition, op.text, op.editorStateBefore, GetEditorState()));

                // reset formatting pointer
                textFormatLinePointer = Mathf.Min(textFormatLinePointer, op.textPosition.lineIndex);

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes insert character operation
        bool ExecuteInsertCharacterOperation(InsertCharacterOperation op)
        {
            if (op.state == InsertCharacterOperation.State.START)
            {
                if (selection != null)
                {
                    // add milestone to history before deleting selected text
                    AddMilestoneToHistory();

                    // create DeleteTextOperation
                    DeleteTextOperation deleteTextOp = new DeleteTextOperation(selection, true);
                    op.deleteTextOp = deleteTextOp;
                    op.state = InsertCharacterOperation.State.DELETE;
                }
                else
                    op.state = InsertCharacterOperation.State.INSERT;
            }

            if (op.state == InsertCharacterOperation.State.DELETE)
            {
                // execute delete text operation until done
                if (ExecuteDeleteTextOperation(op.deleteTextOp))
                    op.state = InsertCharacterOperation.State.INSERT;
            }

            if (op.state == InsertCharacterOperation.State.INSERT)
            {
                // add milestone if character is a white space, tab, or new line
                if (op.character == ' ' || op.character == '\t' || op.character == '\n')
                    AddMilestoneToHistory();

                // insert text and add to history
                if (op.character == '\n')
                {
                    string lineIndent = "";
                    if (indentNewLines)
                    {
                        // indent new line with previous line's indent
                        for (int i = 0; i < lines[caretTextPosition.lineIndex].LineIndent / tabStopWidth; i++)
                            lineIndent += '\t';
                    }

                    InsertText(caretTextPosition, op.character.ToString() + lineIndent, true, true);
                }
                else
                    InsertText(caretTextPosition, op.character.ToString(), true, true);

                // add milestone if character is a white space, tab, or new line
                if (op.character == ' ' || op.character == '\t' || op.character == '\n')
                    AddMilestoneToHistory();

                // reset caret blinking
                caretBlinkTime = 0f;
                caretVisible = false;

                // follow caret
                FollowCaret();

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes delete text operation
        bool ExecuteDeleteTextOperation(DeleteTextOperation op)
        {
            if (op.state == DeleteTextOperation.State.START)
            {
                if (!op.deleteSelection.IsValid)
                    throw new UnityException("Invalid selection");

                // save state before delete
                op.editorStateBefore = enableHistory && op.addToHistory ? GetEditorState() : null;
                op.selectedText = enableHistory && op.addToHistory ? GetSelectedText(op.deleteSelection) : null;

                if (op.deleteSelection.start.lineIndex != op.deleteSelection.end.lineIndex)
                {
                    // store remainders of first and last line
                    op.before = lines[op.deleteSelection.start.lineIndex].Text.Substring(0, op.deleteSelection.start.colIndex);
                    op.after = lines[op.deleteSelection.end.lineIndex].Text.Substring(op.deleteSelection.end.colIndex);

                    // continue with delete first line
                    op.state = DeleteTextOperation.State.DELETE_FIRST_LINE;
                }
                else
                    op.state = DeleteTextOperation.State.DELETE_SINGLE_LINE;
            }

            if (op.state == DeleteTextOperation.State.DELETE_SINGLE_LINE)
            {
                // store old line height and width
                float oldLineHeight = lines[op.deleteSelection.start.lineIndex].Height;
                float oldLineWidth = lines[op.deleteSelection.start.lineIndex].Width;

                // modify line
                string before = lines[op.deleteSelection.start.lineIndex].Text.Substring(0, op.deleteSelection.start.colIndex);
                string after = lines[op.deleteSelection.end.lineIndex].Text.Substring(op.deleteSelection.end.colIndex);
                lines[op.deleteSelection.start.lineIndex].Text = before + after;

                // recalculate longest line width if this line was a longest line and is now shorter
                if (lines[op.deleteSelection.start.lineIndex].Width < oldLineWidth && Mathf.Approximately(longestLineWidth, oldLineWidth))
                    op.recalculateLongestLineWidth = true;

                // if line height has changed, update vertical offset in all following lines
                if (!Mathf.Approximately(lines[op.deleteSelection.start.lineIndex].Height, oldLineHeight))
                {
                    float tmpOffset = lines[op.deleteSelection.start.lineIndex].VerticalOffset - lines[op.deleteSelection.start.lineIndex].Height;
                    for (int i = op.deleteSelection.start.lineIndex + 1; i < lines.Count; i++)
                    {
                        lines[i].VerticalOffset = tmpOffset;
                        tmpOffset -= lines[i].Height;
                    }
                }

                // move caret to start of delete selection
                caretTextPosition = op.deleteSelection.start;

                if (op.deleteSelection.Equals(selection))
                {
                    // remove selection
                    selection = null;
                }
                else if (selection != null && selection.IsValid)
                {
                    int deletedCharacters = op.deleteSelection.end.colIndex - op.deleteSelection.start.colIndex;

                    // modify selection
                    if (selection.start.lineIndex == op.deleteSelection.start.lineIndex && selection.start.colIndex >= op.deleteSelection.start.colIndex)
                        selection.start.colIndex = Mathf.Max(selection.start.colIndex - deletedCharacters, op.deleteSelection.start.colIndex);
                    if (selection.end.lineIndex == op.deleteSelection.start.lineIndex && selection.end.colIndex >= op.deleteSelection.start.colIndex)
                        selection.end.colIndex = Mathf.Max(selection.end.colIndex - deletedCharacters, op.deleteSelection.start.colIndex);

                    // remove selection if invalid
                    if (!selection.IsValid)
                        selection = null;
                }

                // continue with cleanup
                op.state = DeleteTextOperation.State.CLEANUP;
            }

            if (op.state == DeleteTextOperation.State.DELETE_FIRST_LINE)
            {
                // modify selected line
                float oldFirstLineWidth = lines[op.deleteSelection.start.lineIndex].Width;
                lines[op.deleteSelection.start.lineIndex].Text = op.before + op.after;
                longestLineWidth = Mathf.Max(longestLineWidth, lines[op.deleteSelection.start.lineIndex].Width);
                if (lines[op.deleteSelection.start.lineIndex].Width < oldFirstLineWidth && Mathf.Approximately(longestLineWidth, oldFirstLineWidth))
                    op.recalculateLongestLineWidth = true;
                op.state = DeleteTextOperation.State.DELETE_INTERMEDIATE_OR_LAST_LINE;
            }

            if (op.state == DeleteTextOperation.State.DELETE_INTERMEDIATE_OR_LAST_LINE)
            {
                // delete intermediate or last line
                if (op.lineIndex < op.deleteSelection.end.lineIndex - op.deleteSelection.start.lineIndex)
                {
                    if (Mathf.Approximately(longestLineWidth, lines[op.deleteSelection.start.lineIndex + 1].Width))
                        op.recalculateLongestLineWidth = true;
                    lines[op.deleteSelection.start.lineIndex + 1].Destroy();
                    lines.RemoveAt(op.deleteSelection.start.lineIndex + 1);
                    op.lineIndex++;
                }
                else
                {
                    // update vertical offset and line number in all following lines
                    float tmpOffset = lines[op.deleteSelection.start.lineIndex].VerticalOffset - lines[op.deleteSelection.start.lineIndex].Height;
                    for (int i = op.deleteSelection.start.lineIndex + 1; i < lines.Count; i++)
                    {
                        lines[i].VerticalOffset = tmpOffset;
                        tmpOffset -= lines[i].Height;
                        lines[i].LineNumber = i;
                    }

                    // move caret to start of delete selection and remove selection
                    caretTextPosition = op.deleteSelection.start;
                    selection = null;

                    // continue with cleanup
                    op.state = DeleteTextOperation.State.CLEANUP;
                }
            }

            if (op.state == DeleteTextOperation.State.CLEANUP)
            {
                // recalculate longest line width
                if (op.recalculateLongestLineWidth)
                {
                    longestLineWidth = 0f;
                    for (int i = 0; i < lines.Count; i++)
                        longestLineWidth = Mathf.Max(longestLineWidth, lines[i].Width);
                }

                // notify previous and next lines
                for (int i = 0; i < op.deleteSelection.start.lineIndex; i++)
                    lines[i].OnNextLineChanged();
                for (int i = op.deleteSelection.start.lineIndex + 1; i < lines.Count; i++)
                    lines[i].OnPreviousLineChanged();

                // update layout
                UpdateLayout();

                // add delete action to history
                if (enableHistory && op.addToHistory)
                    AddEventToHistory(new Delete(op.deleteSelection, op.selectedText, op.editorStateBefore, GetEditorState()));

                // reset formatting pointer
                textFormatLinePointer = Mathf.Min(textFormatLinePointer, op.deleteSelection.start.lineIndex);

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes delete operation
        bool ExecuteDeleteOperation(DeleteOperation op)
        {
            // add milestone to history before deleting text
            AddMilestoneToHistory();

            if (op.state == DeleteOperation.State.START)
            {
                if (selection != null)
                {
                    // create delete text operation with selection
                    DeleteTextOperation deleteTextOp = new DeleteTextOperation(selection, true);
                    op.deleteTextOp = deleteTextOp;
                    op.state = DeleteOperation.State.DELETE;
                }
                else
                {
                    // create delete selection including the character before or after the caret
                    Selection deleteSelection = null;
                    if (op.forward)
                    {
                        if (caretTextPosition.colIndex < lines[caretTextPosition.lineIndex].Text.Length)
                        {
                            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                                deleteSelection = new Selection(caretTextPosition, lines[caretTextPosition.lineIndex].FindWordStart(new TextPosition(caretTextPosition.lineIndex, caretTextPosition.colIndex + 1), true));
                            else
                                deleteSelection = new Selection(caretTextPosition, new TextPosition(caretTextPosition.lineIndex, caretTextPosition.colIndex + 1));
                        }
                        else if (caretTextPosition.lineIndex < lines.Count - 1)
                            deleteSelection = new Selection(caretTextPosition, new TextPosition(caretTextPosition.lineIndex + 1, 0));
                    }
                    else
                    {
                        if (caretTextPosition.colIndex > 0)
                        {
                            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                                deleteSelection = new Selection(lines[caretTextPosition.lineIndex].FindWordStart(new TextPosition(caretTextPosition.lineIndex, caretTextPosition.colIndex - 1, true), false), caretTextPosition);
                            else
                                deleteSelection = new Selection(new TextPosition(caretTextPosition.lineIndex, caretTextPosition.colIndex - 1, true), caretTextPosition);
                        }
                        else if (caretTextPosition.lineIndex > 0)
                            deleteSelection = new Selection(new TextPosition(caretTextPosition.lineIndex - 1, lines[caretTextPosition.lineIndex - 1].Text.Length), caretTextPosition);
                    }

                    // delete text
                    if (deleteSelection != null)
                    {
                        DeleteTextOperation deleteTextOp = new DeleteTextOperation(deleteSelection, true);

                        op.deleteTextOp = deleteTextOp;
                        op.state = DeleteOperation.State.DELETE;
                    }
                    else
                        op.state = DeleteOperation.State.CLEANUP;
                }
            }

            if (op.state == DeleteOperation.State.DELETE)
            {
                // execute delete text operation
                if (ExecuteDeleteTextOperation(op.deleteTextOp))
                    op.state = DeleteOperation.State.CLEANUP;
            }

            if (op.state == DeleteOperation.State.CLEANUP)
            {
                // add milestone
                AddMilestoneToHistory();

                // reset caret blinking
                caretBlinkTime = 0f;
                caretVisible = false;

                // follow caret
                FollowCaret();

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes increase indent operation
        bool ExecuteModifyIndentOperation(ModifyIndentOperation op)
        {
            if (op.state == ModifyIndentOperation.State.START)
            {
                // abort if selection is invalid
                if (selection == null || !selection.IsValid)
                    return true;

                // add milestone to history before modifying indent
                AddMilestoneToHistory();

                op.startLineIndex = selection.IsReversed ? selection.end.lineIndex : selection.start.lineIndex;
                op.endLineIndex = selection.IsReversed ? selection.start.lineIndex : selection.end.lineIndex;
                op.lineIndex = op.endLineIndex;

                op.state = ModifyIndentOperation.State.MODIFY_INDENT;
            }

            if (op.state == ModifyIndentOperation.State.MODIFY_INDENT)
            {
                if (op.increase)
                {
                    // increase indent
                    InsertText(new TextPosition(op.lineIndex, 0), "\t", true, true);
                }
                else
                {
                    // decrease indent
                    if (lines[op.lineIndex].Text.StartsWith("\t", System.StringComparison.Ordinal))
                        DeleteText(new Selection(new TextPosition(op.lineIndex, 0), new TextPosition(op.lineIndex, 1)), true, true);
                    else if (lines[op.lineIndex].Text.StartsWith(" ", System.StringComparison.Ordinal))
                    {
                        // count number of leading spaces
                        int numberOfSpaces = 1;
                        for (int i = 1; i < tabStopWidth; i++)
                        {
                            if (lines[op.lineIndex].Text.Length > i && lines[op.lineIndex].Text[i] == ' ')
                                numberOfSpaces++;
                            else
                                break;
                        }

                        DeleteText(new Selection(new TextPosition(op.lineIndex, 0), new TextPosition(op.lineIndex, numberOfSpaces)), true, true);
                    }
                }

                if (op.lineIndex > op.startLineIndex)
                {
                    // continue with next line
                    op.lineIndex--;
                    return false;
                }
                else
                {
                    // continue with cleanup
                    op.state = ModifyIndentOperation.State.CLEANUP;
                }
            }

            if (op.state == ModifyIndentOperation.State.CLEANUP)
            {
                // add milestone to history after modifying indent
                AddMilestoneToHistory();

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes rebuild lines operation
        bool ExecuteRebuildLinesOperation(RebuildLinesOperation op)
        {
            if (op.state == RebuildLinesOperation.State.START)
            {
                // reset longest line width
                longestLineWidth = 0f;
                op.state = RebuildLinesOperation.State.REBUILD;
            }

            if (op.state == RebuildLinesOperation.State.REBUILD)
            {
                if (op.lineIndex >= lines.Count)
                {
                    // continue with cleanup
                    op.state = RebuildLinesOperation.State.CLEANUP;
                }
                else
                {
                    // update line
                    lines[op.lineIndex].VerticalOffset = op.tmpOffset;
                    lines[op.lineIndex].Text = replaceTabsBySpaces ? Util.ReplaceTabsWithSpaces(lines[op.lineIndex].Text, tabStopWidth) : lines[op.lineIndex].Text;
                    longestLineWidth = Mathf.Max(longestLineWidth, lines[op.lineIndex].Width);
                    op.tmpOffset -= lines[op.lineIndex].Height;
                    op.lineIndex++;
                }
            }

            if (op.state == RebuildLinesOperation.State.CLEANUP)
            {
                // update layout
                UpdateLayout();

                // follow caret (only in play mode)
                if (Application.isPlaying)
                    FollowCaret();

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes move caret operation
        bool ExecuteMoveCaretOperation(MoveCaretOperation op)
        {
            // add milestone
            AddMilestoneToHistory();

            // move caret up
            if (op.direction == MoveCaretOperation.Direction.UP)
            {
                if (op.select)
                {
                    // create new empty selection if none exists
                    if (selection == null)
                        selection = new Selection(caretTextPosition, caretTextPosition);

                    // get current selection end position
                    Vector2 pos = lines[selection.end.lineIndex].GetCaretPosition(selection.end);

                    // jump to top left if already on first line
                    if (Mathf.Approximately(pos.y, 0f))
                    {
                        selection.end = new TextPosition(0, 0);
                        preferredCaretX = 0f;
                    }
                    else
                        selection.end = GetTextPositionForCoordinates(new Vector2(preferredCaretX, pos.y + characterHeight * 0.5f));
                }
                else
                {
                    // jump to start of selection
                    if (selection != null)
                    {
                        if (selection.IsReversed)
                            caretTextPosition = selection.end;
                        else
                            caretTextPosition = selection.start;
                        preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;
                        selection = null;
                    }

                    // get current caret position
                    Vector2 pos = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition);

                    // jump to top left if already on first line
                    if (Mathf.Approximately(pos.y, 0f))
                    {
                        caretTextPosition = new TextPosition(0, 0);
                        preferredCaretX = 0f;
                    }
                    else
                        caretTextPosition = GetTextPositionForCoordinates(new Vector2(preferredCaretX, pos.y + characterHeight * 0.5f));
                }
            }

            // move caret down
            if (op.direction == MoveCaretOperation.Direction.DOWN)
            {
                if (op.select)
                {
                    // create new empty selection if none exists
                    if (selection == null)
                        selection = new Selection(caretTextPosition, caretTextPosition);

                    // get current selection end position
                    Vector2 pos = lines[selection.end.lineIndex].GetCaretPosition(selection.end);

                    // jump to end of text if already on last line
                    if (Mathf.Approximately(pos.y, lines[lines.Count - 1].VerticalOffset - lines[lines.Count - 1].Height + characterHeight))
                    {
                        selection.end = new TextPosition(lines.Count - 1, lines[lines.Count - 1].Text.Length);
                        preferredCaretX = lines[selection.end.lineIndex].GetCaretPosition(selection.end).x;
                    }
                    else
                        selection.end = GetTextPositionForCoordinates(new Vector2(preferredCaretX, pos.y - characterHeight * 1.5f));
                }
                else
                {
                    // jump to end of selection
                    if (selection != null)
                    {
                        if (selection.IsReversed)
                            caretTextPosition = selection.start;
                        else
                            caretTextPosition = selection.end;
                        preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;
                        selection = null;
                    }

                    // get current caret position
                    Vector2 pos = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition);

                    // jump to end of text if already on last line
                    if (Mathf.Approximately(pos.y, lines[lines.Count - 1].VerticalOffset - lines[lines.Count - 1].Height + characterHeight))
                    {
                        caretTextPosition = new TextPosition(lines.Count - 1, lines[lines.Count - 1].Text.Length);
                        preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;
                    }
                    else
                        caretTextPosition = GetTextPositionForCoordinates(new Vector2(preferredCaretX, pos.y - characterHeight * 1.5f));
                }
            }

            // move caret left
            if (op.direction == MoveCaretOperation.Direction.LEFT)
            {
                if (op.select)
                {
                    // create new empty selection if none exists
                    if (selection == null)
                        selection = new Selection(caretTextPosition, caretTextPosition);

                    // move selection end one to the left
                    if (selection.end.colIndex > 0)
                        selection.end.colIndex--;
                    else if (selection.end.lineIndex > 0)
                    {
                        selection.end.lineIndex--;
                        selection.end.colIndex = lines[selection.end.lineIndex].Text.Length;
                    }

                    // jump to start of word if alt key is pressed
                    if (op.entireWord)
                        selection.end = lines[selection.end.lineIndex].FindWordStart(selection.end, false);

                    // stay on next line if possible
                    selection.end.preferNextLine = true;

                    // store preferred horizontal position
                    preferredCaretX = lines[selection.end.lineIndex].GetCaretPosition(selection.end).x;
                }
                else
                {
                    // jump to start of selection
                    if (selection != null)
                    {
                        if (selection.IsReversed)
                            caretTextPosition = selection.end;
                        else
                            caretTextPosition = selection.start;
                        preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;
                        selection = null;
                    }
                    else
                    {
                        // move caret one to the left
                        if (caretTextPosition.colIndex > 0)
                            caretTextPosition.colIndex--;
                        else if (caretTextPosition.lineIndex > 0)
                        {
                            caretTextPosition.lineIndex--;
                            caretTextPosition.colIndex = lines[caretTextPosition.lineIndex].Text.Length;
                        }

                        // jump to start of word if alt key is pressed
                        if (op.entireWord)
                            caretTextPosition = lines[caretTextPosition.lineIndex].FindWordStart(caretTextPosition, false);

                        // stay on next line if possible
                        caretTextPosition.preferNextLine = true;

                        // store preferred horizontal position
                        preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;
                    }
                }
            }

            // move caret right
            if (op.direction == MoveCaretOperation.Direction.RIGHT)
            {
                if (op.select)
                {
                    // create new empty selection if none exists
                    if (selection == null)
                        selection = new Selection(caretTextPosition, caretTextPosition);

                    // move selection end one to the right
                    if (selection.end.colIndex < lines[selection.end.lineIndex].Text.Length)
                        selection.end.colIndex++;
                    else if (selection.end.lineIndex < lines.Count - 1)
                    {
                        selection.end.lineIndex++;
                        selection.end.colIndex = 0;
                    }

                    // jump to end of word if alt key is pressed
                    if (op.entireWord)
                        selection.end = lines[selection.end.lineIndex].FindWordEnd(selection.end, true);

                    // stay on previous line if possible
                    selection.end.preferNextLine = false;

                    // store preferred horizontal position
                    preferredCaretX = lines[selection.end.lineIndex].GetCaretPosition(selection.end).x;
                }
                else
                {
                    // jump to start of selection
                    if (selection != null)
                    {
                        if (selection.IsReversed)
                            caretTextPosition = selection.start;
                        else
                            caretTextPosition = selection.end;
                        preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;
                        selection = null;
                    }
                    else
                    {
                        // move caret one to the left
                        if (caretTextPosition.colIndex < lines[caretTextPosition.lineIndex].Text.Length)
                            caretTextPosition.colIndex++;
                        else if (caretTextPosition.lineIndex < lines.Count - 1)
                        {
                            caretTextPosition.lineIndex++;
                            caretTextPosition.colIndex = 0;
                        }

                        // jump to end of word if alt key is pressed
                        if (op.entireWord)
                            caretTextPosition = lines[caretTextPosition.lineIndex].FindWordEnd(caretTextPosition, true);

                        // stay on previous line if possible
                        caretTextPosition.preferNextLine = false;

                        // store preferred horizontal position
                        preferredCaretX = lines[caretTextPosition.lineIndex].GetCaretPosition(caretTextPosition).x;
                    }
                }
            }

            // reset caret blinking
            caretBlinkTime = 0f;
            caretVisible = false;

            // update and follow caret
            UpdateSelectionAndCaret();
            FollowCaret();

            // operation complete
            return true;
        }

        // executes place caret operation
        bool ExecutePlaceCaretOperation(PlaceCaretOperation op)
        {
            // remove selection
            selection = null;

            // set caret position and store preferred x position
            caretTextPosition = op.textPosition;
            preferredCaretX = lines[op.textPosition.lineIndex].GetCaretPosition(op.textPosition).x;
            UpdateSelectionAndCaret();

            // operation completed
            return true;
        }

        // executes set selection operation
        bool ExecuteSetSelectionOperation(SetSelectionOperation op)
        {
            // apply selection
            selection = op.selection;

            // follow caret and update
            FollowCaret();
            UpdateSelectionAndCaret();

            // operation completed
            return true;
        }

        // executes select all operation
        bool ExecuteSelectAllOperation(SelectAllOperation op)
        {
            // do nothing if document is empty
            if (lines.Count == 1 && lines[0].Text.Length == 0)
                return true;

            // apply selection covering the entire content
            SetSelection(new Selection(new TextPosition(0, 0), new TextPosition(lines.Count - 1, lines[lines.Count - 1].Text.Length)), true);

            // operation complete
            return true;
        }

        // executes copy operation
        bool ExecuteCopyOperation(CopyOperation op)
        {
            // get selected text and copy it to the system clipboard
            if (selection != null && selection.IsValid)
                GUIUtility.systemCopyBuffer = GetSelectedText(selection);

            // operation complete
            return true;
        }

        // executes cut operation
        bool ExecuteCutOperation(CutOperation op)
        {
            if (op.state == CutOperation.State.START)
            {
                // operation complete if no valid selection
                if (selection == null || !selection.IsValid)
                    return true;

                // add milestone to history before deleting text
                AddMilestoneToHistory();

                // copy text
                Copy(true);

                // create delete text operation
                DeleteTextOperation deleteTextOp = new DeleteTextOperation(selection, true);
                op.deleteTextOp = deleteTextOp;

                // continue in delete state
                op.state = CutOperation.State.DELETE;
            }

            if (op.state == CutOperation.State.DELETE)
            {
                // execute delete text operation until complete, then continue with cleanup
                if (ExecuteDeleteTextOperation(op.deleteTextOp))
                    op.state = CutOperation.State.CLEANUP;
            }

            if (op.state == CutOperation.State.CLEANUP)
            {
                // add milestone after text has been removed
                AddMilestoneToHistory();

                // follow caret
                FollowCaret();

                // reset formatting pointer
                textFormatLinePointer = Mathf.Min(textFormatLinePointer, caretTextPosition.lineIndex);

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes paste operation
        bool ExecutePasteOperation(PasteOperation op)
        {
            if (op.state == PasteOperation.State.START)
            {
                // add milestone to history before text is inserted
                AddMilestoneToHistory();

                // get clipboard text
                op.clipboardText = GUIUtility.systemCopyBuffer;

                // abort if clipboard is empty
                if (op.clipboardText.Length == 0)
                    return true;

                // replace all variations of line feed characters by new line
                op.clipboardText = op.clipboardText.Replace("\u000D\u000A", "\n").Replace("\u000B", "\n").Replace("\u000C", "\n").Replace("\u000D", "\n").Replace("\u0085", "\n").Replace("\u2028", "\n").Replace("\u2029", "\n");

                // remove all non-printable characters
                for (int i = 0; i < op.clipboardText.Length; i++)
                {
                    if (!Util.IsPrintableCharacter(op.clipboardText[i]) && op.clipboardText[i] != '\n' && op.clipboardText[i] != '\t')
                    {
                        op.clipboardText = op.clipboardText.Remove(i, 1);
                        i--;
                    }
                }

                // abort if there is no valid text to be inserted
                if (op.clipboardText.Length == 0)
                    return true;

                // create insert text operation with clipboard text
                op.insertTextOp = new InsertTextOperation(caretTextPosition, op.clipboardText, true);

                // delete existing selection
                if (selection != null && selection.IsValid)
                {
                    // create delete text operation
                    DeleteTextOperation deleteTextOp = new DeleteTextOperation(selection, true);
                    op.deleteTextOp = deleteTextOp;

                    // continue with delete
                    op.state = PasteOperation.State.DELETE;

                    // insert clipboard text at beginning of selection
                    op.insertTextOp = new InsertTextOperation(selection.IsReversed ? selection.end : selection.start, op.clipboardText, true);
                }
                else
                {
                    // create insert text operation to insert clipboard text at caret text position
                    op.insertTextOp = new InsertTextOperation(caretTextPosition, op.clipboardText, true);

                    // continue with insert
                    op.state = PasteOperation.State.INSERT;
                }
            }

            if (op.state == PasteOperation.State.DELETE)
            {
                // execute delete text operation until complete, then continue with insert
                if (ExecuteDeleteTextOperation(op.deleteTextOp))
                    op.state = PasteOperation.State.INSERT;
            }

            if (op.state == PasteOperation.State.INSERT)
            {
                // execute insert text operation until complete, then continue with cleanup
                if (ExecuteInsertTextOperation(op.insertTextOp))
                    op.state = PasteOperation.State.CLEANUP;
            }

            if (op.state == PasteOperation.State.CLEANUP)
            {
                // add milestone to history after text has been inserted
                AddMilestoneToHistory();

                // follow caret
                FollowCaret();

                // reset formatting pointer
                textFormatLinePointer = Mathf.Min(textFormatLinePointer, op.insertTextOp.textPosition.lineIndex);

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes undo operation
        bool ExecuteUndoOperation(UndoOperation op)
        {
            if (op.state == UndoOperation.State.START)
            {
                // abort if history is disabled or empty
                if (!enableHistory || currentEvent == null)
                    return true;

                // add milestone before reverting action
                AddMilestoneToHistory();

                // set current event and traverse history
                op.e = currentEvent;
                op.state = UndoOperation.State.TRAVERSE_HISTORY;
            }

            if (op.state == UndoOperation.State.TRAVERSE_HISTORY)
            {
                if (op.e == null)
                {
                    // continue with cleanup
                    op.state = UndoOperation.State.CLEANUP;
                }
                else if (op.e is Action)
                {
                    if (op.e is Insert)
                    {
                        // delete inserted text
                        Insert insertAction = (Insert)op.e;
                        DeleteTextOperation deleteTextOp = new DeleteTextOperation(new Selection(insertAction.startTextPosition, insertAction.endTextPosition), false);
                        op.revertedOperation = deleteTextOp;
                    }
                    else if (op.e is Delete)
                    {
                        // insert deleted text
                        Delete deleteAction = (Delete)op.e;
                        InsertTextOperation insertTextOp = new InsertTextOperation(deleteAction.selection.start, deleteAction.deletedText, false);
                        op.revertedOperation = insertTextOp;
                    }
                    else
                        throw new UnityException("Invalid action");

                    // continue with revert action
                    op.state = UndoOperation.State.REVERT_ACTION;
                }
                else
                {
                    // continue with previous event
                    op.e = op.e.previous;
                }
            }

            if (op.state == UndoOperation.State.REVERT_ACTION)
            {
                // execute reverted operation
                bool completed = false;
                completed = ExecuteOperation(op.revertedOperation);

                if (completed)
                {
                    // apply editor state before reverted action
                    ApplyEditorState(((Action)op.e).stateBefore);

                    // find previous event
                    op.e = op.e.previous;

                    if (op.e == null || op.e is Milestone)
                    {
                        // continue with cleanup
                        currentEvent = op.e;
                        op.state = UndoOperation.State.CLEANUP;
                    }
                    else
                    {
                        // further traverse history
                        op.state = UndoOperation.State.TRAVERSE_HISTORY;
                    }
                }
            }

            if (op.state == UndoOperation.State.CLEANUP)
            {
                // follow caret
                FollowCaret();

                // reset formatting pointer
                if (op.e is Insert)
                    textFormatLinePointer = Mathf.Min(textFormatLinePointer, ((Insert)op.e).startTextPosition.lineIndex);
                else if (op.e is Delete)
                    textFormatLinePointer = Mathf.Min(textFormatLinePointer, ((Delete)op.e).selection.start.lineIndex);

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes redo operation
        bool ExecuteRedoOperation(RedoOperation op)
        {
            if (op.state == RedoOperation.State.START)
            {
                // abort if history is disabled or empty (or if already at latest event)
                if (!enableHistory || currentEvent == null || currentEvent.next == null)
                    return true;

                // add milestone to history before reapplying event
                AddMilestoneToHistory();

                // set current event and traverse history
                op.e = currentEvent;
                op.state = RedoOperation.State.TRAVERSE_HISTORY;
            }

            if (op.state == RedoOperation.State.TRAVERSE_HISTORY)
            {
                if (op.e == null)
                {
                    // continue with cleanup
                    currentEvent = op.e;
                    op.state = RedoOperation.State.CLEANUP;
                }
                else if (op.e is Action)
                {
                    if (op.e is Insert)
                    {
                        // inserted text
                        Insert insertAction = (Insert)op.e;
                        InsertTextOperation insertTextOp = new InsertTextOperation(insertAction.startTextPosition, insertAction.text, false);
                        op.appliedOperation = insertTextOp;
                    }
                    else if (op.e is Delete)
                    {
                        // delete text
                        Delete deleteAction = (Delete)op.e;
                        DeleteTextOperation deleteTextOp = new DeleteTextOperation(deleteAction.selection, false);
                        op.appliedOperation = deleteTextOp;
                    }
                    else
                        throw new UnityException("Invalid action");

                    // continue with apply action
                    op.state = RedoOperation.State.APPLY_ACTION;
                }
                else
                {
                    // continue with next event
                    op.e = op.e.next;
                }
            }

            if (op.state == RedoOperation.State.APPLY_ACTION)
            {
                // execute applied operation
                bool completed = false;
                completed = ExecuteOperation(op.appliedOperation);

                if (completed)
                {
                    // apply editor state before reverted action
                    ApplyEditorState(((Action)op.e).stateAfter);

                    // find next event
                    op.e = op.e.next;

                    if (op.e == null || op.e is Milestone)
                    {
                        // continue with cleanup
                        currentEvent = op.e;
                        op.state = RedoOperation.State.CLEANUP;
                    }
                    else
                    {
                        // further traverse history
                        op.state = RedoOperation.State.TRAVERSE_HISTORY;
                    }
                }
            }

            if (op.state == RedoOperation.State.CLEANUP)
            {
                // follow caret
                FollowCaret();

                // reset formatting pointer
                if (op.e is Insert)
                    textFormatLinePointer = Mathf.Min(textFormatLinePointer, ((Insert)op.e).startTextPosition.lineIndex);
                else if (op.e is Delete)
                    textFormatLinePointer = Mathf.Min(textFormatLinePointer, ((Delete)op.e).selection.start.lineIndex);

                // operation complete
                return true;
            }

            // operation not complete
            return false;
        }

        // executes find operation
        bool ExecuteFindOperation(FindOperation op)
        {
            // abort if search string is null or empty
            if (string.IsNullOrEmpty(op.searchString))
                return true;

            // create searchable string starting at caret position or selection start or end
            TextPosition searchStart = selection != null ? (op.forward ? selection.end : selection.start) : caretTextPosition;
            string searchableString = GetSelectedText(new Selection(searchStart, new TextPosition(lines.Count - 1, lines[lines.Count - 1].Text.Length))) + '\n' + GetSelectedText(new Selection(new TextPosition(0, 0), searchStart));

            // find first or last occurrence of search string
            int resultIndex = op.forward ? searchableString.IndexOf(op.searchString, System.StringComparison.Ordinal) : searchableString.LastIndexOf(op.searchString, System.StringComparison.Ordinal);
            if (resultIndex >= 0)
            {
                // find line and column index of result start
                int resultStartLineIndex = searchStart.lineIndex;
                int resultStartColIndex = searchStart.colIndex + resultIndex;
                while (resultStartColIndex >= lines[resultStartLineIndex % lines.Count].Text.Length)
                {
                    resultStartColIndex -= (lines[resultStartLineIndex % lines.Count].Text.Length + 1);
                    resultStartLineIndex++;
                }

                // find line and column index of result end
                int resultEndLineIndex = resultStartLineIndex;
                int resultEndColIndex = resultStartColIndex + op.searchString.Length;
                while (resultEndColIndex > lines[resultEndLineIndex % lines.Count].Text.Length)
                {
                    resultEndColIndex -= (lines[resultEndLineIndex % lines.Count].Text.Length + 1);
                    resultEndLineIndex++;
                }

                // select result
                TextPosition resultStart = new TextPosition(resultStartLineIndex % lines.Count, resultStartColIndex);
                TextPosition resultEnd = new TextPosition(resultEndLineIndex % lines.Count, resultEndColIndex);
                SetSelection(new Selection(resultStart, resultEnd), true);
            }

            // operation complete
            return true;
        }
    }
}