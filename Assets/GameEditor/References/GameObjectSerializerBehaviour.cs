using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectSerializerBehaviour : MonoBehaviour
{
    [SerializeField] private Object objectToSerialize;
    [SerializeField, TextArea(3,300)] private string serializedObject;

    private void Serialize()
    {
        serializedObject = GameObjectSerializerUtility.Serialize(objectToSerialize);
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GameObjectSerializerBehaviour))]
    private class GameObjectSerializerBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Serialize"))
            {
                ((GameObjectSerializerBehaviour)target).Serialize();
            }
        }
    }
#endif
}