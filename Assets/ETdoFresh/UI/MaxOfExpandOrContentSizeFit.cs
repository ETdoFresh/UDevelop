using UnityEngine;
using UnityEngine.UI;

namespace ETdoFresh.UI
{
    [ExecuteInEditMode]
    public class MaxOfExpandOrContentSizeFit : MonoBehaviour
    {
        [SerializeField] private LayoutGroup layoutGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform parentRectTransform;
        [SerializeField] private bool executeInEditMode;

        private void OnValidate()
        {
            if (!rectTransform) rectTransform = GetComponent<RectTransform>();
            if (!parentRectTransform) parentRectTransform = transform.parent.GetComponent<RectTransform>();
            if (!layoutGroup) layoutGroup = GetComponent<LayoutGroup>();
        }
        
        private void Update()
        {
            if (!executeInEditMode && !Application.isPlaying) return;
            var layoutGroupPreferredWidth = layoutGroup.preferredWidth;
            var parentRectWidth = parentRectTransform.rect.width;
            var sizeDelta = rectTransform.sizeDelta;
            
            if (layoutGroupPreferredWidth > parentRectWidth)
            {
                sizeDelta.x = layoutGroupPreferredWidth - parentRectWidth;
            }
            else
            {
                sizeDelta.x = 0;
            }
            
            var layoutGroupPreferredHeight = layoutGroup.preferredHeight;
            var parentRectHeight = parentRectTransform.rect.height;
            if (layoutGroupPreferredHeight > parentRectHeight)
            {
                sizeDelta.y = layoutGroupPreferredHeight - parentRectHeight;
            }
            else
            {
                sizeDelta.y = 0;
            }
            
            rectTransform.sizeDelta = sizeDelta;
        }
    }
}