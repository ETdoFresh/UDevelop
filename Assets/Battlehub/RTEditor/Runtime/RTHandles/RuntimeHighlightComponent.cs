using Battlehub.RTCommon;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Battlehub.RTHandles
{
    public interface IRuntimeHighlightComponent
    {
        event EventHandler HighlightChanged;
        bool Enabled
        {
            get;
            set;
        }

        IEnumerable<Renderer> Rendererers
        {
            get;
        }

        void Highlight(GameObject go);
        void ClearHighlight();
    }

    public class RuntimeHighlightComponent : MonoBehaviour, IRuntimeHighlightComponent
    {
        public event EventHandler HighlightChanged;

        [SerializeField]
        private bool m_useColliders = true;
        private LayerMask m_raycastMask;

        private IRTE m_editor;

        private bool m_updateRenderers;
        private bool m_allowPickRenderers;

        private readonly Renderer[] m_noRenderers = new Renderer[0];
        private Renderer[] m_allRenderers;
        private Renderer[] m_pickedRenderers;
        private IRuntimeSelectionComponent m_selectionComponent;
        private CameraMovementTracker m_cameraTracker;
        private PointerMovementTracker m_pointerMovementTracker;

        private Color32[] m_texPixels;
        private Vector2Int m_texSize;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private IRenderersCache m_cache;
        public IEnumerable<Renderer> Rendererers
        {
            get { return m_cache != null ? m_cache.Renderers : new Renderer[0]; }
        }

        private void Awake()
        {
            IOC.RegisterFallback<IRuntimeHighlightComponent>(this);
        }

        private void Start()
        {
            IOC.Register("HighlightRenderers", m_cache = gameObject.AddComponent<RenderersCache>());

            m_editor = IOC.Resolve<IRTE>();
            m_pointerMovementTracker = new PointerMovementTracker();
            m_pointerMovementTracker.Editor = m_editor;

            m_cameraTracker = new CameraMovementTracker();
            m_cameraTracker.Editor = m_editor;
            m_cameraTracker.ActiveWindow = m_editor.ActiveWindow;

            m_raycastMask = m_editor.CameraLayerSettings.RaycastMask;

            if (m_cameraTracker.ActiveWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Scene)
            {
                m_selectionComponent = m_cameraTracker.ActiveWindow.IOCContainer.Resolve<IRuntimeSelectionComponent>();
            }

            m_editor.Object.Enabled += OnObjectEnabled;
            m_editor.Object.Disabled += OnObjectDisabled;
            m_editor.Object.ComponentAdded += OnComponentAdded;

            m_editor.Selection.SelectionChanged += OnSelectionChanged;
            m_editor.ActiveWindowChanged += OnActiveWindowChanged;

            if (m_useColliders)
            {
                m_pickedRenderers = new Renderer[1];
            }
        }

        private void OnDestroy()
        {
            IOC.UnregisterFallback<IRuntimeHighlightComponent>(this);
            IOC.Unregister("HighlightRenderers", m_cache);

            if (m_editor != null)
            {
                if (m_editor.Object != null)
                {
                    m_editor.Object.Enabled -= OnObjectEnabled;
                    m_editor.Object.Disabled -= OnObjectDisabled;
                    m_editor.Object.ComponentAdded -= OnComponentAdded;
                }

                if (m_editor.Selection != null)
                {
                    m_editor.Selection.SelectionChanged -= OnSelectionChanged;
                }

                m_editor.ActiveWindowChanged -= OnActiveWindowChanged;
            }
        }

        private void Update()
        {
            if (m_selectionComponent == null)
            {
                return;
            }

            if (m_useColliders)
            {
                TryUpdateHighlightUsingColliders();
            }
            else
            {
                TryUpdateHighlight();
            }
        }

        private void RaiseHighlightChanged()
        {
            HighlightChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Highlight(GameObject go)
        {
            m_cache.Clear();
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            m_cache.Add(renderers, true, true);
            RaiseHighlightChanged();
        }

        public void ClearHighlight()
        {
            m_cache.Clear();
            RaiseHighlightChanged();
        }

        private void TryUpdateHighlightUsingColliders()
        {
            m_cameraTracker.Track();
            m_pointerMovementTracker.Track();
            m_updateRenderers |= m_cameraTracker.IsMoving;
            m_updateRenderers |= m_pointerMovementTracker.IsMoving;

            BaseHandle handle = GetCurrentHandle();
            if (m_cameraTracker.IsMoving || IsPointerOver(handle) || m_editor.Tools.ActiveTool == m_selectionComponent.BoxSelection && m_selectionComponent.BoxSelection != null)
            {
                m_pickedRenderers[0] = null;

                if (!m_cache.IsEmpty)
                {
                    m_cache.Clear();
                    RaiseHighlightChanged();
                }
            }
            else if (m_updateRenderers)
            {
                m_updateRenderers = false;

                var pointer = m_cameraTracker.ActiveWindow.Pointer;
                if (Physics.Raycast(pointer.Ray, out RaycastHit hit, float.MaxValue, m_raycastMask.value) && hit.collider.GetComponent<ExposeToEditor>())
                {
                    Renderer renderer = hit.collider.GetComponent<Renderer>();
                    if (m_pickedRenderers[0] != renderer)
                    {
                        m_cache.Clear();
                        m_pickedRenderers[0] = renderer;
                        if (m_pickedRenderers[0] != null)
                        {
                            Renderer[] renderersToAdd;
                            if (m_editor.Tools.SelectionMode == SelectionMode.Root)
                            {
                                renderersToAdd = RuntimeSelectionUtil.GetRoots(m_pickedRenderers)
                                    .OfType<GameObject>()
                                    .SelectMany(go => go.GetComponentsInChildren<Renderer>())
                                    .ToArray();
                            }
                            else
                            {
                                renderersToAdd = m_pickedRenderers;
                            }
                            m_cache.Add(renderersToAdd, true, true);
                        }
                        RaiseHighlightChanged();
                    }
                }
                else
                {
                    m_pickedRenderers[0] = null;

                    if (!m_cache.IsEmpty)
                    {
                        m_cache.Clear();
                        RaiseHighlightChanged();
                    }
                }
            }
        }

        private void TryUpdateHighlight()
        {
            bool wasCameraMoving = m_cameraTracker.IsMoving;
            m_cameraTracker.Track();
            m_pointerMovementTracker.Track();
            m_allowPickRenderers |= m_pointerMovementTracker.IsMoving;

            if (!m_cameraTracker.IsMoving)
            {
                m_updateRenderers |= wasCameraMoving;
                if (m_updateRenderers)
                {
                    m_updateRenderers = false;
                    m_allRenderers = m_editor.Object.Get(true).Where(go => go.ActiveSelf).SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray();
                    m_texPixels = m_selectionComponent.BoxSelection.BeginPick(out m_texSize, m_allRenderers);
                    m_pickedRenderers = null;
                }

                BaseHandle handle = GetCurrentHandle();
                if (IsPointerOver(handle) || m_editor.Tools.ActiveTool == m_selectionComponent.BoxSelection && m_selectionComponent.BoxSelection != null)
                {
                    m_pickedRenderers = null;
                    m_cache.Clear();
                    RaiseHighlightChanged();
                    return;
                }

                if (m_allowPickRenderers)
                {
                    Renderer[] renderers = m_noRenderers;
                    m_allowPickRenderers = false;
                    renderers = m_selectionComponent.BoxSelection.EndPick(m_texPixels, m_texSize, m_allRenderers);

                    bool updateCache = false;
                    if (m_pickedRenderers == null || !AreEqual(m_pickedRenderers, renderers))
                    {
                        m_pickedRenderers = renderers;
                        if (m_editor.Tools.SelectionMode == SelectionMode.Root)
                        {
                            renderers = RuntimeSelectionUtil.GetRoots(renderers)
                                .OfType<GameObject>()
                                .SelectMany(go => go.GetComponentsInChildren<Renderer>())
                                .ToArray();
                        }
                        updateCache = true;
                    }

                    if (updateCache)
                    {
                        m_cache.Clear();
                        m_cache.Add(renderers, true, true);
                        RaiseHighlightChanged();
                    }
                }
            }
            else
            {
                m_pickedRenderers = null;
                m_cache.Clear();
                RaiseHighlightChanged();
            }
        }

        private bool AreEqual(Renderer[] pickedRenderers, Renderer[] renderers)
        {
            if (pickedRenderers.Length != renderers.Length)
            {
                return false;
            }

            for (int i = 0; i < pickedRenderers.Length; ++i)
            {
                if (pickedRenderers[i] != renderers[i])
                {
                    return false;
                }
            }
            return true;
        }

        private BaseHandle GetCurrentHandle()
        {
            BaseHandle handle = null;
            switch (m_editor.Tools.Current)
            {
                case RuntimeTool.Move:
                    handle = m_selectionComponent.PositionHandle;
                    break;
                case RuntimeTool.Rotate:
                    handle = m_selectionComponent.RotationHandle;
                    break;
                case RuntimeTool.Scale:
                    handle = m_selectionComponent.ScaleHandle;
                    break;
                case RuntimeTool.Rect:
                    handle = m_selectionComponent.RectTool;
                    break;
                case RuntimeTool.Custom:
                    handle = m_selectionComponent.CustomHandle;
                    break;
            }

            return handle;
        }

        private bool IsPointerOver(BaseHandle handle)
        {
            return handle != null && handle.SelectedAxis != RuntimeHandleAxis.None;
        }

        private void OnObjectEnabled(ExposeToEditor obj)
        {
            m_updateRenderers = true;
        }

        private void OnObjectDisabled(ExposeToEditor obj)
        {
            m_updateRenderers = true;
        }

        private void OnComponentAdded(ExposeToEditor obj, Component arg)
        {
            m_updateRenderers = true;
        }

        private void OnSelectionChanged(UnityEngine.Object[] unselectedObjects)
        {
            m_updateRenderers = true;
        }

        private void OnActiveWindowChanged(RuntimeWindow window)
        {
            if (m_editor.ActiveWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Scene)
            {
                m_updateRenderers = true;
                m_cameraTracker.ActiveWindow = m_editor.ActiveWindow;
                m_selectionComponent = m_editor.ActiveWindow.IOCContainer.Resolve<IRuntimeSelectionComponent>();
            }
            else
            {
                m_cameraTracker.ActiveWindow = null;
                m_selectionComponent = null;
                m_allRenderers = null;
            }
        }

        private class PointerMovementTracker
        {
            private Vector2 m_prevPointer;
            private IRTE m_editor;
            public IRTE Editor
            {
                get { return m_editor; }
                set { m_editor = value; }
            }

            private bool m_isMoving;
            public bool IsMoving
            {
                get { return m_isMoving; }
            }

            public void Track()
            {
                Vector2 pointer = m_editor.Input.GetPointerXY(0);
                if (m_prevPointer != pointer)
                {
                    m_isMoving = true;
                    m_prevPointer = pointer;
                }
                else
                {
                    m_isMoving = false;
                }
            }
        }

        private class CameraMovementTracker
        {
            private Ray m_prevRay;
            private RuntimeWindow m_activeWindow;
            public RuntimeWindow ActiveWindow
            {
                get { return m_activeWindow; }
                set
                {
                    if (m_activeWindow != value)
                    {
                        m_activeWindow = value;
                        if (m_activeWindow != null)
                        {
                            m_prevRay = Ray;
                        }
                    }
                }
            }

            private Ray Ray
            {
                get { return new Ray(m_activeWindow.Camera.transform.position, m_activeWindow.Camera.transform.forward); }
            }

            private IRTE m_editor;
            public IRTE Editor
            {
                get { return m_editor; }
                set { m_editor = value; }
            }

            private float m_cooldownTime;
            private bool m_isMoving;
            public bool IsMoving
            {
                get { return m_isMoving; }
            }

            public void Track()
            {
                if (m_activeWindow == null)
                {
                    return;
                }

                Ray ray = Ray;
                if (m_prevRay.origin != ray.origin || m_prevRay.direction != ray.direction || Editor.Tools.IsViewing || !m_activeWindow.IsPointerOver)
                {
                    m_isMoving = true;
                    m_prevRay = ray;
                    m_cooldownTime = Time.time + 0.2f;
                }
                else
                {
                    if (m_cooldownTime <= Time.time)
                    {
                        m_isMoving = false;
                    }
                }
            }
        }
    }
}
