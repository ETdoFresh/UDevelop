using UnityEngine;

namespace GameEditor.UI
{
    public class CollapsableItemBehaviour : MonoBehaviour
    {
        [SerializeField] private bool isCollapsed;
        [SerializeField] private float collapseSpeed = 1;
        [SerializeField] private float initialHeight;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (initialHeight <= 0) initialHeight = _rectTransform.rect.height;
        }

        private void Update()
        {
            if (isCollapsed && _rectTransform.rect.height > 0)
            {
                var rectTransform = GetComponent<RectTransform>();
                var height = rectTransform.rect.height;
                height = Mathf.MoveTowards(height, 0, Time.deltaTime * collapseSpeed);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
            else if (!isCollapsed && _rectTransform.rect.height < initialHeight)
            {
                var rectTransform = GetComponent<RectTransform>();
                var height = rectTransform.rect.height;
                height = Mathf.MoveTowards(height, initialHeight, Time.deltaTime * collapseSpeed);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    
        public void Collapse(float speed)
        {
            collapseSpeed = speed <= 0 ? collapseSpeed : speed;
            isCollapsed = true;
        }
    
        public void Expand(float speed)
        {
            collapseSpeed = speed <= 0 ? collapseSpeed : speed;
            isCollapsed = false;
        }
    }
}
