using System;
using UnityEngine;

[Serializable]
public enum CardType
{ 
	NONE,
	ALIVE,
	RESOURCE,
	LOCATION,
	STRUCTURE
}


[CreateAssetMenu(fileName = "CardData", menuName = "CardGame/CardData")]
[Serializable]
public class CardData : ScriptableObject
{
    private static int s_count = 1;

    public int Id;
    public string Name;
    public Sprite CardSprite;

	public CardType CardType;

    public bool CanStack = true;

	private void CreateId()
	{
		Id = s_count++;
	}

	private void OnEnable()
	{
        CreateId();
	}
}
