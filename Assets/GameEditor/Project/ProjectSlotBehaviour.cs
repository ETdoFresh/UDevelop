using ETdoFresh.UnityPackages.ExtensionMethods;
using GameEditor.Project;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProjectSlotBehaviour : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private ProjectJsonObject data;
    private UnityAction<ProjectSlotBehaviour> _onClickAction;

    public ProjectJsonObject Data => data;

    private void OnEnable()
    {
        button.onClick.AddPersistentListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemovePersistentListener(OnClick);
    }

    public void SetData(ProjectJsonObject newData, UnityAction<ProjectSlotBehaviour> onClickAction)
    {
        data = newData;
        _onClickAction = onClickAction;
        nameText.text = $"{newData.name}\n[{newData.guid[..8]}]";
    }

    private void OnClick()
    {
        _onClickAction?.Invoke(this);
    }
}
