using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CommandSystem.Editor
{
    /// <summary>
    /// A window that displays a command line like terminal for the editor.
    /// </summary>
    public class EditorCommandLineTerminalWindow2 : EditorWindow
    {
        private int _selectedCommandIndex = -1;
        private string _commandLineInput = "";
        private bool _isCommandLineWindowFocused = true;
        private string _tempCurrentCommand = "";
        private TextEditor _textEditor;
        private bool _hasAutomaticallyFocusedInitially = false;
        private Vector2 _scrollPosition = Vector2.zero;
        private Texture2D _backgroundTexture;

        // Ctrl + alt + space to open the window.
        [MenuItem("Window/Unity Command Line Terminal 2 %&SPACE")]
        public static void ShowWindow()
        {
            GetWindow<EditorCommandLineTerminalWindow2>("Command Line Terminal");
        }

        private void OnGUI()
        {
            _textEditor ??=
                typeof(EditorGUI).GetField("activeEditor", BindingFlags.Static | BindingFlags.NonPublic)
                    ?.GetValue(null) as TextEditor;

            var windowRect = new Rect(0, 0, position.width, position.height);
            
            // Slightly darker background
            var darkerBackgroundStyle = new GUIStyle(GUI.skin.box);
            if (_backgroundTexture == null) _backgroundTexture = MakeTex(1, 1, new Color(0f, 0f, 0f, 0.5f));
            darkerBackgroundStyle.normal.background = _backgroundTexture;
            darkerBackgroundStyle.normal.textColor = Color.white;
            GUI.Box(windowRect, GUIContent.none, darkerBackgroundStyle);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.BeginVertical();
            
            // ShowDebugInformation();

            // Draw the growing command history.
            var commandOutput = CommandLineHeader.GetHeader();
            commandOutput += EditorCommandProcessor.GetCommandOutput();
            var commandOutputHeight = EditorStyles.label.CalcHeight(new GUIContent(commandOutput), windowRect.width);
            EditorGUILayout.LabelField(commandOutput, GUILayout.Height(commandOutputHeight));

            // Draw the command line input. and name it "CommandLineInput" so we can focus it.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(">", GUILayout.Width(10));
            GUI.SetNextControlName("CommandLineInput");
            
            // _commandLineInput = EditorGUILayout.TextField(_commandLineInput);
            // Allow the command line input to be multiline. Left Align the text.
            var commandInputHeight = EditorStyles.textField.CalcHeight(new GUIContent(_commandLineInput), windowRect.width);
            // left align the text area style
            var textAreaStyle = new GUIStyle(EditorStyles.textField)
            {
                alignment = TextAnchor.MiddleLeft,
                normal = {background = _backgroundTexture},
                fixedHeight = commandInputHeight + 5,
            };
            _commandLineInput = EditorGUILayout.TextArea(_commandLineInput, textAreaStyle, GUILayout.Width(windowRect.width - 40));
            EditorGUILayout.EndHorizontal();

            // Draw the command line input auto complete.
            var autoCompleteCommand = EditorCommandProcessor.AutoCompleteCommand(_commandLineInput);
            autoCompleteCommand = string.IsNullOrEmpty(autoCompleteCommand)
                ? ""
                : autoCompleteCommand + " {Tab to autocomplete}";
            EditorGUILayout.LabelField(autoCompleteCommand);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            // Handle keyboard events.
            var isKeyUp = Event.current.type == EventType.KeyUp;
            if (isKeyUp && _isCommandLineWindowFocused)
            {
                var keyCode = Event.current.keyCode;
                var isShift = Event.current.shift;
                if (keyCode == KeyCode.Return)
                {
                    // Ignore Return Key Down
                    if (_commandLineInput.EndsWith("\n"))
                        _commandLineInput = _commandLineInput.Remove(_commandLineInput.Length - 1);
                    
                    if (isShift && _textEditor != null)
                    {
                        _textEditor.Insert('\n');
                        _commandLineInput = _textEditor.text;
                        EditorGUI.FocusTextInControl("CommandLineInput");
                        EditorApplication.delayCall += OnDelayCallSelectCommandLineInput;
                    }
                    else
                    {
                        if (_commandLineInput.ToLower() == "close" || _commandLineInput.ToLower() == "exit")
                        {
                            Close();
                            return;
                        }

                        EditorCommandProcessor.ExecuteCommand(_commandLineInput);
                        _commandLineInput = "";
                        _selectedCommandIndex = -1;
                        if (_textEditor != null)
                        {
                            _textEditor.text = _commandLineInput;
                            _textEditor.MoveTextEnd();
                        }
                    }
                }
                else if (keyCode == KeyCode.UpArrow)
                {
                    if (_selectedCommandIndex == -1) _tempCurrentCommand = _commandLineInput;
                    _selectedCommandIndex = EditorCommandProcessor.SelectPreviousCommand(_selectedCommandIndex);
                    _commandLineInput = EditorCommandProcessor.GetCommandHistory(_selectedCommandIndex);
                    if (_selectedCommandIndex == -1) _commandLineInput = _tempCurrentCommand;
                    if (_textEditor != null)
                    {
                        _textEditor.text = _commandLineInput;
                        _textEditor.MoveTextEnd();
                    }
                }
                else if (keyCode == KeyCode.DownArrow)
                {
                    if (_selectedCommandIndex == -1) _tempCurrentCommand = _commandLineInput;
                    _selectedCommandIndex = EditorCommandProcessor.SelectNextCommand(_selectedCommandIndex);
                    _commandLineInput = EditorCommandProcessor.GetCommandHistory(_selectedCommandIndex);
                    if (_selectedCommandIndex == -1) _commandLineInput = _tempCurrentCommand;
                    if (_textEditor != null)
                    {
                        _textEditor.text = _commandLineInput;
                        _textEditor.MoveTextEnd();
                    }
                }
                else if (keyCode == KeyCode.Tab)
                {
                    _commandLineInput = EditorCommandProcessor.AutoCompleteCommand(_commandLineInput);
                    EditorGUI.FocusTextInControl("CommandLineInput");
                    EditorApplication.delayCall += OnDelayCallSelectCommandLineInput;
                }
                else if (keyCode == KeyCode.Escape)
                {
                    _commandLineInput = "";
                    _selectedCommandIndex = -1;
                    _tempCurrentCommand = "";
                    EditorGUI.FocusTextInControl("CommandLineInput");
                    EditorApplication.delayCall += OnDelayCallSelectCommandLineInput;
                    
                }
                
                Repaint();
                _scrollPosition.y = Mathf.Infinity;
            }

            // Handle mouse events.
            var isMouseDown = Event.current.type == EventType.MouseDown;
            if (isMouseDown)
            {
                var isInsideWindow = windowRect.Contains(Event.current.mousePosition);
                _isCommandLineWindowFocused = isInsideWindow;
                if (isInsideWindow) EditorGUI.FocusTextInControl("CommandLineInput");
            }

            // Automatically focus the command line input when the window is opened.
            if (!_hasAutomaticallyFocusedInitially)
            {
                _hasAutomaticallyFocusedInitially = true;
                _isCommandLineWindowFocused = true;
                EditorGUI.FocusTextInControl("CommandLineInput");
            }
        }

        private void ShowDebugInformation()
        {
            EditorGUILayout.LabelField("Selected Command Index: " + _selectedCommandIndex);
            EditorGUILayout.LabelField("Command Line Input: " + _commandLineInput);
            EditorGUILayout.LabelField("Is Command Line Window Focused: " + _isCommandLineWindowFocused);
            EditorGUILayout.LabelField("Temp Current Command: " + _tempCurrentCommand);
            EditorGUILayout.LabelField("Hot Control: " + GUIUtility.hotControl);
            EditorGUILayout.LabelField("Keyboard Control: " + GUIUtility.keyboardControl);
            EditorGUILayout.LabelField("Editing Text Field: " + EditorGUIUtility.editingTextField);
            EditorGUILayout.LabelField("Text Editor: " + _textEditor?.text);
        }

        private void OnDelayCallSelectCommandLineInput()
        {
            EditorApplication.delayCall -= OnDelayCallSelectCommandLineInput;
            if (_textEditor == null) return;
            _textEditor.text = _commandLineInput;
            _textEditor.MoveTextEnd();
            Repaint();
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}