using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETdoFresh.UI
{
    public class CustomContentSizeFitter : MonoBehaviour
    {
        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize,
            AtLeastExpandedSizeOrPreferredSize
        }
        
        [SerializeField] private FitMode horizontalFit = FitMode.Unconstrained;
        [SerializeField] private FitMode verticalFit = FitMode.Unconstrained;
        
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            var size = _rectTransform.sizeDelta;
            switch (horizontalFit)
            {
                case FitMode.Unconstrained:
                    break;
                case FitMode.MinSize:
                    size.x = GetMinSize().x;
                    break;
                case FitMode.PreferredSize:
                    size.x = GetPreferredSize().x;
                    break;
                case FitMode.AtLeastExpandedSizeOrPreferredSize:
                    size.x = GetExpandedSizeOrPreferredSize().x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (verticalFit)
            {
                case FitMode.Unconstrained:
                    break;
                case FitMode.MinSize:
                    size.y = GetMinSize().y;
                    break;
                case FitMode.PreferredSize:
                    size.y = GetPreferredSize().y;
                    break;
                case FitMode.AtLeastExpandedSizeOrPreferredSize:
                    size.y = GetExpandedSizeOrPreferredSize().y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // Resize rectTransform to fit size
            // _rectTransform.sizeDelta = size; // not correct because it is set to stretch
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        }
        
        private Vector2 GetMinSize()
        {
            var minSize = Vector2.zero;
            foreach (RectTransform child in _rectTransform)
            {
                var childLayoutElement = child.GetComponent<LayoutElement>();
                if (childLayoutElement)
                {
                    minSize.x += childLayoutElement.minWidth;
                    minSize.y += childLayoutElement.minHeight;
                }
                else
                {
                    minSize.x += child.sizeDelta.x;
                    minSize.y += child.sizeDelta.y;
                }
            }
            return minSize;
        }
        
        private Vector2 GetPreferredSize()
        {
            var preferredSize = Vector2.zero;
            foreach (RectTransform child in _rectTransform)
            {
                var childLayoutElement = child.GetComponent<LayoutElement>();
                if (childLayoutElement)
                {
                    preferredSize.x += childLayoutElement.preferredWidth;
                    preferredSize.y += childLayoutElement.preferredHeight;
                }
                else
                {
                    preferredSize.x += child.sizeDelta.x;
                    preferredSize.y += child.sizeDelta.y;
                }
            }
            return preferredSize;
        }
        
        private Vector2 GetExpandedSizeOrPreferredSize()
        {
            var expandedSizeOrPreferredSize = Vector2.zero;
            var preferredSize = GetPreferredSize();
            var parentSize = _rectTransform.parent.GetComponent<RectTransform>().rect.size;
            expandedSizeOrPreferredSize.x = Mathf.Max(preferredSize.x, parentSize.x);
            expandedSizeOrPreferredSize.y = Mathf.Max(preferredSize.y, parentSize.y);
            return expandedSizeOrPreferredSize;
        }
    }
}