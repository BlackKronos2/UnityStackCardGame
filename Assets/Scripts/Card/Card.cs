using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardCraft))]
[RequireComponent(typeof(DragAndDrop))]
[RequireComponent(typeof(Rigidbody))]
public class Card : MonoBehaviour
{
	private static readonly Vector3 Offset = new Vector3(0, 0.01f, -1f);

	public const string DragCardLayer = "DraggedCard";
	public const string CardLayer = "Card";

	private Rigidbody _rigidbody;

	[SerializeField]
    private CardData _cardData;
	[SerializeField]
	private MeshRenderer _cardView;

	private DragAndDrop _dragAndDrop;
	private CardCraft _cardCraft;

	public Card ChildCard;
	public Card ParentCard;

	public CardCraft CardCraft => _cardCraft;
	public DragAndDrop DragAndDrop => _dragAndDrop;

	public CardData CardData
	{
		get { return _cardData; }
		set { 
			_cardData = value;
			CardViewSet(_cardData.CardSprite);
		}
	} 

	private void Awake()
	{
		_dragAndDrop = GetComponent<DragAndDrop>();
		_dragAndDrop.OnDraggingStart.AddListener(OnCardDrag);
		_dragAndDrop.OnDroppedOn.AddListener(OnCardDroppedOn);
		_dragAndDrop.OnDrop.AddListener(OnCardDrop);

		_cardCraft = GetComponent<CardCraft>();
		_rigidbody = GetComponent<Rigidbody>();

		CardViewSet(_cardData.CardSprite);
	}

	public List<Card> GetFullCardStack()
	{
		List<Card> cardStack = new List<Card>(0);
		cardStack.Add(this);

		var parentCard = ParentCard;
		var childCard = ChildCard;

		while (parentCard != null) 
		{
			cardStack.Add(parentCard);
			parentCard = parentCard.ParentCard;
		}

		while (childCard != null)
		{
			cardStack.Add(childCard);
			childCard = childCard.ChildCard;
		}

		return cardStack;
	}
	public Card MainParent() => GetFullCardStack().Find(c => c.ParentCard == null);
	public Card DownChild() => GetFullCardStack().Find(c => c.ChildCard == null);

	public void CraftTry()
	{
		CardCraft.CraftStop();

		if (ParentCard != null)
		{
			ParentCard.CraftTry();
			return;
		}

		CardCraft.Craft(GetFullCardStack());
	}

	private void CardViewSet(Sprite sprite)
	{
		Material material = new Material(Shader.Find("Standard"));
		material.mainTexture = sprite.texture;
		_cardView.material = material; 
	}

	#region DRAG_AND_DROP_EVENTS

	private void OnCardDrop()
	{
		var cards = GetFullCardStack();

		foreach (var card in cards)
		{
			card.DragAndDrop.VelocityZero();
			card.gameObject.layer = LayerMask.NameToLayer(CardLayer);
		}

		CraftTry();
	}
	private void OnCardDroppedOn(DragAndDrop cardDrag)
	{
		Card card = cardDrag.gameObject.GetComponent<Card>();

		if (!CardData.CanStack || !CanStack(card))
			return;

		JoinChildCard(card);

		CraftTry();
	}
	private void OnCardDrag()
	{
		MainParent().CardCraft.CraftStop();

		if (ParentCard != null)
		{
			ParentCard.TakeChildCard();
			ParentCard.CraftTry();
			ParentCard = null;
		}

		_dragAndDrop.DraggingEffect = GetFullCardStack().Count == 1;

		foreach (var card in GetFullCardStack())
		{
			card.DragAndDrop.VelocityZero();
			card.gameObject.layer = LayerMask.NameToLayer(DragCardLayer);
		}

		var joint = GetComponent<FixedJoint>();

		if(joint != null)
			Destroy(joint);
	}

	#endregion

	#region STACK_SYSTEM

	public void JoinChildCard(Card card)
	{
		if (GetFullCardStack().Contains(card))
			return;

		if (ChildCard != null)
		{
			ChildCard.JoinChildCard(card);
			return;
		}

		Vector3 childPosition = transform.position + Offset;
		card.transform.position = childPosition;

		FixedJoint joint = card.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = gameObject.GetComponent<Rigidbody>();

		card.ParentCard = this;
		ChildCard = card;

		MainParent().CardCraft.CraftStop();

		card.gameObject.layer = gameObject.layer;
		CraftTry();
	}
	public Card TakeChildCard()
	{
		if (ChildCard == null)
			return null;

		MainParent().CardCraft.CraftStop();

		var joint = ChildCard.GetComponent<FixedJoint>();
		if (joint != null)
			Destroy(joint);

		var card = ChildCard;
		ChildCard = null;

		CraftTry();
		return card;
	}

	private bool CanStack(Card card)
	{
		switch (card.CardData.CardType)
		{
			case CardType.LOCATION: return false;
			case CardType.STRUCTURE: return false;
			case CardType.RESOURCE: return CardData.CardType != CardType.LOCATION;

			default: return true;
		}
	}

	#endregion


	private const float PushForce = 0.15f;

	private void FixedUpdate()
	{
		if (_rigidbody.velocity.y == 0)
			CardsRepulsion();

	}

	private void CardsRepulsion()
	{
		// Применяем силу для отталкивания объектов друг от друга при столкновении
		Vector3 direction = Vector3.zero;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f); // Проверяем коллайдеры в радиусе 1 единицы
		foreach (Collider col in hitColliders)
		{
			if (col.gameObject != gameObject && col.gameObject.layer == gameObject.layer)
			{
				Vector3 pushDir = transform.position - col.transform.position; // Направление от текущего объекта к столкнувшемуся объекту
				direction += pushDir.normalized;
			}
		}
		if (direction != Vector3.zero)
			Debug.Log("Push");

		_rigidbody.AddForce(direction * PushForce, ForceMode.Impulse);
	}
}
