using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCraftSystem : MonoBehaviour
{
    public static CardCraftSystem Instance { get; private set; }

	[SerializeField]
	private Card _cardPrefab;

	[SerializeField]
	private List<CardRecipe> CraftRecipes;

	private void Awake()
	{
		Instance = this;
	}

	public CardRecipe GetCardRecipe(int[] cardsId) => CraftRecipes.FirstOrDefault(r => r.FullEntryCardsList().Select(c => c.Id).OrderBy(v => v).SequenceEqual(cardsId.OrderBy(v => v)));

	public void CreateCard(CardData cardData, Vector3 point)
	{
		var card = Instantiate(_cardPrefab, point, Quaternion.identity);
		card.CardData = cardData;

		card.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.right) * 100f);
	}
}
