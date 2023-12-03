using UnityEditor;
using UnityEngine;

namespace ETdoFresh.Editor
{
    public class DebugLogGitVersion
    {
        [MenuItem("ETdoFresh/Debug Log Git Version")]
        public static void LogGitVersion()
        {
            try
            {
                var process = new System.Diagnostics.Process();
#if UNITY_EDITOR_WIN
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C git describe --tags --always"
                };
#else
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "/bin/bash",
                    Arguments = "-c \"git describe --tags --always\""
                };
#endif
                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Debug.Log(output.StartsWith("v") ? output : "v" + output);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}