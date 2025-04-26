using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;

public class CardManager : Singleton<CardManager>
{
    public GameObject cardUIPrefab; // 卡片UI预制体
    public Transform cardGrid;     // 卡片UI的父物体
    public List<CardData> cardDataList; // 从编辑器配置的卡牌数据列表
    public Dictionary<Card, CardUI> cardToUIMap = new Dictionary<Card, CardUI>(); // 运行时卡牌与UI的映射

    private void Start()
    {
        GenerateCards();
    }

    // 生成卡牌
    void GenerateCards()
    {
        foreach (var cardData in cardDataList)
        {
            AddCard(cardData);
        }
    }

    // 添加卡牌
    public void AddCard(CardData cardData)
    {
        // 创建运行时卡牌
        var card = new Card(cardData);

        // 创建并绑定 UI
        var cardUIObj = Instantiate(cardUIPrefab, cardGrid);
        var cardUI = cardUIObj.GetComponent<CardUI>();
        cardUI.Bind(card);

        // 记录映射关系
        cardToUIMap[card] = cardUI;
    }

    // 移除卡牌
    public void RemoveCard(Card card)
    {
        if (cardToUIMap.TryGetValue(card, out CardUI cardUI))
        {
            // 销毁 UI 对象
            Destroy(cardUI.gameObject);

            // 从映射中移除
            cardToUIMap.Remove(card);
        }
    }
}
