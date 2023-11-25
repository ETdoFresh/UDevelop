using System.IO;
using Jint.Native;

namespace Jint
{
    public static class JavaScript
    {
        private static Engine _engine;

        public static Engine Engine => _engine ??= new Engine();

        public static JsValue Run(string script)
        {
            return Engine.Execute(script).GetCompletionValue();
        }
        
        public static JsValue RunFile(string path)
        {
            return Run(File.ReadAllText(path));
        }
    }
}
