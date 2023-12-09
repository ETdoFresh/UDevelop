using System;
using ETdoFresh.UnityPackages.ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.GameEditor
{
    public class GameEditorScene : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Button manageProjectButton;
        [SerializeField] private Button gameSettingsPanel;
        [SerializeField] private Button scenesPanel;
        [SerializeField] private Button packagesPanel;
        [SerializeField] private GameObject sceneTabsParent;
        [SerializeField] private GameObject contextBar;
        [SerializeField] private GameObject projectEditorPanel;
        [SerializeField] private GameObject sceneEditorPanel;
        [SerializeField] private GameObject resourcesPanel;
        [SerializeField] private GameObject userProfilePanel;
        [SerializeField] private GameObject projectSettingsPanel;
        [SerializeField] private GameObject gameManagerPanel;
        [SerializeField] private GameObject globalVariablesPanel;
        [SerializeField] private GameObject iconsAndThumbnailsPanel;
        [SerializeField] private GameObject packagePanel;
        [SerializeField] private GameObject addScenePanel;
        [SerializeField] private GameObject sceneContextPanel;

        private void OnEnable()
        {
            gameSettingsPanel.onClick.AddPersistentListener(OnGameSettingsPanelClick);
            scenesPanel.onClick.AddPersistentListener(OnScenesPanelClick);
            packagesPanel.onClick.AddPersistentListener(OnPackagesPanelClick);
        }

        private void OnDisable()
        {
            gameSettingsPanel.onClick.RemovePersistentListener(OnGameSettingsPanelClick);
            scenesPanel.onClick.RemovePersistentListener(OnScenesPanelClick);
            packagesPanel.onClick.RemovePersistentListener(OnPackagesPanelClick);
        }
        
        private void OnGameSettingsPanelClick()
        {
            var gameSettingsPanelSiblingIndex = gameSettingsPanel.transform.GetSiblingIndex();
            for (var i = 0; i < gameSettingsPanel.transform.parent.childCount; i++)
            {
                var child = gameSettingsPanel.transform.parent.GetChild(i);
                if (child == gameSettingsPanel.transform) continue;
                child.SetSiblingIndex(child.GetSiblingIndex() > gameSettingsPanelSiblingIndex
                    ? child.GetSiblingIndex() - 1
                    : child.GetSiblingIndex() + 1);
            }
        }
        
        private void OnScenesPanelClick()
        {
            var scenesPanelSiblingIndex = scenesPanel.transform.GetSiblingIndex();
            for (var i = 0; i < scenesPanel.transform.parent.childCount; i++)
            {
                var child = scenesPanel.transform.parent.GetChild(i);
                if (child == scenesPanel.transform) continue;
                child.SetSiblingIndex(child.GetSiblingIndex() > scenesPanelSiblingIndex
                    ? child.GetSiblingIndex() - 1
                    : child.GetSiblingIndex() + 1);
            }
        }
        
        private void OnPackagesPanelClick()
        {
            var packagesPanelSiblingIndex = packagesPanel.transform.GetSiblingIndex();
            for (var i = 0; i < packagesPanel.transform.parent.childCount; i++)
            {
                var child = packagesPanel.transform.parent.GetChild(i);
                if (child == packagesPanel.transform) continue;
                child.SetSiblingIndex(child.GetSiblingIndex() > packagesPanelSiblingIndex
                    ? child.GetSiblingIndex() - 1
                    : child.GetSiblingIndex() + 1);
            }
        }
    }
}
