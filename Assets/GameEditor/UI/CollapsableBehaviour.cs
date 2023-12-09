using UnityEngine;

namespace GameEditor.UI
{
    public class CollapsableBehaviour : MonoBehaviour
    {
        [SerializeField] private bool isCollapsed;
        [SerializeField] private float collapseSpeed = 1;
        [SerializeField] private float initialHeight;
        private RectTransform _rectTransform;

        private void Awake()
        {
            initialHeight = GetComponent<RectTransform>().rect.height;
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (isCollapsed && _rectTransform.rect.height > 0)
            {
                var rectTransform = GetComponent<RectTransform>();
                var height = rectTransform.rect.height;
                height = Mathf.Lerp(height, 0, Time.deltaTime * collapseSpeed);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
            else if (!isCollapsed && _rectTransform.rect.height < initialHeight)
            {
                var rectTransform = GetComponent<RectTransform>();
                var height = rectTransform.rect.height;
                height = Mathf.Lerp(height, initialHeight, Time.deltaTime * collapseSpeed);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    
        public void Collapse()
        {
            isCollapsed = true;
        }
    
        public void Expand()
        {
            isCollapsed = false;
        }
    }
}
