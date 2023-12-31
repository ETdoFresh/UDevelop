﻿using Battlehub.RTCommon;
using Battlehub.UIControls;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityObject = UnityEngine.Object;

namespace Battlehub.RTEditor.UI
{
    public struct Space
    {
    }

    public static class MonoBehaviorExtension
    {
        public static RectTransform RectTransform(this MonoBehaviour monoBehaviour)
        {
            return (RectTransform)monoBehaviour.transform;
        }
    }

    public partial class AutoUI
    {
        public (TextMeshProUGUI Control, LayoutElement LayoutElement) Label(bool createLayoutElement = true, bool wrap = false, StyleAttribute style = null)
        {
            var element = Control(m_controlsMap.GetControl<TextMeshProUGUI>(), createLayoutElement, wrap);

            if(style != null)
            {
                element.Item1.alignment = style.TextAlignment;
            }

            return element;
        }

        public (Button Control, LayoutElement LayoutElement) Button(bool createLayoutElement = true, bool wrap = false, StyleAttribute style = null)
        {
            return Control(m_controlsMap.GetControl<Button>(), createLayoutElement, wrap);
        }

        public (TMP_Dropdown Control, LayoutElement LayoutElement) Dropdown(bool createLayoutElement = true, bool wrap = false, StyleAttribute style = null)
        {
            return Control(m_controlsMap.GetControl<TMP_Dropdown>(), createLayoutElement, wrap);
        }

        public (Toggle Control, LayoutElement LayoutElement) Checkbox(bool createLayoutElement = true, bool wrap = false, StyleAttribute style = null)
        {
            return Control(m_controlsMap.GetControl<Toggle>(), createLayoutElement, wrap);
        }

        public (VirtualizingTreeView Control, VirtualizingTreeViewItem Item, LayoutElement LayoutElement) TreeView(bool createLayoutElement = true, StyleAttribute style = null)
        {
            VirtualizingTreeView vtv = m_controlsMap.GetControl<VirtualizingTreeView>();
            VirtualizingTreeViewItem vtvItem = m_controlsMap.GetControl<VirtualizingTreeViewItem>();

            bool isTreeViewPrefabActive = vtv.gameObject.activeSelf;
            vtv.gameObject.SetActive(false);
            var (treeView, layoutElement) = Control(vtv, createLayoutElement);

            bool isTreeViewItemPrefabActive = vtvItem.gameObject.activeSelf;
            vtvItem.gameObject.SetActive(false);
            
            var treeViewItem = UnityObject.Instantiate(vtvItem);
            foreach(Transform child in treeViewItem.ItemPresenter.transform)
            {
                UnityObject.Destroy(child.gameObject);
            }

            HorizontalOrVerticalLayoutGroup layoutGroup = treeViewItem.ItemPresenter.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if(layoutGroup != null)
            {
                layoutGroup.childForceExpandWidth = true;
                layoutGroup.childForceExpandHeight = true;
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = true;
            }

            var scrollRect = treeView.GetComponent<VirtualizingScrollRect>();
            scrollRect.ContainerPrefab = treeViewItem.GetComponent<RectTransform>();
            treeViewItem.transform.SetParent(treeView.transform, false);

            vtvItem.gameObject.SetActive(isTreeViewItemPrefabActive);
            vtv.gameObject.SetActive(isTreeViewPrefabActive);

            return new ValueTuple<VirtualizingTreeView, VirtualizingTreeViewItem, LayoutElement>(treeView, treeViewItem, layoutElement);
        }

        public (RectTransform Control, LayoutElement LayoutElement) Space(bool createLayoutElement = true, StyleAttribute style = null)
        {
            GameObject imageGo = new GameObject();
            imageGo.name = "Space";

            RectTransform imageRT = imageGo.AddComponent<RectTransform>();
            
            if(ColorUtility.TryParseHtmlString(style.Color, out Color color))
            {
                Image image = imageGo.AddComponent<Image>();
                image.raycastTarget = false;
                image.color = color;
            }

            return AttachToPanel(imageRT, createLayoutElement, false);
        }

        public (Image Control, LayoutElement LayoutElement) Image(bool createLayoutElement = true, bool wrap = false, StyleAttribute style = null)
        {
            Image image = new GameObject().AddComponent<Image>();
            image.name = "Image";
            
            return AttachToPanel(image, createLayoutElement,wrap);
        }

        public (T Control, LayoutElement LayoutElement) Control<T>(bool createLayoutElement = false, bool wrap = false) where T : Component
        {
            var control = m_controlsMap.GetControl<T>();
            return Control(control, createLayoutElement, wrap);
        }
        
        public (T Control, LayoutElement LayoutElement) Control<T>(T prefab, bool createLayoutElement = false, bool wrap = false) where T : Component
        {
            T element = UnityObject.Instantiate(prefab);
            element.name = prefab.name;

            return AttachToPanel(element, createLayoutElement, wrap);
        }

        private (T Control, LayoutElement LayoutElement) AttachToPanel<T>(T element, bool createLayoutElement, bool wrap) where T : Component
        {
            RectTransform panel = m_panelStack.Peek();
            RectTransform rt;
            if (wrap)
            {
                GameObject wrapper = new GameObject();
                wrapper.name = $"{element.name} Layout";
                wrapper.transform.SetParent(panel, false);
                wrapper.AddComponent<RectTransform>();

                element.transform.SetParent(wrapper.transform);
                rt = (RectTransform)element.transform;
                rt.Stretch();

                return new ValueTuple<T, LayoutElement>(element, CreateLayoutElement(wrapper.transform, createLayoutElement));
            }

            element.transform.SetParent(panel);
            rt = (RectTransform)element.transform;
            rt.Stretch();

            return new ValueTuple<T, LayoutElement>(element, CreateLayoutElement(element, createLayoutElement));
        }

        private static LayoutElement CreateLayoutElement(Component element, bool createLayoutElement)
        {
            LayoutElement layoutElement = element.GetComponent<LayoutElement>();
            if (layoutElement == null && createLayoutElement)
            {
                layoutElement = element.gameObject.AddComponent<LayoutElement>();
            }

            return layoutElement;
        }
    }

}
