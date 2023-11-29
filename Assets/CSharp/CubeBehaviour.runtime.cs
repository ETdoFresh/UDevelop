using UnityEngine;

public class CubeBehaviour : RuntimeBehaviour
{
    private float _rotateSpeed = 300;
    private float _jumpForce = 10;
    private float _movementSpeed = 10;
    private Rigidbody _rigidbody;
    private Camera _camera;
	private bool _isRotating = false;

    public override void OnEnable()
    {
	    GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public override void Update()
    {
	    if (!_rigidbody) _rigidbody = GetComponent<Rigidbody>();
	    if (!_camera) _camera = Camera.main;
	    
		if (_isRotating)
		    transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime, Space.World);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			_rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
		}
		if (Input.GetKey(KeyCode.A))
		{
			var leftVector = -_camera.transform.right;
			leftVector.y = 0;
			_rigidbody.AddForce(leftVector * _movementSpeed, ForceMode.Force);
		}
		if (Input.GetKey(KeyCode.D))
		{
			var rightVector = _camera.transform.right;
			rightVector.y = 0;
			_rigidbody.AddForce(rightVector * _movementSpeed, ForceMode.Force);
		}
		if (Input.GetKey(KeyCode.W))
		{
			var forwardVector = _camera.transform.forward;
			forwardVector.y = 0;
			_rigidbody.AddForce(forwardVector * _movementSpeed, ForceMode.Force);
		}
		if (Input.GetKey(KeyCode.S))
		{
			var backwardVector = -_camera.transform.forward;
			backwardVector.y = 0;
			_rigidbody.AddForce(backwardVector * _movementSpeed, ForceMode.Force);
		}
		if (Input.GetKey(KeyCode.R))
		{
			transform.position = new Vector3(0, 0, 0);
			transform.rotation = Quaternion.identity;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
		}
    }
}
