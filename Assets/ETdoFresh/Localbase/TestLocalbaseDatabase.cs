using GameEditor.Databases;
using UnityEngine;
using ValueChangedEventArgs = Firebase.Database.ValueChangedEventArgs;

public class TestLocalbaseDatabase : MonoBehaviour
{
    [SerializeField] private string endpoint = "/test/feature/1";

    private void OnEnable()
    {
        Database.ValueChanged.AddListener(endpoint, OnTestValueChanged);
    }

    private void OnDisable()
    {
        Database.ValueChanged.RemoveListener(endpoint, OnTestValueChanged);
    }

    private void OnTestValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log(e);
    }

    [ContextMenu("Set Text")]
    private void SetTextRandomInt()
    {
        Database.SetValueAsync(endpoint, Random.Range(0, 100));
    }
}