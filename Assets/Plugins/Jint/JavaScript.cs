using System;
using System.IO;
using System.Linq;
using ETdoFresh.UnityPackages.EventBusSystem;
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
                .AllowClr(typeof(EventBus).Assembly)
                .AllowClr(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "Assembly-CSharp"))
            );
            engine.SetValue("UnityEngine", new NamespaceReference(engine, "UnityEngine"));
            engine.SetValue("ETdoFresh", new NamespaceReference(engine, "ETdoFresh"));
            engine.SetValue("EventBus", TypeReference.CreateTypeReference(engine, typeof(EventBus)));
            engine.SetValue("AssemblyCSharp", AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "Assembly-CSharp"));
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