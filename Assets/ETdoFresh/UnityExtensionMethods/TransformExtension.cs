using System.Linq;
using UnityEngine;

namespace ETdoFresh.UnityExtensionMethods
{
    public static class TransformExtension
    {
        public static Transform[] GetSiblings(this Transform transform)
        {
            var siblingIndex = transform.GetSiblingIndex();
            var parent = transform.parent;
            var children = parent.GetChildren() ?? transform.gameObject.scene.GetRootGameObjects().Select(x => x.transform).ToArray();
            var siblings = new Transform[children.Length - 1];
            for (var i = 0; i < children.Length; i++)
            {
                if (i == siblingIndex) continue;
                siblings[i > siblingIndex ? i - 1 : i] = children[i];
            }
            return siblings;
        }
        
        public static Transform[] GetChildren(this Transform transform)
        {
            if (transform == null) return null;
            var children = new Transform[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }
            return children;
        }
    }
}