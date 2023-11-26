using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ETdoFresh.UnityPackages.EventBusSystem;
using Jint.Native;
using Jint.Runtime.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Jint
{
    public static class JavaScript
    {
        private static string settingsPath = "Assets/JavaScript/JavaScriptSettings.json";
        private static Engine _engine;

        public static Engine Engine => _engine ??= CreateEngine();

        private static Engine CreateEngine()
        {
            var settingsJson = File.ReadAllText(settingsPath);
            var settings = JsonConvert.DeserializeObject(settingsJson) as JObject;
            
            var allowedAssemblies = settings["AllowedAssemblies"] as JArray;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var engine = new Engine(cfg =>
            {
                foreach (JObject assembly in allowedAssemblies)
                {
                    var assemblyName = assembly["assemblyName"].ToString(); 
                    cfg = cfg.AllowClr(assemblies.FirstOrDefault(x => x.GetName().Name == assemblyName));
                }
            });
            
            var assemblyCSharp = assemblies.FirstOrDefault(x => x.GetName().Name == "Assembly-CSharp");
            engine.SetValue("AssemblyCSharp", assemblyCSharp);
            
            var namespaces = settings["Namespaces"] as JArray;
            foreach (JObject ns in namespaces)
            {
                var namespaceName = ns["name"].ToString();
                var namespacePath = ns["namespacePath"].ToString();
                engine.SetValue(namespaceName, new NamespaceReference(engine, namespacePath));
            }
            
            var types = settings["Types"] as JArray;
            foreach (JObject type in types)
            {
                var typeName = type["name"].ToString();
                var typeFullName = type["typeFullName"].ToString();
                var systemType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(x => x.FullName == typeFullName);
                engine.SetValue(typeName, TypeReference.CreateTypeReference(engine, systemType));
            }
            
            var aliases = settings["Aliases"] as JArray;
            foreach (JObject alias in aliases)
            {
                var aliasName = alias["name"].ToString();
                var aliasJs = alias["js"].ToString();
                engine.SetValue(aliasName, engine.Execute(aliasJs).GetCompletionValue());
            }
            
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