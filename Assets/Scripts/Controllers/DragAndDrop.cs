using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class DragAndDrop : MonoBehaviour
{
	private const float DragYPos = 1;

	private const string TargetLayer = "Card";

	[SerializeField]
	[Range(1f, 15f)]
	private float _speed;

	private Rigidbody _rigidbody;
	[SerializeField]
	private bool _block;

	protected MapBoard MapBoard => MapBoard.Instance;

	public bool Block
	{
		get { return _block; }
		set
		{ 
			_block = value;
		}
	}
	public bool DraggingEffect { get; set; }

	#region EVENTS

	public UnityEvent OnDrop { get; set; } = new UnityEvent();
	public UnityEvent OnDraggingStart { get; set; } = new UnityEvent();

	public UnityEvent<DragAndDrop> OnDroppedOn { get; set; } = new UnityEvent<DragAndDrop>();

	#endregion

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		Block = false;
		DraggingEffect = true;
	}

	private void OnMouseDown()
	{
		OnDraggingStart?.Invoke();
	}

	public void VelocityZero()
	{
		_rigidbody.velocity = Vector3.zero;
		transform.rotation = Quaternion.identity;
	}

	private void OnMouseUp()
	{
		VelocityZero();

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit[] hits = Physics.RaycastAll(ray);

		var otherObject = hits.Select(hit => hit.collider.gameObject)
						   .Where(gObj => gObj != this.gameObject && gObj.layer == LayerMask.NameToLayer(TargetLayer))
						   .OrderByDescending(otherCard => otherCard.transform.position.y)
						   .FirstOrDefault();

		if (otherObject != null)
		{ 
			DragAndDrop otherObjectDragSystem = otherObject.gameObject.GetComponent<DragAndDrop>();
			otherObjectDragSystem.OnDroppedOn.Invoke(this);
		}

		OnDrop?.Invoke();
	}

	private void OnMouseDrag()
	{
		if (Block)
			return;

		Vector3 newWorldPosition = new Vector3(
			MapBoard.CurrentMousePosition.x,
			MapBoard.transform.position.y + DragYPos,
			MapBoard.CurrentMousePosition.z);

		var difference = newWorldPosition - transform.position;

		var speed = _speed * difference;
		_rigidbody.velocity = speed;

		if (!DraggingEffect)
			return;

		_rigidbody.rotation = Quaternion.Euler(new Vector3(speed.z, 0, -speed.x));
	}
}
