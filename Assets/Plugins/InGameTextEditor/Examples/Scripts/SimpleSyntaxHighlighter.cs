using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InGameTextEditor.Format
{
    /// <summary>
    /// Simple text formatter using regular expressions to highlighting the
    /// words 'red', 'green' and 'blue', as well as numbers.
    /// </summary>
    public class SimpleSyntaxHighlighter : TextFormatter
    {
        /// <summary>
        /// Red text style.
        /// </summary>
        public TextStyle textStyleRed = new TextStyle(new Color(0.8f, 0f, 0f));

        /// <summary>
        /// Green text style.
        /// </summary>
        public TextStyle textStyleGreen = new TextStyle(new Color(0f, 0.8f, 0f));

        /// <summary>
        /// Blue text style.
        /// </summary>
        public TextStyle textStyleBlue = new TextStyle(new Color(0f, 0f, 0.8f));

        /// <summary>
        /// Blue text style.
        /// </summary>
        public TextStyle textStyleNumber = new TextStyle(FontStyle.Bold);

        // indicates if the text formatter has been initialized
        bool initialized = false;

        // regex to match the text
        Regex regex = null;

        /// <summary>
        /// Indicates whether this text formatter has been initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public override bool Initialized
        {
            get { return initialized; }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Init()
        {
            // create regex pattern
            string regexPattern = "";

            // matches the word 'Red' or 'red'
            regexPattern += @"(?<red>\b[Rr]ed)";

            // matches the word 'Green' or 'green'
            regexPattern += @"|(?<green>\b[Gg]reen)";

            // matches the word 'Blue' or 'blue'
            regexPattern += @"|(?<blue>\b[Bb]lue)";

            // matches a number
            regexPattern += @"|(?<number>\b[0-9]+(\.[0-9]+)?)";

            // create regex
            regex = new Regex(regexPattern);

            // initialization complete
            initialized = true;
        }

        /// <summary>
        /// Is called by the text editor when the given line has changed.
        /// </summary>
        /// <param name="line">Line that has changed.</param>
        public override void OnLineChanged(Line line)
        {
            // collection of text format groups
            List<TextFormatGroup> textFormatGroups = new List<TextFormatGroup>();

            if (line.Text.Length > 0)
            {
                // find regex matches
                MatchCollection matches = regex.Matches(line.Text);

                // loop through all matches
                foreach (Match match in matches)
                {
                    int i = 0;
                    foreach (Group group in match.Groups)
                    {
                        if (group.Success && i > 0)
                        {
                            string groupName = regex.GroupNameFromNumber(i);
                            int tokenStartIndex = group.Index;
                            int tokenEndIndex = tokenStartIndex + group.Value.Length - 1;

                            // add corresponding text format group
                            switch (groupName)
                            {
                                case "red":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleRed));
                                    break;
                                case "green":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleGreen));
                                    break;
                                case "blue":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleBlue));
                                    break;
                                case "number":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleNumber));
                                    break;
                            }
                        }

                        i++;
                    }
                }
            }

            // apply format to line
            line.ApplyTextFormat(textFormatGroups);
        }
    }
}