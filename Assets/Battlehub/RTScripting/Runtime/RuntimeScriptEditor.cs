using Battlehub.RTCommon;
using Battlehub.RTEditor;

namespace Battlehub.RTScripting
{
    public class RuntimeScriptEditor : ComponentEditor
    {
        public override void BuildEditor()
        {
            base.BuildEditor();

            if (IconImage != null)
            {
                ISettingsComponent settingsComponent = IOC.Resolve<ISettingsComponent>();
                BuiltInWindowsSettings settings;
                if (settingsComponent == null)
                {
                    settings = BuiltInWindowsSettings.Default;
                }
                else
                {
                    settings = settingsComponent.BuiltInWindowsSettings;
                }


                if (settingsComponent.SelectedTheme != null)
                {
                    IconImage.sprite = settingsComponent.SelectedTheme.GetIcon("cs Script Icon");
                    IconImage.transform.parent.gameObject.SetActive(IconImage.sprite != null && settings.Inspector.ComponentEditor.ShowIcon);
                }
            }
        }

        protected override void BuildEditor(IComponentDescriptor componentDescriptor, PropertyDescriptor[] descriptors)
        {
            
        }
    }
}
