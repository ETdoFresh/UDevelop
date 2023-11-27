using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming // Using Unity's naming convention for this Selection wrapper class

namespace CommandSystem.Commands
{
    public static class Selection
    {
        private static Object _activeContext;
        private static int _activeInstanceID;
        private static GameObject _activeGameObject;
        private static Object _activeObject;
        private static Transform _activeTransform;
        private static string[] _assetGUIDs;
        private static int _count;
        private static int[] _instanceIDs;
        private static GameObject[] _gameObjects;
        private static Object[] _objects;
        private static Action _selectionChanged;
        private static Transform[] _transforms;

#if UNITY_EDITOR
        private static bool IsInEditorMode => !Application.isPlaying;

        public static Object activeContext
        {
            get => IsInEditorMode ? UnityEditor.Selection.activeContext : _activeContext;
            set => SetActiveObject(value);
        }

        public static int activeInstanceID
        {
            get => IsInEditorMode ? UnityEditor.Selection.activeInstanceID : _activeInstanceID;
            set => SetActiveInstanceID(value);
        }

        public static GameObject activeGameObject
        {
            get => IsInEditorMode ? UnityEditor.Selection.activeGameObject : _activeGameObject;
            set => SetActiveGameObject(value);
        }

        public static Object activeObject
        {
            get => IsInEditorMode ? UnityEditor.Selection.activeObject : _activeObject;
            set => SetActiveObject(value);
        }

        public static Transform activeTransform
        {
            get => IsInEditorMode ? UnityEditor.Selection.activeTransform : _activeTransform;
            set => SetActiveTransform(value);
        }

        public static string[] assetGUIDs => IsInEditorMode ? UnityEditor.Selection.assetGUIDs : _assetGUIDs;

        public static int count => IsInEditorMode ? UnityEditor.Selection.count : _count;

        public static int[] instanceIDs
        {
            get => IsInEditorMode ? UnityEditor.Selection.instanceIDs : _instanceIDs;
            set => SetSelection(value);
        }

        public static GameObject[] gameObjects => IsInEditorMode ? UnityEditor.Selection.gameObjects : _gameObjects;

        public static Object[] objects
        {
            get => IsInEditorMode ? UnityEditor.Selection.objects : _objects;
            set => SetSelection(value);
        }

        public static Action selectionChanged =>
            IsInEditorMode ? UnityEditor.Selection.selectionChanged : _selectionChanged;

        public static Transform[] transforms => IsInEditorMode ? UnityEditor.Selection.transforms : _transforms;

#else
        public static bool IsInEditorMode => false;
            
        public static Object activeContext { get => _activeContext; set => SetActiveObject(value); }
        public static int activeInstanceID { get => _activeInstanceID; set => SetActiveInstanceID(value); }
        public static GameObject activeGameObject { get => _activeGameObject; set => SetActiveGameObject(value); }
        public static Object activeObject { get => _activeObject; set => SetActiveObject(value); }
        public static Transform activeTransform { get => _activeTransform; set => SetActiveTransform(value); }
        public static string[] assetGUIDs => _assetGUIDs;
        public static int count => _count;
        public static int[] instanceIDs { get => _instanceIDs; set => SetSelection(value); }
        public static GameObject[] gameObjects => _gameObjects;
        public static Object[] objects { get => _objects; set => SetSelection(value); }
        public static Action selectionChanged => _selectionChanged;
        public static Transform[] transforms => _transforms;
#endif

        private static void SetSelection(Object[] newObjects)
        {
#if UNITY_EDITOR
            if (IsInEditorMode)
            {
                UnityEditor.Selection.objects = newObjects;
                return;
            }
#endif
            _objects = newObjects;
            _count = newObjects.Length;
            _instanceIDs = _objects.Select(x => x.GetInstanceID()).ToArray();
            _transforms = _objects.Select(x => x as Transform).Where(x => x != null).ToArray();
            _gameObjects = _objects.Select(x => x as GameObject).Where(x => x != null).ToArray();
            _activeObject = _objects.FirstOrDefault();
            _activeTransform = _transforms.FirstOrDefault();
            _activeGameObject = _gameObjects.FirstOrDefault();
            _activeInstanceID = _activeObject != null ? _activeObject.GetInstanceID() : 0;
            _activeContext = _activeGameObject;
            #if UNITY_EDITOR
            _assetGUIDs = _objects.Select(UnityEditor.AssetDatabase.GetAssetPath)
                .Select(UnityEditor.AssetDatabase.AssetPathToGUID).ToArray();
            #endif
            _selectionChanged?.Invoke();
        }

        private static void SetSelection(int[] instanceIds)
        {
#if UNITY_EDITOR
            if (IsInEditorMode)
            {
                UnityEditor.Selection.instanceIDs = instanceIds;
                return;
            }

            SetSelection(instanceIds.Select(UnityEditor.EditorUtility.InstanceIDToObject).ToArray());
#endif
        }

        private static void SetActiveInstanceID(int newActiveInstanceID)
        {
#if UNITY_EDITOR
            if (IsInEditorMode)
            {
                UnityEditor.Selection.activeInstanceID = newActiveInstanceID;
                return;
            }
#endif

            _activeInstanceID = newActiveInstanceID;
        }

        private static void SetActiveObject(Object newActiveObject)
        {
#if UNITY_EDITOR
            if (IsInEditorMode)
            {
                UnityEditor.Selection.activeObject = newActiveObject;
                return;
            }
#endif

            _activeObject = newActiveObject;
        }

        private static void SetActiveGameObject(GameObject newActiveGameObject)
        {
#if UNITY_EDITOR
            if (IsInEditorMode)
            {
                UnityEditor.Selection.activeGameObject = newActiveGameObject;
                return;
            }
#endif

            _activeGameObject = newActiveGameObject;
        }

        private static void SetActiveTransform(Transform newActiveTransform)
        {
#if UNITY_EDITOR
            if (IsInEditorMode)
            {
                UnityEditor.Selection.activeTransform = newActiveTransform;
                return;
            }
#endif

            _activeTransform = newActiveTransform;
        }
    }
}