using UnityEngine;

namespace ETdoFresh.ReadonlyInspectorAttribute
{
    public class ReadonlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginDisabledGroup(true);
            UnityEditor.EditorGUI.PropertyField(position, property, label, true);
            UnityEditor.EditorGUI.EndDisabledGroup();
        }
        
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
#endif
}