using UnityEditor;
using UnityEngine;

namespace CommandSystem.Editor
{
    /// <summary>
    /// A window that displays a command line like terminal for the editor.
    /// </summary>
    public class EditorCommandLineTerminalWindow : EditorWindow
    {
        private int _selectedCommandIndex = -1;
        private string _commandLineInput = "";
        private bool _isCommandLineInputFocused = true;
        private string _tempCurrentCommand = "";

        // Ctrl + space to open the window.
        [MenuItem("Window/Unity Command Line Terminal %SPACE")]
        public static void ShowWindow()
        {
            GetWindow<EditorCommandLineTerminalWindow>("Command Line Terminal");
        }

        private void OnGUI()
        {
            // Create a command line terminal where user types in command at bottom of window.
            // When user presses enter, the command is executed and the output is displayed in the window.
            // The user can also use the submit button to execute the command.
            // Alternatively, the user can press the up and down arrow keys to cycle through the command history.
            // The user can also press the tab key to auto complete the command.
            // The user can also press the escape key to clear the command line.
            // The entered command scrolls up as new commands are entered.
            // The user can also scroll up and down the window to view previous commands.
            // The user can also select and copy text from the window.
            // The user can also drag the window around.
            // The user can also resize the window.
            // The user can also close the window.

            var windowRect = new Rect(0, 0, position.width, position.height);

            // Draw the window background.
            GUI.Box(windowRect, "");

            // Draw the text area where the command line input is displayed. Fill entire window.
            var textAreaRect = new Rect(0, 0, windowRect.width, windowRect.height);
            var textAreaText = EditorCommandProcessor.GetCommandOutput() + "\n> " + _commandLineInput;
            textAreaText += _isCommandLineInputFocused ? "|" : "";
            GUI.TextArea(textAreaRect, textAreaText);
            GUI.FocusControl("");

            // Listen for mouse events.
            if (Event.current.type == EventType.MouseDown)
            {
                _isCommandLineInputFocused = textAreaRect.Contains(Event.current.mousePosition);
            }

            if (_isCommandLineInputFocused)
            {
                if (Event.current.type == EventType.KeyUp)
                {
                    if (Event.current.keyCode == KeyCode.Return)
                    {
                        EditorCommandProcessor.ExecuteCommand(_commandLineInput);
                        _commandLineInput = "";
                        _tempCurrentCommand = "";
                        _selectedCommandIndex = -1;
                    }
                    else if (Event.current.keyCode == KeyCode.UpArrow)
                    {
                        if (_selectedCommandIndex == -1)
                            _tempCurrentCommand = _commandLineInput;
                        _selectedCommandIndex = EditorCommandProcessor.SelectPreviousCommand(_selectedCommandIndex);
                        _commandLineInput = EditorCommandProcessor.GetCommandHistory(_selectedCommandIndex);
                    }
                    else if (Event.current.keyCode == KeyCode.DownArrow)
                    {
                        if (_selectedCommandIndex == -1)
                            _tempCurrentCommand = _commandLineInput;
                        _selectedCommandIndex = EditorCommandProcessor.SelectNextCommand(_selectedCommandIndex);
                        _commandLineInput = EditorCommandProcessor.GetCommandHistory(_selectedCommandIndex);
                        if (_selectedCommandIndex == -1)
                            _commandLineInput = _tempCurrentCommand;
                    }
                    else if (Event.current.keyCode == KeyCode.Tab)
                    {
                        _commandLineInput = EditorCommandProcessor.AutoCompleteCommand(_commandLineInput);
                    }
                    else if (Event.current.keyCode == KeyCode.Escape)
                    {
                        _commandLineInput = "";
                        _selectedCommandIndex = -1;
                    }
                    else if (Event.current.keyCode >= KeyCode.A && Event.current.keyCode <= KeyCode.Z)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += Event.current.keyCode.ToString().ToLower();
                        else
                            _commandLineInput += Event.current.keyCode;
                    }
                    else if (Event.current.keyCode >= KeyCode.Alpha0 && Event.current.keyCode <= KeyCode.Alpha9)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += Event.current.keyCode.ToString().Substring(5);
                        else if (Event.current.keyCode == KeyCode.Alpha1)
                            _commandLineInput += "!";
                        else if (Event.current.keyCode == KeyCode.Alpha2)
                            _commandLineInput += "@";
                        else if (Event.current.keyCode == KeyCode.Alpha3)
                            _commandLineInput += "#";
                        else if (Event.current.keyCode == KeyCode.Alpha4)
                            _commandLineInput += "$";
                        else if (Event.current.keyCode == KeyCode.Alpha5)
                            _commandLineInput += "%";
                        else if (Event.current.keyCode == KeyCode.Alpha6)
                            _commandLineInput += "^";
                        else if (Event.current.keyCode == KeyCode.Alpha7)
                            _commandLineInput += "&";
                        else if (Event.current.keyCode == KeyCode.Alpha8)
                            _commandLineInput += "*";
                        else if (Event.current.keyCode == KeyCode.Alpha9)
                            _commandLineInput += "(";
                        else if (Event.current.keyCode == KeyCode.Alpha0)
                            _commandLineInput += ")";
                    }
                    else if (Event.current.keyCode >= KeyCode.Keypad0 && Event.current.keyCode <= KeyCode.Keypad9)
                    {
                        _commandLineInput += Event.current.keyCode.ToString().Substring(7);
                    }
                    else if (Event.current.keyCode == KeyCode.Space)
                    {
                        if (!Event.current.control)
                            _commandLineInput += " ";
                    }
                    else if (Event.current.keyCode == KeyCode.Backspace)
                    {
                        if (_commandLineInput.Length > 0)
                            _commandLineInput = _commandLineInput.Substring(0, _commandLineInput.Length - 1);
                    }
                    else if (Event.current.keyCode == KeyCode.Comma)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += ",";
                        else
                            _commandLineInput += "<";
                    }
                    else if (Event.current.keyCode == KeyCode.Period)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += ".";
                        else
                            _commandLineInput += ">";
                    }
                    else if (Event.current.keyCode == KeyCode.Slash)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "/";
                        else
                            _commandLineInput += "?";
                    }
                    else if (Event.current.keyCode == KeyCode.Question)
                    {
                        _commandLineInput += "?";
                    }
                    else if (Event.current.keyCode == KeyCode.Backslash)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "\\";
                        else
                            _commandLineInput += "|";
                    }
                    else if (Event.current.keyCode == KeyCode.Exclaim)
                    {
                        _commandLineInput += "!";
                    }
                    else if (Event.current.keyCode == KeyCode.At)
                    {
                        _commandLineInput += "@";
                    }
                    else if (Event.current.keyCode == KeyCode.Hash)
                    {
                        _commandLineInput += "#";
                    }
                    else if (Event.current.keyCode == KeyCode.Dollar)
                    {
                        _commandLineInput += "$";
                    }
                    else if (Event.current.keyCode == KeyCode.Percent)
                    {
                        _commandLineInput += "%";
                    }
                    else if (Event.current.keyCode == KeyCode.Caret)
                    {
                        _commandLineInput += "^";
                    }
                    else if (Event.current.keyCode == KeyCode.Ampersand)
                    {
                        _commandLineInput += "&";
                    }
                    else if (Event.current.keyCode == KeyCode.Asterisk)
                    {
                        _commandLineInput += "*";
                    }
                    else if (Event.current.keyCode == KeyCode.LeftParen)
                    {
                        _commandLineInput += "(";
                    }
                    else if (Event.current.keyCode == KeyCode.RightParen)
                    {
                        _commandLineInput += ")";
                    }
                    else if (Event.current.keyCode == KeyCode.Minus)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "-";
                        else
                            _commandLineInput += "_";
                    }
                    else if (Event.current.keyCode == KeyCode.Underscore)
                    {
                        _commandLineInput += "_";
                    }
                    else if (Event.current.keyCode == KeyCode.Equals)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "=";
                        else
                            _commandLineInput += "+";
                    }
                    else if (Event.current.keyCode == KeyCode.Plus)
                    {
                        _commandLineInput += "+";
                    }
                    else if (Event.current.keyCode == KeyCode.LeftBracket)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "[";
                        else
                            _commandLineInput += "{";
                    }
                    else if (Event.current.keyCode == KeyCode.RightBracket)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "]";
                        else
                            _commandLineInput += "}";
                    }
                    else if (Event.current.keyCode == KeyCode.LeftCurlyBracket)
                    {
                        _commandLineInput += "{";
                    }
                    else if (Event.current.keyCode == KeyCode.RightCurlyBracket)
                    {
                        _commandLineInput += "}";
                    }
                    else if (Event.current.keyCode == KeyCode.Semicolon)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += ";";
                        else
                            _commandLineInput += ":";
                    }
                    else if (Event.current.keyCode == KeyCode.Colon)
                    {
                        _commandLineInput += ":";
                    }
                    else if (Event.current.keyCode == KeyCode.Quote)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "'";
                        else
                            _commandLineInput += "\"";
                    }
                    else if (Event.current.keyCode == KeyCode.Less)
                    {
                        _commandLineInput += "<";
                    }
                    else if (Event.current.keyCode == KeyCode.Greater)
                    {
                        _commandLineInput += ">";
                    }
                    else if (Event.current.keyCode == KeyCode.BackQuote)
                    {
                        if (!Event.current.shift)
                            _commandLineInput += "`";
                        else
                            _commandLineInput += "~";
                    }
                    Repaint();
                }
            }
        }
    }
}