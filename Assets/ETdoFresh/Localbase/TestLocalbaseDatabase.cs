using GameEditor.Databases;
using UnityEngine;

public class TestLocalbaseDatabase : MonoBehaviour
{
    [SerializeField] private string endpoint = "/test/feature/1";

    private void OnEnable()
    {
        Database.AddValueChangedListener(endpoint, OnTestValueChanged);
    }

    private void OnDisable()
    {
        Database.RemoveValueChangedListener(endpoint, OnTestValueChanged);
    }

    private void OnTestValueChanged(object sender, IValueChangedEventArgs e)
    {
        Debug.Log(e);
    }

    [ContextMenu("Set Text")]
    private void SetTextRandomInt()
    {
        Database.SetValueAsync(endpoint, Random.Range(0, 100));
    }
}