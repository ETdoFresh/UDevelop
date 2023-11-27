using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.RTEditor.ViewModels
{
    [Binding]
    internal class ToolCmdItemViewModel
    {

        [Binding]
        public string Text
        {
            get;
            set;
        }

        [Binding]
        public bool IsValid
        {
            get;
        }

        private ToolCmdItemViewModel() { Debug.Assert(false); }
    }
}
