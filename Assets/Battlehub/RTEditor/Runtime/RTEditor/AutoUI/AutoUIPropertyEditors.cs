using Battlehub.RTCommon;
using Battlehub.UIControls;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using UnityObject = UnityEngine.Object;

namespace Battlehub.RTEditor.UI
{
    public static class PropertyEditorExtension
    {
        public static void SetRange(this PropertyEditor propertyEditor, Range range)
        {
            RangeEditor rangeEditor = propertyEditor as RangeEditor;
            if (rangeEditor != null)
            {
                rangeEditor.Min = range.Min;
                rangeEditor.Max = range.Max;
            }
        }

        public static void SetRange(this PropertyEditor propertyEditor, RangeInt range)
        {
            RangeIntEditor rangeEditor = propertyEditor as RangeIntEditor;
            if (rangeEditor != null)
            {
                rangeEditor.Min = (int)range.Min;
                rangeEditor.Max = (int)range.Max;
            }
        }
    }

    public partial class AutoUI
    {
        public (PropertyEditor Control, LayoutElement LayoutElement) PropertyEditor<T>(bool createLayoutElement = false, bool wrap = false)
        {
            IEditorsMap editors = IOC.Resolve<IEditorsMap>();
            GameObject propertyEditorGo = editors.GetPropertyEditor(typeof(T));
            PropertyEditor propertyEditor = UnityObject.Instantiate(propertyEditorGo.GetComponent<PropertyEditor>());
            propertyEditor.name = propertyEditorGo.name;

            LayoutElement layoutElement = propertyEditor.GetComponent<LayoutElement>();
            if (createLayoutElement)
            {
                layoutElement.flexibleHeight = 0;
            }
            else
            {
                UnityObject.Destroy(layoutElement);
            }

            return AttachToPanel(propertyEditor, createLayoutElement, wrap);
        }
        
        public (RectTransform Control, LayoutElement LayoutElement) PropertyEditor(PropertyInfo propertyInfo,  bool createLayoutElement = true, bool wrap = true)
        {
            RectTransform panel = m_panelStack.Peek();

            GameObject editor = m_editorsMap.GetPropertyEditor(propertyInfo.PropertyType);
            if (editor == null)
            {
                editor = new GameObject("NoEditor");
            }
            else
            {
                string name = editor.name;
                editor = UnityObject.Instantiate(editor);
                editor.name = name;
            }

            if (wrap)
            {
                GameObject wrapper = new GameObject();
                wrapper.name = $"{editor.name} Layout";
                wrapper.transform.SetParent(panel, false);
                wrapper.AddComponent<RectTransform>();

                editor.transform.SetParent(wrapper.transform);
                RectTransform rt = (RectTransform)editor.transform;
                rt.Stretch();

                return new ValueTuple<RectTransform, LayoutElement>((RectTransform)editor.transform, CreateLayoutElement(wrapper, createLayoutElement));
            }

            return new ValueTuple<RectTransform, LayoutElement>((RectTransform)editor.transform, CreateLayoutElement(editor, createLayoutElement));
        }

        private static LayoutElement CreateLayoutElement(GameObject editor, bool createLayoutElement)
        {
            LayoutElement layoutElement = editor.GetComponent<LayoutElement>();
            if (layoutElement == null && createLayoutElement)
            {
                layoutElement = editor.gameObject.AddComponent<LayoutElement>();
            }

            return layoutElement;
        }
    }
}

