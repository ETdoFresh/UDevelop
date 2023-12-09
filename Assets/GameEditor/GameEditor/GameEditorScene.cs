using ETdoFresh.UnityPackages.ExtensionMethods;
using GameEditor.UI;
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
        [SerializeField] private CollapsableSectionBehaviour collapsableGameSettingsPanel;
        [SerializeField] private CollapsableSectionBehaviour collapsableScenesPanel;
        [SerializeField] private CollapsableSectionBehaviour collapsablePackagesPanel;
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
        }
        
        private void OnScenesPanelClick()
        {
            
        }
        
        private void OnPackagesPanelClick()
        {
            
        }
    }
}
