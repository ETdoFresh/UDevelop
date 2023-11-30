using UnityEngine;

namespace Game.RuntimeScripts{
public class CubeBehaviour2 : RuntimeBehaviour2
{
	[SerializeField] private Camera cam;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private float rotateSpeed = 500;
	[SerializeField] private float jumpForce = 5;
	[SerializeField] private float movementSpeed = 5;
	[SerializeField] private bool isRotating = true;

    private void OnEnable()
    {
	    GetComponent<MeshRenderer>().material.color = Color.red;
    }

    private void Update()
    {
	    if (!rb) rb = GetComponent<Rigidbody>();
	    if (!cam) cam = Camera.main;
	    
		if (isRotating)
		    transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
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
			transform.position = new Vector3(0, 0, 0);
			transform.rotation = Quaternion.identity;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
    }
}
}