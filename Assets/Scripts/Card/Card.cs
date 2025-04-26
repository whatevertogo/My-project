using System;
using UnityEngine;

[Serializable]
public class CardData
{
    public int id;
    public string cardName;
    public string description;
    public CardType cardType;
    public Sprite cardSprite;

    public CardData(int id, string name, string desc, CardType type, Sprite sprite)
    {
        this.id = id;
        this.cardName = name;
        this.description = desc;
        this.cardType = type;
        this.cardSprite = sprite;
    }
}