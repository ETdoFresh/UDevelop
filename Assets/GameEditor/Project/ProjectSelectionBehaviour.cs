using ETdoFresh.Localbase;
using UnityEngine;

public class ProjectSelectionBehaviour : MonoBehaviour
{
    private static DatabaseReference Projects => LocalbaseDatabase.DefaultInstance.GetReference("Projects");
    
    private void OnEnable()
    {
        Projects.ChildChanged.AddListener(OnProjectsChildChanged);
    }
    
    private void OnDisable()
    {
        Projects.ChildChanged.RemoveListener(OnProjectsChildChanged);
    }

    private void OnProjectsChildChanged(ChildChangedEventArgs e)
    {
        
    }
}
