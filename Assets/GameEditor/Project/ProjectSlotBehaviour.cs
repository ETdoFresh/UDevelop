using System;
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
    private UnityAction<ProjectSlotBehaviour> onClickAction;
    
    public string Guid => data.guid;
    public Button Button => button;
    public ProjectJsonObject Data => data;

    private void OnEnable()
    {
        button.onClick.AddPersistentListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemovePersistentListener(OnClick);
    }

    public void SetData(ProjectJsonObject data, UnityAction<ProjectSlotBehaviour> onClickAction)
    {
        this.data = data;
        this.onClickAction = onClickAction;
        nameText.text = data.name;
    }

    private void OnClick()
    {
        onClickAction?.Invoke(this);
    }
}
