using ETdoFresh.Localbase;
using UnityEngine;

public class TestLocalbaseDatabase : MonoBehaviour
{
    private DatabaseReference test => LocalbaseDatabase.DefaultInstance.GetReference(endpoint);
    [SerializeField] private string endpoint = "/test/feature/1";

    private void OnEnable()
    {
        test.ValueChanged.AddListener(OnTestValueChanged);
    }

    private void OnDisable()
    {
        test.ValueChanged.RemoveListener(OnTestValueChanged);
    }

    private void OnTestValueChanged(ValueChangedEventArgs e)
    {
        Debug.Log(e.Snapshot.Value);
    }

    [ContextMenu("Set Text")]
    private void SetTextRandomInt()
    {
        test.SetValueAsync(UnityEngine.Random.Range(0, 100));
    }

    [ContextMenu("Clear Database")]
    private void ClearDatabase()
    {
        LocalbaseDatabase.DefaultInstance.Json = "";
    }
}