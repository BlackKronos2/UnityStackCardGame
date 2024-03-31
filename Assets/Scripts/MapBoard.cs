using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapBoard : MonoBehaviour
{
	public static MapBoard Instance { get; private set; }

	private Camera _mainCamera;

	public Vector3 CurrentMousePosition { get; private set; }


	private void Awake()
	{
		Instance = this;
		_mainCamera = Camera.main;
	}

	private void Update()
	{
		Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray);


		foreach (var hit in hits.Reverse())
		{
			if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Table")) continue;
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red);

			CurrentMousePosition = hit.point;
			break;
		}
	}
}
