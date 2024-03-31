using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct CardStock 
{
	[Tooltip("�����")]
	public CardData CardInfo;
	[Tooltip("����������")]
	public int Count;
}

[Serializable]
public struct CardReceiptPart
{
    public string Name;

	[Tooltip("�����")]
	public CardStock CardData;
	[Tooltip("������������ ��� ���������� �������")]
	public bool DestroyedOnCraft;
}

[CreateAssetMenu(fileName = "CardRecipe", menuName = "CardGame/CardRecipe")]
[Serializable]
public class CardRecipe : ScriptableObject
{
    public string Name;
    public CardReceiptPart[] EntryCards;
    public CardStock[] CardsResult;

    public float CraftTime;

	public List<CardData> FullEntryCardsList() => EntryCards.SelectMany(receiptPart => Enumerable.Repeat(receiptPart.CardData.CardInfo, receiptPart.CardData.Count)).ToList();
	public List<CardData> FullResultCardsList() => CardsResult.SelectMany(receiptPart => Enumerable.Repeat(receiptPart.CardInfo, receiptPart.Count)).ToList();
}
