using Battlehub.UIControls;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTEditor.UI
{
    public partial class AutoUI 
    {
        public (VerticalLayoutGroup Control, LayoutElement LayoutElement) BeginVerticalLayout(bool createLayoutElement = false)
        {
            return BeginLayout<VerticalLayoutGroup>(createLayoutElement);
        }

        public (VerticalLayoutGroup Control, LayoutElement LayoutElement) BeginVerticalLayout(bool createLayoutElement, bool childForceExpandHeight)
        {
            var layoutGroup = BeginLayout<VerticalLayoutGroup>(createLayoutElement);

            layoutGroup.Item1.childForceExpandHeight = childForceExpandHeight;

            return layoutGroup;
        }

        public void EndVerticalLayout()
        {
            EndLayout();
        }

        public (HorizontalLayoutGroup Control, LayoutElement LayoutElement) BeginHorizontalLayout(bool createLayoutElement = false)
        {
            return BeginLayout<HorizontalLayoutGroup>(createLayoutElement);
        }

        public (HorizontalLayoutGroup Control, LayoutElement LayoutElement) BeginHorizontalLayout(bool createLayoutElement, bool childForceExpandWidth)
        {
            var layoutGroup = BeginLayout<HorizontalLayoutGroup>(createLayoutElement);
            
            layoutGroup.Item1.childForceExpandWidth = childForceExpandWidth;

            return layoutGroup;
        }

        public void EndHorizontalLayout()
        {
            EndLayout();
        }

        public ValueTuple<T, LayoutElement> BeginLayout<T>(bool createLayoutElement = false) where T : LayoutGroup
        {
            RectTransform panel = m_panelStack.Peek();
            LayoutElement layoutElement = null;
            if (panel.GetComponent<LayoutGroup>())
            {
                Transform childPanel = new GameObject().transform;
                childPanel.name = nameof(T);
                childPanel.transform.SetParent(panel, false);

                panel = childPanel.gameObject.AddComponent<RectTransform>();
                panel.Stretch();

                if(createLayoutElement)
                {
                    layoutElement = panel.gameObject.AddComponent<LayoutElement>();
                }
                
                m_panelStack.Push(panel);
            }

            return new ValueTuple<T, LayoutElement>(panel.gameObject.AddComponent<T>(), layoutElement);
        }

        public void EndLayout()
        {
            if (m_panelStack.Count > 0)
            {
                m_panelStack.Pop();
            }
        }
    }

}
