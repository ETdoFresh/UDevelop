using System;
using System.IO;
using System.Linq;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            try
            {
                return Engine.Execute(script).GetCompletionValue();
            }
            catch (JavaScriptException e)
            {
                throw new Exception($"[JavaScript] Line: {e.LineNumber} {e.Message}");
            }
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
            if (obj is JsValue objJsValue) return objJsValue;
            return JsValue.FromObject(Engine, obj);
        }
        
        public static string GetJson(JsValue obj)
        {
            return Engine.Json.Stringify(null, new[]{ obj }).AsString();
        }

        public static void Throw(JavaScriptException e)
        {
            throw new Exception($"[{e.GetType().Name}] Line: {e.LineNumber} Column: {e.Column} Source: {e.Location.Source} CallStack: {e.CallStack} ", e);
        }
    }
}