using Battlehub.RTCommon;
using Battlehub.RTScripting;
using Battlehub.RTSL.Interface;
using Battlehub.UIControls.MenuControl;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Battlehub.RTEditor.Examples.Scene60
{
    [MenuDefinition]
    public class RuntimeScriptingExampleMenu : MonoBehaviour
    {
        private static string s_nl = Environment.NewLine;
        private static string s_csTemplate = 
                "using System.Collections;" + s_nl +
                "using System.Collections.Generic;" + s_nl +
                "using UnityEngine;" + s_nl + s_nl +

                "public class {0} : MonoBehaviour" + s_nl +
                "{{" + s_nl +
                "    // Start is called before the first frame update" + s_nl +
                "    void Start()" + s_nl +
                "    {{" + s_nl +
                "       {1}" + s_nl +
                "    }}" + s_nl + s_nl +

                "    // Update is called once per frame" + s_nl +
                "    void Update()" + s_nl +
                "    {{" + s_nl +
                "       {2}" + s_nl +
                "    }}" + s_nl + s_nl +

                "    void OnRuntimeEditorOpened()" + s_nl +
                "    {{" + s_nl +
                "       Debug.Log(\"Editor Opened\");" + s_nl +
                "    }}" + s_nl + s_nl +

                "    void OnRuntimeEditorClosed()" + s_nl +
                "    {{" + s_nl +
                "       Debug.Log(\"Editor Closed\");" + s_nl +
                "    }}" + s_nl + s_nl +

                "    void RuntimeAwake()" + s_nl +
                "    {{" + s_nl +
                "       Debug.Log(\"Awake in play mode\");" + s_nl +
                "    }}" + s_nl + s_nl +

                "    void RuntimeStart()" + s_nl +
                "    {{" + s_nl +
                "       Debug.Log(\"Start in play mode\");" + s_nl +
                "    }}" + s_nl + s_nl +

                "    void OnRuntimeDestroy()" + s_nl +
                "    {{" + s_nl +
                "       Debug.Log(\"Destroy in play mode\");" + s_nl +
                "    }}" + s_nl + s_nl +

                "    void OnRuntimeActivate()" + s_nl +
                "    {{" + s_nl +
                "       Debug.Log(\"Game view activated\");" + s_nl +
                "    }}" + s_nl + s_nl +

                "    void OnRuntimeDeactivate()" + s_nl +
                "    {{" + s_nl +
                "       Debug.Log(\"Game view deactivated\");" + s_nl +
                "    }}" + s_nl + s_nl +

                "}}";

        [MenuCommand("Example/Create Script")]       
        public async void CreateScript()
        {
            IProjectAsync project = IOC.Resolve<IProjectAsync>();
            IRuntimeScriptManager scriptManager = IOC.Resolve<IRuntimeScriptManager>();

            string desiredTypeName = "MyHelloWorld";

            ProjectItem assetItem = await scriptManager.CreateScriptAsync(project.State.RootFolder, desiredTypeName);
            RuntimeTextAsset cs = await scriptManager.LoadScriptAsync(assetItem);

            string typeName = cs.name;

            cs.Text = string.Format(s_csTemplate, typeName, "Debug.Log(\"Hello World\");", " ", "");
            await scriptManager.SaveScriptAsync(assetItem, cs);
            await scriptManager.CompileAsync();

            GameObject testGo = new GameObject("RT Scripting example");
            testGo.AddComponent<ExposeToEditor>();
            testGo.AddComponent(scriptManager.GetType(typeName));

            await Task.Yield();

            IRTE editor = IOC.Resolve<IRTE>();
            editor.IsPlaying = true;

            await Task.Yield();

            editor.IsPlaying = false;
        }
    }
}
