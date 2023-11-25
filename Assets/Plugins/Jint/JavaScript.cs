using System;
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
            var engine = new Engine(cfg => cfg
                .AllowClr()
                .AllowClr(typeof(Debug).Assembly)
            );
            engine.SetValue("UnityEngine", new NamespaceReference(engine, "UnityEngine"));
            return engine;
        }

        public static JsValue Evaluate(string script)
        {
            if (script == "restart-engine")
            {
                _engine = null;
                return "Engine restarted.";
            }
            return Engine.Execute(script).GetCompletionValue();
        }

        public static Engine SetValue(string name, object value)
        {
            return Engine.SetValue(name, value);
        }

        public static JsValue EvaluateFile(string path)
        {
            return Evaluate(File.ReadAllText(path));
        }

        public static JsValue GetJsValue(object obj)
        {
            var guid = "guid" + Guid.NewGuid().ToString("N");
            Engine.SetValue(guid, obj);
            var jsValue = Evaluate(guid);
            Evaluate($"delete {guid};");
            return jsValue;
        }
    }
}