using UnityEngine;

/// <summary>
/// Example to demonstrate the annotation functionality of the text editor.
/// </summary>
public class LabelHandler : MonoBehaviour
{
    /// <summary>
    /// The text editor.
    /// </summary>
    public InGameTextEditor.TextEditor textEditor = null;

    /// <summary>
    /// Sprite for underline.
    /// </summary>
    public Sprite underlineSprite;

    /// <summary>
    /// Sprite for label icon.
    /// </summary>
    public Sprite labelIconSprite;

    /// <summary>
    /// Labels all occurrences of 'label' in the text.
    /// </summary>
    public void AddLabels()
    {
        // remove old labels
        textEditor.RemoveLabels();

        foreach (InGameTextEditor.Line line in textEditor.Lines)
        {
            int endIndex = 0;
            while (true)
            {
                // find next start index of string 'label'
                int startIndex = line.Text.IndexOf("label", endIndex);
                if (startIndex >= 0)
                {
                    // add label with underline and icon
                    endIndex = startIndex + "label".Length;
                    line.AddLabel(startIndex, endIndex, underlineSprite, new Color(0f, 0.8f, 0f, 0.8f), labelIconSprite, new Color(0f, 0.8f, 0f, 1f), "This is a label", InGameTextEditor.Line.Label.DeleteCondition.THIS_LINE_CHANGES);
                }
                else
                    break;
            }
        }
    }

    /// <summary>
    /// Removes all labels.
    /// </summary>
    public void ClearLabels()
    {
        textEditor.RemoveLabels();
    }
}