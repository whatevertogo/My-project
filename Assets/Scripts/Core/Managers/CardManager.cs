using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;

public class CardManager : Singleton<CardManager>
{
    public int cardCount = 10;     // 卡片数量
    public GameObject cardUIPrefab;// 卡片UI预制体
    public Transform CardGrid;//生成卡牌的父物体

    private List<CardData> cards = new List<CardData>();
    private List<CardUI> cardUIs = new List<CardUI>();

    private void Start()
    {
        GenerateCards();
    }

    void GenerateCards()
    {
        for (int i = 0; i < cardCount; i++)
        {
            var newCard = new CardData(i, $"卡牌 {i}", "这是一张描述", CardType.tree, GetRandomSprite());
            cards.Add(newCard);

            var cardUIObj = Instantiate(cardUIPrefab, CardGrid);
            var cardUI = cardUIObj.GetComponent<CardUI>();
            cardUI.Bind(newCard);
            cardUIs.Add(cardUI);
        }
    }

    Sprite GetRandomSprite()
    {
        // 这里可以自己做随机贴图加载
        return null;
    }

    public void RemoveCard(CardData card)
    {
        int index = cards.IndexOf(card);
        if (index >= 0)
        {
            Destroy(cardUIs[index].gameObject);
            cards.RemoveAt(index);
            cardUIs.RemoveAt(index);
        }
    }




}
