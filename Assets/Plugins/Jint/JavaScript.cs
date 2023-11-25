using System.IO;
using Jint.Native;
using Jint.Runtime.Interop;
using UnityEngine;

namespace Jint
{
    public static class JavaScript
    {
        private static Engine _engine;

        public static Engine Engine => _engine ??= CreateEngine();

        private static Engine CreateEngine()
        {
            var engine = new Engine(cfg => cfg.AllowClr());
            engine.SetValue("Debug", TypeReference.CreateTypeReference(engine, typeof(Debug)));
            engine.SetValue("Application", TypeReference.CreateTypeReference(engine, typeof(Application)));
            engine.SetValue("GameObject", TypeReference.CreateTypeReference(engine, typeof(GameObject)));
            engine.SetValue("Transform", TypeReference.CreateTypeReference(engine, typeof(Transform)));
            engine.SetValue("Vector3", TypeReference.CreateTypeReference(engine, typeof(Vector3)));
            engine.SetValue("Quaternion", TypeReference.CreateTypeReference(engine, typeof(Quaternion)));
            engine.SetValue("Time", TypeReference.CreateTypeReference(engine, typeof(Time)));
            engine.SetValue("Input", TypeReference.CreateTypeReference(engine, typeof(Input)));
            engine.SetValue("Resources", TypeReference.CreateTypeReference(engine, typeof(Resources)));
            engine.SetValue("Object", TypeReference.CreateTypeReference(engine, typeof(Object)));
            engine.SetValue("MonoBehaviour", TypeReference.CreateTypeReference(engine, typeof(MonoBehaviour)));
            engine.SetValue("Component", TypeReference.CreateTypeReference(engine, typeof(Component)));
            engine.SetValue("Behaviour", TypeReference.CreateTypeReference(engine, typeof(Behaviour)));
            engine.SetValue("Collider", TypeReference.CreateTypeReference(engine, typeof(Collider)));
            engine.SetValue("Collider2D", TypeReference.CreateTypeReference(engine, typeof(Collider2D)));
            engine.SetValue("Rigidbody", TypeReference.CreateTypeReference(engine, typeof(Rigidbody)));
            engine.SetValue("Rigidbody2D", TypeReference.CreateTypeReference(engine, typeof(Rigidbody2D)));
            engine.SetValue("SpriteRenderer", TypeReference.CreateTypeReference(engine, typeof(SpriteRenderer)));
            engine.SetValue("Sprite", TypeReference.CreateTypeReference(engine, typeof(Sprite)));
            engine.SetValue("Texture2D", TypeReference.CreateTypeReference(engine, typeof(Texture2D)));
            engine.SetValue("Texture", TypeReference.CreateTypeReference(engine, typeof(Texture)));
            return engine;
        }
        
        public static JsValue Run(string script)
        {
            if (script == "restart-engine")
            {
                _engine = null;
                return "Engine restarted.";
            }
            return Engine.Execute(script).GetCompletionValue();
        }
        
        public static JsValue RunFile(string path)
        {
            return Run(File.ReadAllText(path));
        }
    }
}
