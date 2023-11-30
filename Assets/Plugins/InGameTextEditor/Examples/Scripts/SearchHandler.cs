using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the search functionality of the text editor.
/// </summary>
public class SearchHandler : MonoBehaviour
{
    /// <summary>
    /// The text editor.
    /// </summary>
    public InGameTextEditor.TextEditor textEditor = null;

    /// <summary>
    /// The search field.
    /// </summary>
    public InputField inputField = null;

    // indicates if the search field is active
    bool searchFieldActive = false;


    /// <summary>
    /// Finds the next occurrence after the caret position.
    /// </summary>
    public void FindNext()
    {
        textEditor.Find(inputField.text, true);
        ActivateSearchField();
    }

    /// <summary>
    /// Finds the next occurrence before the caret position.
    /// </summary>
    public void FindPrevious()
    {
        textEditor.Find(inputField.text, false);
        ActivateSearchField();
    }

    /// <summary>
    /// Activates the search field and deactivates the editor.
    /// This method is called by the event trigger attached to the search field.
    /// </summary>
    public void ActivateSearchField()
    {
        inputField.ActivateInputField();
        searchFieldActive = true;
        textEditor.EditorActive = false;
    }

    /// <summary>
    /// Deactivates the search field.
    /// This method is called by the event trigger attached to the search field.
    /// </summary>
    public void DeactivateSearchField()
    {
        searchFieldActive = false;
        inputField.DeactivateInputField();
    }

    // update
    void Update()
    {
        if (searchFieldActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // reverse search direction with shift key
                bool reverseSearchDirection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                // find
                if (reverseSearchDirection)
                    FindPrevious();
                else
                    FindNext();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // deactivate search field and activate editor instead
                DeactivateSearchField();
                textEditor.EditorActive = true;
            }
        }

        // check for modifier keys (Ctrl on Windows and Linux, Cmd on macOS)
        bool ctrlOrCmdPressed = false;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
            ctrlOrCmdPressed = true;

        if (ctrlOrCmdPressed && Input.GetKeyDown(KeyCode.F))
            ActivateSearchField();
    }
}