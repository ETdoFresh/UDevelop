using Jint.Native;
using QFSW.QC;

namespace Jint
{
    public static class JsQuantumConsole
    {
        public static JsValue RunJavaScriptCommand(params string[] args)
        {
            var script = string.Join(" ", args);
            return JavaScript.Run(script);
        }
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0) => RunJavaScriptCommand(new[] { arg0 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1) => RunJavaScriptCommand(new[] { arg0, arg1 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2) => RunJavaScriptCommand(new[] { arg0, arg1, arg2 });
            
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2, string arg3) => RunJavaScriptCommand(new[] { arg0, arg1, arg2, arg3 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2, string arg3, string arg4) => RunJavaScriptCommand(new[] { arg0, arg1, arg2, arg3, arg4 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5) => RunJavaScriptCommand(new[] { arg0, arg1, arg2, arg3, arg4, arg5 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6) => RunJavaScriptCommand(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7) => RunJavaScriptCommand(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8) => RunJavaScriptCommand(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        
        [Command("js", "Runs JavaScript code in Quantum Console.")]
        [Command("javascript", "Runs JavaScript code in Quantum Console.")]
        private static JsValue RunJavaScriptCommand(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9) => RunJavaScriptCommand(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
    }
}