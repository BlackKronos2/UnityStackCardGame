using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private float _moveSpeed = 5.0f;

	private void Update()
	{
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");

		Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput) * _moveSpeed * Time.deltaTime;
		transform.Translate(moveDirection);
	}
}
