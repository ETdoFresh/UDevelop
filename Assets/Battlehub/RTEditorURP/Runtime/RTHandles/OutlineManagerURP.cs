using UnityEngine;
using Battlehub.RTCommon;
using System.Linq;
using Battlehub.Utils;
using System.Collections.Generic;
using System.Collections;

namespace Battlehub.RTHandles.URP
{
    public class OutlineManagerURP : MonoBehaviour, IOutlineManager
    {
        private IRenderersCache m_cache;
        private IRTE m_editor;
        private IEnumerator m_coUpdate;

        private IRuntimeSelection m_selectionOverride;
        public IRuntimeSelection Selection
        {
            get
            {
                if (m_selectionOverride != null)
                {
                    return m_selectionOverride;
                }

                return m_editor.Selection;
            }
            set
            {
                if (m_selectionOverride != value)
                {
                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged -= OnSelectionChanged;
                    }

                    m_selectionOverride = value;
                    if (m_selectionOverride == m_editor.Selection)
                    {
                        m_selectionOverride = null;
                    }

                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged += OnSelectionChanged;
                    }
                }
            }
        }

        private void Start()
        {
            m_cache = GetComponentInChildren<IRenderersCache>();
            IOC.Register("SelectedRenderers", m_cache);

            m_editor = IOC.Resolve<IRTE>();

            TryToAddRenderers(m_editor.Selection);
            m_editor.Selection.SelectionChanged += OnRuntimeEditorSelectionChanged;
            m_editor.Object.Enabled += OnObjectEnabled;
            m_editor.Object.Disabled += OnObjectDisabled;

            IOC.RegisterFallback<IOutlineManager>(this);
        }

        private void OnDisable()
        {
            if(m_editor != null)
            {
                if (m_selectionOverride != null)
                {
                    OnSelectionChanged(m_selectionOverride.objects);
                }
                else
                {
                    OnRuntimeEditorSelectionChanged(m_editor.Selection.objects);
                }
            }
        }

        private void OnDestroy()
        {
            if (m_editor != null)
            {
                if(m_editor.Selection != null)
                {
                    m_editor.Selection.SelectionChanged -= OnRuntimeEditorSelectionChanged;
                }

                if(m_editor.Object != null)
                {
                    m_editor.Object.Enabled -= OnObjectEnabled;
                    m_editor.Object.Disabled -= OnObjectDisabled;
                }
            }

            if (m_selectionOverride != null)
            {
                m_selectionOverride.SelectionChanged -= OnSelectionChanged;
            }

            if (m_coUpdate != null)
            {
                StopCoroutine(m_coUpdate);
                m_coUpdate = null;
            }

            IOC.Unregister("SelectedRenderers", m_cache);
            IOC.UnregisterFallback<IOutlineManager>(this);           
        }

        private void OnObjectEnabled(ExposeToEditor obj)
        {
            if(m_coUpdate == null)
            {
                m_coUpdate = CoUpdate();
                StartCoroutine(m_coUpdate);
            }
        }

        private void OnObjectDisabled(ExposeToEditor obj)
        {
            if (m_coUpdate == null && isActiveAndEnabled)
            {
                m_coUpdate = CoUpdate();
                StartCoroutine(m_coUpdate);
            }
        }

        private IEnumerator CoUpdate()
        {
            yield return null;

            if (m_selectionOverride != null)
            {
                OnSelectionChanged(m_selectionOverride.objects);
            }
            else
            {
                OnRuntimeEditorSelectionChanged(m_editor.Selection.objects);
            }

            m_coUpdate = null;
        }

        private void OnRuntimeEditorSelectionChanged(Object[] unselectedObject)
        {
            OnSelectionChanged(m_editor.Selection, unselectedObject);
        }

        private void OnSelectionChanged(Object[] unselectedObjects)
        {
            OnSelectionChanged(m_selectionOverride, unselectedObjects);
        }

        private void OnSelectionChanged(IRuntimeSelection selection, Object[] unselectedObjects)
        {
            TryToRemoveRenderers(unselectedObjects);
            TryToAddRenderers(selection);
        }

        private void TryToRemoveRenderers(Object[] unselectedObjects)
        {
            if (unselectedObjects != null)
            {
                Renderer[] renderers = unselectedObjects.Select(go => go as GameObject).Where(go => go != null).SelectMany(go => go.GetComponentsInChildren<Renderer>(true)).ToArray();
                for (int i = 0; i < renderers.Length; ++i)
                {
                    Renderer renderer = renderers[i];
                    m_cache.Remove(renderer);
                }
            }
        }

        private void TryToAddRenderers(IRuntimeSelection selection)
        {
            if (selection.gameObjects != null)
            {
                IList<Renderer> renderers = GetRenderers(selection.gameObjects);
                for (int i = 0; i < renderers.Count; ++i)
                {
                    Renderer renderer = renderers[i];
                    m_cache.Add(renderer);
                }
            }
        }

        private IList<GameObject> FilterSelection(IList<GameObject> gameObjects)
        {
            IList<GameObject> result = new List<GameObject>();

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject go = gameObjects[i];
                if (go == null || go.IsPrefab() || (go.hideFlags & HideFlags.HideInHierarchy) != 0)
                {
                    continue;
                }

                ExposeToEditor exposed = go.GetComponent<ExposeToEditor>();
                if (exposed == null || exposed.ShowSelectionGizmo)
                {
                    result.Add(go);
                }
            }
            return result;
        }

        private IList<Renderer> GetRenderers(IList<GameObject> gameObjects)
        {
            List<Renderer> result = new List<Renderer>();

            gameObjects = FilterSelection(gameObjects);

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject go = gameObjects[i];

                foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
                {
                    if (renderer.gameObject.activeInHierarchy && (renderer.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        result.Add(renderer);
                    }
                }
            }

            return result;
        }

        public void AddRenderers(Renderer[] renderers)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                m_cache.Add(renderer);
            }
        }

        public void RemoveRenderers(Renderer[] renderers)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                m_cache.Remove(renderer);
            }
        }

        public void RecreateCommandBuffer()
        {
            
        }

        public bool ContainsRenderer(Renderer renderer)
        {
            return m_cache.Renderers.Contains(renderer);
        }
    }

}

