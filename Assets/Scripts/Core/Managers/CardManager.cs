using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;

public class CardManager : Singleton<CardManager>
{
    public int cardCount = 10;     // 卡片数量
    public GameObject cardUIPrefab;// 卡片UI预制体
    public Transform CardGrid;//生成卡牌的父物体

    public Dictionary<CardData, CardUI> cardData_UIDic = new();

    private void Start()
    {
        GenerateCards();
    }

    void GenerateCards()
    {
        for (int i = 0; i < cardCount; i++)
        {
            var newCard = new CardData(i, $"卡牌 {i}", "这是一张描述", CardType.tree);

            var cardUIObj = Instantiate(cardUIPrefab, CardGrid);
            var cardUI = cardUIObj.GetComponent<CardUI>();
            cardUI.Bind(newCard);//绑定sprite
            cardData_UIDic[newCard] = cardUI;
        }
    }

    public void RemoveCard(CardData card)
    {
        if (cardData_UIDic.TryGetValue(card, out CardUI cardUI))
        {
            Destroy(cardUI.gameObject);
            cardData_UIDic.Remove(card);
        }
    }

    public void AddCard(CardData card)
    {
        var cardUIObj = Instantiate(cardUIPrefab, CardGrid);

    }




}
