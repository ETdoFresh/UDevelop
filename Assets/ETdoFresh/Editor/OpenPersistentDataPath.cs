#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ETdoFresh.Editor
{
    public static class OpenPersistentDataPath
    {
        [MenuItem("ETdoFresh/Open Persistent Data Path")]
        public static void Open()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}
#endif