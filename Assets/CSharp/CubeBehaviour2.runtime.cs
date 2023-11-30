using RuntimeCSharp;
using UnityEngine;

namespace Game.RuntimeScripts
{
    public class CubeBehaviour2 : RuntimeBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float floatForce = 9.6f;
        [SerializeField] private float movementSpeed = 5;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }

        private void OnEnable()
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }

        private void Update()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            if (!cam) cam = Camera.main;
            
            if (Input.GetKey(KeyCode.A))
            {
                var leftVector = -cam.transform.right;
                leftVector.y = 0;
                rb.AddForce(leftVector * movementSpeed, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.D))
            {
                var rightVector = cam.transform.right;
                rightVector.y = 0;
                rb.AddForce(rightVector * movementSpeed, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.W))
            {
                var forwardVector = cam.transform.forward;
                forwardVector.y = 0;
                rb.AddForce(forwardVector * movementSpeed, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.S))
            {
                var backwardVector = -cam.transform.forward;
                backwardVector.y = 0;
                rb.AddForce(backwardVector * movementSpeed, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.R))
            {
                transform.position = _initialPosition;
                transform.rotation = _initialRotation;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        
        private void FixedUpdate()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * floatForce, ForceMode.Force);
        }
    }
}