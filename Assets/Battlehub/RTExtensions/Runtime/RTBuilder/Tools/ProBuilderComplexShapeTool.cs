using Battlehub.ProBuilderIntegration;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.RTHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battlehub.RTBuilder
{
    public abstract class ProBuilderComplexShapeTool<T> : ProBuilderCustomTool where T : PBComplexShape
    {
        private MeshEditorState m_oldState;
        private Vector3[] m_oldPositions;

        private bool m_endEditOnPointerUp;
        private bool m_isShapeSelected = false;
        protected bool IsShapeSelected
        {
            get { return m_isShapeSelected; }
        }

        private ExposeToEditor m_exposeToEditor;
        protected ExposeToEditor ExposeToEditor
        {
            get { return m_exposeToEditor; }
        }

        private T m_target;
        protected T Target
        {
            get { return m_target; }
            set
            {
                m_target = value;
                if (m_target != null)
                {
                    m_exposeToEditor = m_target.GetComponent<ExposeToEditor>();
                }
                else
                {
                    m_exposeToEditor = null;
                }
            }
        }

        private Transform m_pivot;
        protected Transform Pivot
        {
            get { return m_pivot; }
        }

        private LockAxes m_lockAxes;
        protected LockAxes LockAxes
        {
            get { return m_lockAxes; }
        }

        private IRTE m_editor;
        protected IRTE Editor
        {
            get { return m_editor; }
        }

        private IProBuilderTool m_proBuilderTool;
        protected IProBuilderTool ProBuilderTool
        {
            get { return m_proBuilderTool; }
        }

        private IRuntimeSelectionComponent m_selectionComponent;
        protected IRuntimeSelectionComponent SelectionComponent
        {
            get { return m_selectionComponent; }
        }

        private ILocalization m_localization;
        protected ILocalization Localization
        {
            get { return m_localization; }
        }

        public override string Name
        {
            get { return typeof(T).Name; }
        }

        public virtual LayerMask LayerMask
        {
            get { return PlayerPrefs.GetInt($"Battlehub.RTBuilder.{typeof(T).Name}Editor.LayerMask", 0); }
            set { PlayerPrefs.SetInt($"Battlehub.RTBuilder.{typeof(T).Name}Editor.LayerMask", value); }
        }

        protected virtual string CreateShapeCommandText
        {
            get { return $"New {Name}"; }
        }

        protected virtual string EditShapeCommandText
        {
            get { return $"Edit {Name}"; }
        }

        protected override void Awake()
        {
            base.Awake();

            m_editor = IOC.Resolve<IRTE>();
            m_proBuilderTool = IOC.Resolve<IProBuilderTool>();
            m_localization = IOC.Resolve<ILocalization>();

            m_pivot = new GameObject($"{Name} Pivot").transform;

            m_lockAxes = m_pivot.gameObject.AddComponent<LockAxes>();
            m_lockAxes.RotationFree = m_lockAxes.RotationScreen = m_lockAxes.RotationX = m_lockAxes.RotationY = m_lockAxes.RotationZ = true;
            m_lockAxes.ScaleX = m_lockAxes.ScaleY = m_lockAxes.ScaleZ = true;

            m_pivot.transform.SetParent(transform);
        }

        protected virtual void Start()
        {
            m_proBuilderTool.ModeChanged += OnModeChanged;
            m_proBuilderTool.CustomToolChanged += OnCustomToolChanged;

            if (m_editor != null)
            {
                m_editor.ActiveWindowChanged += OnActiveWindowChanged;

                if (m_editor.ActiveWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Scene)
                {
                    m_selectionComponent = m_editor.ActiveWindow.IOCContainer.Resolve<IRuntimeSelectionComponent>();
                    SubscribeToEvents();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (m_proBuilderTool != null)
            {
                m_proBuilderTool.CustomToolChanged -= OnCustomToolChanged;
                m_proBuilderTool.ModeChanged -= OnModeChanged;
                m_proBuilderTool = null;
            }

            if (m_editor != null)
            {
                m_editor.ActiveWindowChanged -= OnActiveWindowChanged;
                m_editor = null;
            }

            UnsubscribeFromEvents();
            m_selectionComponent = null;
            m_localization = null;
        }

        protected virtual void LateUpdate()
        {
            if (m_exposeToEditor != null && m_exposeToEditor.MarkAsDestroyed)
            {
                EndEditing(true);
                m_exposeToEditor = null;
                m_endEditOnPointerUp = false;
                return;
            }

            if (m_editor.ActiveWindow == null || m_editor.ActiveWindow.WindowType != RuntimeWindowType.Scene || !m_editor.ActiveWindow.Camera)
            {
                m_endEditOnPointerUp = false;
                return;
            }

            if (Target != null && Target.IsEditing)
            {
                BaseHandle baseHandle = m_editor.Tools.ActiveTool as BaseHandle;
                if (baseHandle != null && baseHandle.IsDragging && m_editor.Selection.activeGameObject == m_pivot.gameObject)
                {
                    if (baseHandle is PositionHandle)
                    {
                        Target.SelectedPosition = Target.transform.InverseTransformPoint(m_pivot.position);
                        m_endEditOnPointerUp = false;
                    }
                }
                else
                {

                    if (m_editor.Input.GetPointerDown(0))
                    {
                        m_endEditOnPointerUp = true;
                        if (m_editor.Tools.ActiveTool is BaseHandle)
                        {
                            BaseHandle handle = (BaseHandle)m_editor.Tools.ActiveTool;
                            if (handle.IsDragging)
                            {
                                m_endEditOnPointerUp = false;
                            }
                        }
                    }
                    else if (m_editor.Input.GetKeyDown(KeyCode.Return))
                    {
                        EndEditing(true);
                    }
                    else if (m_editor.Input.GetPointerUp(0))
                    {
                        if (!m_editor.ActiveWindow.IsPointerOver || m_editor.Tools.ActiveTool != null && !(m_editor.Tools.ActiveTool is BoxSelection))
                        {
                            return;
                        }

                        if (m_endEditOnPointerUp)
                        {
                            if (OnClick())
                            {
                                EndEditing(false);
                            }
                        }
                    }
                }
            }
        }

        private void OnActiveWindowChanged(RuntimeWindow window)
        {
            UnsubscribeFromEvents();

            if (m_editor.ActiveWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Scene)
            {
                m_selectionComponent = m_editor.ActiveWindow.IOCContainer.Resolve<IRuntimeSelectionComponent>();
            }
            else
            {
                m_selectionComponent = null;
            }

            SubscribeToEvents();
        }

        protected virtual void SubscribeToEvents()
        {
            if (m_selectionComponent != null)
            {
                if (m_selectionComponent.PositionHandle != null)
                {
                    m_selectionComponent.PositionHandle.BeforeDrag.AddListener(OnBeginMove);
                    m_selectionComponent.PositionHandle.Drop.AddListener(OnEndMove);
                }
            }
        }

        protected virtual void UnsubscribeFromEvents()
        {
            if (m_selectionComponent != null)
            {
                if (m_selectionComponent.PositionHandle != null)
                {
                    m_selectionComponent.PositionHandle.BeforeDrag.RemoveListener(OnBeginMove);
                    m_selectionComponent.PositionHandle.Drop.RemoveListener(OnEndMove);
                }
            }
        }
        protected virtual void OnCustomToolChanged(string oldCustomToolName)
        {
            if (m_proBuilderTool.CustomTool == Name)
            {
                EnableShape();
            }
            else if (oldCustomToolName == Name)
            {
                DisableShape();
            }
        }

        protected virtual void OnModeChanged(ProBuilderToolMode oldMode)
        {
            if (m_proBuilderTool.CustomTool == Name)
            {
                if (oldMode == ProBuilderToolMode.Custom)
                {
                    m_proBuilderTool.CustomTool = null;
                }
            }
        }

        protected virtual void EnableShape()
        {
            Target = m_editor.Selection.activeGameObject.GetComponent<T>();
            UpdateShapePivot(Target);
            Target.IsEditing = true;
            SetLayer(Target.gameObject);
        }

        protected virtual void DisableShape()
        {
            if (Target != null)
            {
                Target.IsEditing = false;
            }

            if (m_exposeToEditor != null && !m_exposeToEditor.MarkAsDestroyed)
            {
                bool wasEnabled = m_editor.Undo.Enabled;
                m_editor.Undo.Enabled = false;

                m_editor.Selection.activeObject = Target.gameObject;
                UpdateShapePivot(Target);
                m_editor.Undo.Enabled = wasEnabled;
            }

            Target = null;
        }

        protected void SetLayer(GameObject go)
        {
            int layer = m_editor.CameraLayerSettings.AllScenesLayer;

            foreach (Transform child in go.GetComponentsInChildren<Transform>(true))
            {
                if (child.transform == go.transform)
                {
                    continue;
                }

                if (child.GetComponent<WireframeMesh>() != null)
                {
                    continue;
                }

                child.gameObject.layer = layer;
            }
        }

        protected virtual bool OnClick()
        {
            RuntimeWindow window = m_editor.ActiveWindow;
            return Target.Click(window.Camera, m_editor.Input.GetPointerXY(0), LayerMask.value);
        }

        protected virtual bool CanMoveToNextStage()
        {
            if(Target.Stage == 0)
            {
                return Target.VertexCount >= 3;
            }

            return false;
        }

        protected virtual void EndEditing(bool forceEndEditing)
        {
            m_endEditOnPointerUp = false;

            if (Target == null || !Target.IsEditing)
            {
                return;
            }

            if (Target.Stage == 0)
            {
                if (CanMoveToNextStage())
                {
                    Target.Stage++;
                }
                else
                {
                    m_proBuilderTool.Mode = ProBuilderToolMode.Object;
                    m_proBuilderTool.CustomTool = null;
                    return;
                }
            }

            if (Target.Stage > 0)
            {
                if (forceEndEditing)
                {
                    m_proBuilderTool.Mode = ProBuilderToolMode.Object;
                    m_proBuilderTool.CustomTool = null;
                }
                else
                {

                    bool wasEnabled = m_editor.Undo.Enabled;
                    m_editor.Undo.Enabled = false;
                    m_editor.Selection.activeObject = m_pivot.gameObject;
                    UpdateShapePivot(Target);
                    m_editor.Undo.Enabled = wasEnabled;
                }
            }
        }

        protected virtual void UpdateShapePivot(PBComplexShape shape)
        {
            if (shape.SelectedIndex >= 0)
            {
                m_pivot.position = Target.transform.TransformPoint(shape.SelectedPosition);
                m_pivot.rotation = Quaternion.identity;
            }
        }

        public void RecordState(
            MeshEditorState oldState, Vector3[] oldPositions,
            MeshEditorState newState, Vector3[] newPositions,
            bool oldStateChanged = true, bool newStateChanged = true,
            Action onAfterRedoCallback = null, Action onAfterUndoCallback = null)
        {

            PBComplexShape shape = Target;
            UndoRedoCallback redo = record =>
            {
                if (newState != null)
                {
                    m_proBuilderTool.Mode = ProBuilderToolMode.Object;
                    shape.SetState(newState);
                    if (newPositions != null)
                    {
                        shape.Positions = newPositions.ToList();
                        UpdateShapePivot(shape);
                    }

                    onAfterRedoCallback?.Invoke();
                    return newStateChanged;
                }

                onAfterRedoCallback?.Invoke();
                return false;
            };

            UndoRedoCallback undo = record =>
            {
                if (oldState != null)
                {
                    m_proBuilderTool.Mode = ProBuilderToolMode.Object;
                    shape.SetState(oldState);
                    if (oldPositions != null)
                    {
                        shape.Positions = oldPositions.ToList();
                        UpdateShapePivot(shape);
                    }
                    onAfterUndoCallback?.Invoke();
                    return oldStateChanged;
                }

                onAfterUndoCallback?.Invoke();
                return false;
            };

            IOC.Resolve<IRTE>().Undo.CreateRecord(redo, undo);
        }

        private void OnBeginMove(BaseHandle positionHandle)
        {
            if (m_proBuilderTool.CustomTool != Name)
            {
                return;
            }

            if (Target.Stage == 0)
            {
                return;
            }

            if (m_editor.Selection.activeGameObject != m_pivot.gameObject)
            {
                return;
            }

            positionHandle.EnableUndo = false;
            m_oldPositions = Target.Positions.ToArray();
            m_oldState = Target.GetState(true);
        }

        private void OnEndMove(BaseHandle positionHandle)
        {
            if (m_proBuilderTool.CustomTool != Name)
            {
                return;
            }
            if (Target.Stage == 0)
            {
                return;
            }
            if (m_editor.Selection.activeGameObject != m_pivot.gameObject)
            {
                return;
            }

            positionHandle.EnableUndo = true;

            Target.Refresh();

            MeshEditorState newState = Target.GetState(true);
            RecordState(m_oldState, m_oldPositions, newState, Target.Positions.ToArray());
        }

        public override void OnBeforeCommandsUpdate()
        {
            GameObject[] selected = m_editor.Selection.gameObjects;
            if (selected != null && selected.Length > 0)
            {
                m_isShapeSelected = selected.Where(go => go.GetComponent<T>() != null).Count() == 1;
            }
            else
            {
                m_isShapeSelected = false;
            }
        }

        public override void GetCommonCommands(List<ToolCmd> commands)
        {
            base.GetCommonCommands(commands);

            ToolCmd cmd = GetCustomToolsCmd(commands);
            if (cmd != null)
            {
                cmd.Children.Add(new ToolCmd(CreateShapeCommandText, OnNewShape, true));
                cmd.Children.Add(new ToolCmd(EditShapeCommandText, OnEditShape, () => m_isShapeSelected));
            }
        }

        protected virtual object OnNewShape(object arg)
        {
            ExposeToEditor exposeToEditor = CreateGameObject();

            IRuntimeEditor rte = IOC.Resolve<IRuntimeEditor>();
            RuntimeWindow scene = rte.GetWindow(RuntimeWindowType.Scene);

            Vector3 position;
            Quaternion rotation;
            m_proBuilderTool.GetPositionAndRotation(scene, out position, out rotation);
            exposeToEditor.transform.position = position;
            exposeToEditor.transform.rotation = rotation;

            PBComplexShape shape = CreateShape(exposeToEditor);
            shape.IsEditing = true;

            m_editor.Undo.BeginRecord();
            m_editor.Selection.activeGameObject = exposeToEditor.gameObject;
            m_proBuilderTool.CustomTool = Name;
            m_proBuilderTool.Mode = ProBuilderToolMode.Custom;
            m_editor.Undo.RegisterCreatedObjects(new[] { exposeToEditor });
            m_editor.Undo.EndRecord();

            return exposeToEditor.gameObject;
        }

        protected virtual ExposeToEditor CreateGameObject()
        {
            ExposeToEditor exposeToEditor = m_proBuilderTool.CreateNewShape(PBShapeType.Cube);
            exposeToEditor.SetName(Name);

            PBMesh pbMesh = exposeToEditor.GetComponent<PBMesh>();
            pbMesh.Clear();
            return exposeToEditor;
        }

        protected abstract PBComplexShape CreateShape(ExposeToEditor exposeToEditor);

        private void OnEditShape()
        {
            m_proBuilderTool.CustomTool = Name;
            m_proBuilderTool.Mode = ProBuilderToolMode.Custom;
        }
    }

}
