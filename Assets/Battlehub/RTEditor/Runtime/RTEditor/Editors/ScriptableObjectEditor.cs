using Battlehub.RTCommon;
using System;
using TMPro;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Battlehub.RTEditor
{
    public class ScriptableObjectEditor : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_nameText = null;
        [SerializeField]
        private Transform m_propertiesPanel = null;

        private IRuntimeEditor m_editor;
        private IRuntimeSelection m_selectionOverride;
        private IEditorsMap m_editorsMap;

          private UnityObject[] SelectedObjects
        {
            get
            {
                if (m_selectionOverride == null || m_selectionOverride.activeObject == null)
                {
                    return m_editor.Selection.objects;
                }

                return m_selectionOverride.objects;
            }
        }

        private bool m_initOnStart = false;
        private void Awake()
        {
            m_editor = IOC.Resolve<IRuntimeEditor>();
            m_editorsMap = IOC.Resolve<IEditorsMap>();

            m_initOnStart = SelectedObjects == null;
            if(!m_initOnStart)
            {
                Init();
            }
        }

        private void Start()
        {
            RuntimeWindow window = GetComponentInParent<RuntimeWindow>();
            if(window != null)
            {
                m_selectionOverride = window.IOCContainer.Resolve<IRuntimeSelection>();
            }

            if(m_initOnStart)
            {
                Init();
            }
        }

        private void Init()
        {
            m_nameText.text = GameObjectEditorUtils.GetObjectName(SelectedObjects);
            CreatePropertyEditors(SelectedObjects);
        }

        private void OnDestroy()
        {
            m_editor = null;
            m_editorsMap = null;
            m_selectionOverride = null;
        }

        private void Update()
        {
            UnityObject[] objects = SelectedObjects;
            if (objects[0] == null)
            {
                return;
            }
            string objectName = GameObjectEditorUtils.GetObjectName(objects);
            if (m_nameText.text != objectName)
            {
                m_nameText.text = objectName;
            }
        }

        protected virtual void CreatePropertyEditors(UnityObject[] selectedObjects)
        {
            Type objectType = selectedObjects[0].GetType();
            for (int i = 1; i < selectedObjects.Length; ++i)
            {
                if (objectType != selectedObjects[i].GetType())
                {
                    return;
                }
            }

            PropertyDescriptor[] propertyDescriptors = GetPropertyDescriptors(selectedObjects, objectType);
            for (int i = 0; i < propertyDescriptors.Length; ++i)
            {
                var propertyDescriptor = propertyDescriptors[i];
                BuildPropertyEditor(propertyDescriptor);
            }
        }

        protected virtual PropertyDescriptor[] GetPropertyDescriptors(UnityObject[] selectedObjects, Type objectType)
        {
            return m_editorsMap.GetDefaultPropertyDescriptors(objectType, selectedObjects);
        }

        protected virtual void BuildPropertyEditor(PropertyDescriptor descriptor)
        {
            PropertyEditor editor = m_editorsMap.InstantiatePropertyEditor(descriptor, m_propertiesPanel);
            if (editor == null)
            {
                return;
            }

            editor.Init(
                descriptor.Targets,
                descriptor.Targets,
                descriptor.MemberInfo,
                null, 
                descriptor.Label, 
                null, 
                () => { descriptor.ValueChangedCallback?.Invoke(); }, 
                () => { descriptor.EndEditCallback?.Invoke(); },
                true, 
                descriptor.ChildDesciptors, 
                null, null, null, null, () => { });
        }

    }
}

