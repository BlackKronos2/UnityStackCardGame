using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardCraft : MonoBehaviour
{
	private DragAndDrop _dragAndDrop;

	[SerializeField]
	private UIFillBar _progressBar;

	private bool _crafting;

	private void Awake()
	{
		_dragAndDrop = GetComponent<DragAndDrop>();
		_dragAndDrop.OnDraggingStart.AddListener(CraftStop);

		_crafting = false;
	}

	public void CraftStop()
	{
		StopAllCoroutines();
		_progressBar.Value = 0;
		_crafting = false;
	}

	private IEnumerator Crafting(CardRecipe cardRecipe, List<Card> cards)
	{
		_crafting = true;
		_progressBar.BarReset(cardRecipe.CraftTime);

		float timer = cardRecipe.CraftTime;
		float step = 0.1f;

		while (timer > 0)
		{
			yield return new WaitForSeconds(step);
			_progressBar.Value += step;

			timer -= step;
		}

		_progressBar.Value = 0;
		_crafting = false;
		CraftSuccess(cardRecipe, cards);
	}

	private void CraftSuccess(CardRecipe cardRecipe, List<Card> cards)
	{
		var cardList = cardRecipe.FullResultCardsList();

		foreach (var card in cardList)
			CardCraftSystem.Instance.CreateCard(card, transform.position);

		int[] cardsIdToDestoroy = cardRecipe.EntryCards.Where(c => c.DestroyedOnCraft).Select(c => c.CardData.CardInfo.Id).ToArray();

		for (int i = 0; i < cardsIdToDestoroy.Length; i++)
		{
			var cardToDestoy = cards.FirstOrDefault(c => c.CardData.Id == cardsIdToDestoroy[i]);
			if (cardsIdToDestoroy != null)
			{
				cards.Remove(cardToDestoy);
				Destroy(cardToDestoy.gameObject);
			}
		}

		Craft(cards);
	}

	public void Craft(List<Card> cards)
	{
		if (_crafting)
			return;

		var receipt = CardCraftSystem.Instance.GetCardRecipe(cards.Select(c => c.CardData.Id).ToArray());

		if (receipt == null)
			return;

		StartCoroutine(Crafting(receipt, cards));
	}
}
