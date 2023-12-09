using ETdoFresh.UnityPackages.ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.UI
{
    public class CollapsableSectionBehaviour : MonoBehaviour
    {
        [SerializeField] private Button collapseButton;
        [SerializeField] private float collapseSpeed = 1;
        [SerializeField] private bool isCollapsed;
        private bool _wasCollapsed;

        private void OnEnable()
        {
            collapseButton.onClick.AddPersistentListener(OnCollapseButtonClick);
        }

        private void OnDisable()
        {
            collapseButton.onClick.RemovePersistentListener(OnCollapseButtonClick);
        }

        private void Update()
        {
            if (_wasCollapsed == isCollapsed) return;
            _wasCollapsed = isCollapsed;
            CollapseOrExpand();
        }

        private void OnCollapseButtonClick()
        {
            isCollapsed = !isCollapsed;
        }

        private void CollapseOrExpand()
        {
            foreach (var collapsableItem in GetComponentsInChildren<CollapsableItemBehaviour>(true))
                if (isCollapsed) collapsableItem.Collapse(collapseSpeed);
                else collapsableItem.Expand(collapseSpeed);
        }
    }
}