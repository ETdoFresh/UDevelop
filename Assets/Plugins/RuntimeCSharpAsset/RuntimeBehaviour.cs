using UnityEngine;

public class RuntimeBehaviour
{
    protected RuntimeCSharpBehaviour self;
    protected Transform transform;
    protected GameObject gameObject;

    public RuntimeBehaviour() { }

    public static T Create<T>(RuntimeCSharpBehaviour self, Transform transform, GameObject gameObject)
        where T : RuntimeBehaviour, new()
    {
        return new T { self = self, transform = transform, gameObject = gameObject };
    }

    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
    public virtual void OnDestroy() { }
    public virtual void OnApplicationQuit() { }
    public virtual void OnApplicationFocus(bool hasFocus) { }
    public virtual void OnApplicationPause(bool pauseStatus) { }
    public virtual void OnValidate() { }
    public virtual void OnCollisionEnter(Collision collision) { }
    public virtual void OnCollisionStay(Collision collision) { }
    public virtual void OnCollisionExit(Collision collision) { }
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerStay(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnCollisionStay2D(Collision2D collision) { }
    public virtual void OnCollisionExit2D(Collision2D collision) { }
    public virtual void OnTriggerEnter2D(Collider2D collision) { }
    public virtual void OnTriggerStay2D(Collider2D collision) { }
    public virtual void OnTriggerExit2D(Collider2D collision) { }
    
    protected T GetComponent<T>() => gameObject.GetComponent<T>();
    protected T GetComponentInChildren<T>() => gameObject.GetComponentInChildren<T>();
    protected T GetComponentInParent<T>() => gameObject.GetComponentInParent<T>();
    protected T[] GetComponents<T>() => gameObject.GetComponents<T>();
    protected T[] GetComponentsInChildren<T>() => gameObject.GetComponentsInChildren<T>();
    protected T[] GetComponentsInParent<T>() => gameObject.GetComponentsInParent<T>();
    protected T FindObjectOfType<T>() where T : Object => Object.FindObjectOfType<T>();
    protected T[] FindObjectsOfType<T>() where T : Object => Object.FindObjectsOfType<T>();
    protected GameObject Instantiate(GameObject original) => Object.Instantiate(original);
    protected GameObject Instantiate(GameObject original, Transform parent) => Object.Instantiate(original, parent);
    protected GameObject Instantiate(GameObject original, Transform parent, bool instantiateInWorldSpace) => Object.Instantiate(original, parent, instantiateInWorldSpace);
    protected GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation) => Object.Instantiate(original, position, rotation);
    protected GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent) => Object.Instantiate(original, position, rotation, parent);
}