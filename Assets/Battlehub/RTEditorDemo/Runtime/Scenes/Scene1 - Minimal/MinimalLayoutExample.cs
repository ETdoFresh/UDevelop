﻿using Battlehub.RTCommon;
using Battlehub.UIControls.DockPanels;
using UnityEngine;

namespace Battlehub.RTEditor.Examples.Scene1
{
    /// <summary>
    /// Initializes the runtime editor with a minimal layout and a scene with no header and frame
    /// </summary>
    public class MinimalLayoutExample : LayoutExtension
    {
        [SerializeField]
        private GameObject m_sceneWindow = null;

        protected override void OnInit()
        {
            base.OnInit();

            //Disable foreground ui layer.
            //Better in terms of performance, but does not allow to switch SceneWindow and GameWindow to "floating" mode when using UnversalRP or HDRP
            RenderPipelineInfo.UseForegroundLayerForUI = false;

            //Hide main menu and footer
            IRTEAppearance appearance = IOC.Resolve<IRTEAppearance>();
            appearance.IsMainMenuActive = false;
            appearance.IsFooterActive = false;
            appearance.IsUIBackgroundActive = false;
        }

        protected override void OnRegisterWindows(IWindowManager wm)
        {
            if(m_sceneWindow != null)
            {
                //Override scene window with borderless variant
                wm.OverrideWindow(BuiltInWindowNames.Scene, m_sceneWindow);
            }
        }

        protected override void OnBeforeBuildLayout(IWindowManager wm)
        {
            //Hide header toolbar
            wm.OverrideTools(null);
        }

        protected override LayoutInfo GetLayoutInfo(IWindowManager wm)
        {
            //Initializing a layout with one window - Scene
            LayoutInfo layoutInfo = wm.CreateLayoutInfo(BuiltInWindowNames.Scene);
            layoutInfo.IsHeaderVisible = false;

            return layoutInfo;
        }
    }

}