using UnityEngine;

using Battlehub.RTCommon;
namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(2)]
    public class ScaleHandle : BaseHandle
    {
        public bool AbsoluteGrid = false;
        public float GridSize = 0.1f;
        public Vector3 MinScale = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        private Vector3 m_prevPoint;
        private Matrix4x4 m_matrix;
        private Matrix4x4 m_inverse;

        private Vector3 m_roundedScale;
        private Vector3 m_scale;
        private Vector3[] m_refScales;
        private float m_screenScale;

        public override bool SnapToGrid
        {
            get { return AbsoluteGrid; }
            set { AbsoluteGrid = value; }
        }

        public override float SizeOfGrid
        {
            get { return GridSize; }
            set { GridSize = value; }
        }

        protected override float CurrentGridUnitSize
        {
            get { return SizeOfGrid; }
        }

        public override RuntimeTool Tool
        {
            get { return RuntimeTool.Scale; }
        }

        private LockObject m_sharedLockObject;
        protected override LockObject SharedLockObject
        {
            get { return base.SharedLockObject; }
            set
            {
                m_sharedLockObject = value;
                LockObject lockObject = m_sharedLockObject;
                if (m_currentMode != Mode.XYZ3D)
                {
                    lockObject = m_sharedLockObject != null ? new LockObject(m_sharedLockObject) : new LockObject();
                    switch (m_currentMode)
                    {
                        case Mode.XY2D:
                            lockObject.ScaleZ = true;
                            break;
                        case Mode.XZ2D:
                            lockObject.ScaleY = true;
                            break;
                        case Mode.YZ2D:
                            lockObject.ScaleX = true;
                            break;
                    }
                }
                base.SharedLockObject = lockObject;
            }
        }

        private Mode m_currentMode;
        protected override Mode CurrentMode
        {
            get { return m_currentMode; }
            set
            {
                if (m_currentMode != value)
                {
                    m_currentMode = value;
                    SharedLockObject = m_sharedLockObject;
                }
            }
        }

        [SerializeField]
        private bool m_isUniform = false;

        public bool IsUniform
        {
            get { return m_isUniform; }
            set
            {
                if (m_isUniform != value)
                {
                    m_isUniform = value;
                    TryUpdateModelProperties();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_scale = Vector3.one;
            m_roundedScale = m_scale;
        }

        protected override void Start()
        {
            base.Start();

            TryUpdateModelProperties();
        }

        protected override void UpdateOverride()
        {
            base.UpdateOverride();
            UpdateCurrentMode();
        }
        protected override bool OnBeginDrag()
        {
            if (!base.OnBeginDrag())
            {
                return false;
            }

            if (SelectedAxis == RuntimeHandleAxis.Free)
            {
                DragPlane = GetDragPlane(Vector3.zero);
            }
            else if (SelectedAxis == RuntimeHandleAxis.None)
            {
                return false;
            }

            m_refScales = new Vector3[ActiveTargets.Length];
            for (int i = 0; i < m_refScales.Length; ++i)
            {
                Quaternion rotation = PivotRotation == RuntimePivotRotation.Global ? ActiveTargets[i].rotation : Quaternion.identity;
                m_refScales[i] = rotation * ActiveTargets[i].localScale;
            }

            Vector3 axis = Vector3.zero;
            switch (SelectedAxis)
            {
                case RuntimeHandleAxis.X:
                    axis = Vector3.right;
                    break;
                case RuntimeHandleAxis.Y:
                    axis = Vector3.up;
                    break;
                case RuntimeHandleAxis.Z:
                    axis = Vector3.forward;
                    break;
            }

            if (SelectedAxis == RuntimeHandleAxis.Free)
            {
                m_prevPoint = Window.Pointer.ScreenPoint;
                return true;
            }

            DragPlane = GetDragPlane(axis);
            bool result = GetPointOnDragPlane(Window.Pointer, out m_prevPoint);
            if (!result)
            {
                SelectedAxis = RuntimeHandleAxis.None;
            }

            return result;
        }

        protected override void OnDrag()
        {
            base.OnDrag();


            if (SelectedAxis == RuntimeHandleAxis.Free)
            {
                Vector2 screenOffset = Window.Pointer.ScreenPoint - (Vector2)m_prevPoint;
                m_prevPoint = Window.Pointer.ScreenPoint;

                Rect rect = Window.ViewRoot.rect;
                float windowSize = Mathf.Max(rect.width, rect.height);
                float mag = screenOffset.magnitude / (windowSize * 0.25f);
                float sign = Mathf.Abs(screenOffset.x) > Mathf.Abs(screenOffset.y) ?
                     Mathf.Sign(screenOffset.x) :
                     Mathf.Sign(screenOffset.y);

                if (SharedLockObject != null)
                {
                    if (!SharedLockObject.ScaleX)
                    {
                        m_scale.x += sign * mag;
                    }

                    if (!SharedLockObject.ScaleY)
                    {
                        m_scale.y += sign * mag;
                    }

                    if (!SharedLockObject.ScaleZ)
                    {
                        m_scale.z += sign * mag;
                    }
                }
                else
                {
                    m_scale.x += sign * mag;
                    m_scale.y += sign * mag;
                    m_scale.z += sign * mag;
                }
            }
            else
            {
                if (!GetPointOnDragPlane(Window.Pointer, out Vector3 point))
                {
                    return;
                }


                Vector3 screenOffset = (point - m_prevPoint) / m_screenScale;
                m_prevPoint = point;

                Vector3 offset = m_inverse.MultiplyVector(screenOffset);
                float mag = offset.magnitude;
                if (SelectedAxis == RuntimeHandleAxis.X)
                {
                    if (SharedLockObject == null || !SharedLockObject.ScaleX)
                    {
                        float ds = Mathf.Sign(offset.x) * mag;
                        if (IsUniform)
                        {
                            m_scale += Vector3.one * ds;
                        }
                        else
                        {
                            m_scale.x += ds;
                        }

                    }
                }
                else if (SelectedAxis == RuntimeHandleAxis.Y)
                {
                    if (SharedLockObject == null || !SharedLockObject.ScaleY)
                    {
                        float ds = Mathf.Sign(offset.y) * mag;
                        if (IsUniform)
                        {
                            m_scale += Vector3.one * ds;
                        }
                        else
                        {
                            m_scale.y += ds;
                        }
                    }
                }
                else if (SelectedAxis == RuntimeHandleAxis.Z)
                {
                    if (SharedLockObject == null || !SharedLockObject.ScaleZ)
                    {
                        float ds = Mathf.Sign(offset.z) * mag;
                        if (IsUniform)
                        {
                            m_scale += Vector3.one * ds;
                        }
                        else
                        {
                            m_scale.z += ds;
                        }
                    }
                }
            }

            if (SnapToGrid)
            {
                for (int i = 0; i < m_refScales.Length; ++i)
                {
                    Quaternion rotation = PivotRotation == RuntimePivotRotation.Global ? Targets[i].rotation : Quaternion.identity;

                    float gridSize = EffectiveGridUnitSize * 2;

                    m_roundedScale = Vector3.Scale(m_refScales[i], m_scale);
                    if (EffectiveGridUnitSize > 0.01)
                    {
                        m_roundedScale.x = Mathf.RoundToInt(m_roundedScale.x / gridSize) * gridSize;
                        m_roundedScale.y = Mathf.RoundToInt(m_roundedScale.y / gridSize) * gridSize;
                        m_roundedScale.z = Mathf.RoundToInt(m_roundedScale.z / gridSize) * gridSize;
                    }

                    Vector3 scale = Quaternion.Inverse(rotation) * m_roundedScale;
                    scale.x = Mathf.Max(MinScale.x, scale.x);
                    scale.y = Mathf.Max(MinScale.y, scale.y);
                    scale.z = Mathf.Max(MinScale.z, scale.z);
                    ActiveTargets[i].localScale = scale;
                }

                if (Model != null)
                {
                    Model.SetScale(m_scale);
                }
            }
            else
            {
                m_roundedScale = m_scale;

                if (EffectiveGridUnitSize > 0.01)
                {
                    m_roundedScale.x = Mathf.RoundToInt(m_roundedScale.x / EffectiveGridUnitSize) * EffectiveGridUnitSize;
                    m_roundedScale.y = Mathf.RoundToInt(m_roundedScale.y / EffectiveGridUnitSize) * EffectiveGridUnitSize;
                    m_roundedScale.z = Mathf.RoundToInt(m_roundedScale.z / EffectiveGridUnitSize) * EffectiveGridUnitSize;
                }

                if (Model != null)
                {
                    Model.SetScale(m_roundedScale);
                }

                for (int i = 0; i < m_refScales.Length; ++i)
                {
                    Quaternion rotation = PivotRotation == RuntimePivotRotation.Global ? Targets[i].rotation : Quaternion.identity;

                    Vector3 scale = Quaternion.Inverse(rotation) * Vector3.Scale(m_refScales[i], m_roundedScale);

                    scale.x = Mathf.Max(MinScale.x, scale.x);
                    scale.y = Mathf.Max(MinScale.y, scale.y);
                    scale.z = Mathf.Max(MinScale.z, scale.z);
                    ActiveTargets[i].localScale = scale;
                }
            }
        }

        protected override void OnDrop()
        {
            base.OnDrop();

            m_scale = Vector3.one;
            m_roundedScale = m_scale;
            if (Model != null)
            {
                Model.SetScale(m_roundedScale);
            }
        }

        private RTHDrawingSettings m_settings = new RTHDrawingSettings();
        protected override void RefreshCommandBuffer(IRTECamera camera)
        {
            m_settings.Position = Target.position;
            m_settings.Rotation = Rotation;
            m_settings.Scale = m_roundedScale;
            m_settings.SelectedAxis = SelectedAxis;
            m_settings.LockObject = SharedLockObject;

            Appearance.DoScaleHandle(camera.CommandBuffer, camera.Camera, m_settings, IsUniform);
        }
        public override RuntimeHandleAxis HitTest(out float distance)
        {
            m_screenScale = RuntimeHandlesComponent.GetScreenScale(transform.position, Window.Camera) * Appearance.HandleScale;
            m_matrix = Matrix4x4.TRS(transform.position, Rotation, Appearance.InvertZAxis ? new Vector3(1, 1, -1) : Vector3.one);
            m_inverse = m_matrix.inverse;

            RuntimeHandleAxis axis = Model != null ?
                Model.HitTest(Window.Pointer, out distance) :
                Appearance.HitTestScaleHandle(Window.Camera, Window.Pointer, m_settings, out distance);

            return axis;
        }

        private void TryUpdateModelProperties()
        {
            var scaleHandleModel = Model as ScaleHandleModel;
            if (scaleHandleModel != null)
            {
                scaleHandleModel.IsUniform = IsUniform;
            }
        }
    }
}