using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;

public class CardManager : Singleton<CardManager>
{
    public GameObject cardUIContainerPrefab; // 卡片UI预制体
    public Transform cardGrid;     // 卡片UI的父物体
    public List<CardData> cardDataList; // 从编辑器配置的卡牌数据列表
    public Dictionary<Card, CardUI> cardToUIDic = new(); // 运行时卡牌与UI的映射
    private void Start()
    {
        GenerateCards();
    }

    // todo-测试生成卡牌
    void GenerateCards()
    {
        foreach (var cardData in cardDataList)
        {
            Debug.Log($"Processing CardData: {cardData.cardName}");
            AddCard(cardData);
        }
    }

    // 添加卡牌
    public void AddCard(CardData cardData)
    {
        // 创建运行时卡牌
        var card = new Card(cardData);

        // 创建并绑定 UI
        var cardUIContainerObj = Instantiate(cardUIContainerPrefab, cardGrid);
        CardUI cardUI = cardUIContainerObj.transform.Find("CardUI")?.GetComponent<CardUI>();
        if (cardUI is null)
        {
            Debug.LogError("CardUI component not found in the instantiated prefab. Please check the prefab structure.");
            return;
        }
        cardUI.Bind(card);
        // 记录映射关系
        cardToUIDic[card] = cardUI;
    }

    // 移除卡牌
    public void RemoveCard(Card card)
    {
        if (cardToUIDic.TryGetValue(card, out CardUI cardUI))
        {
            // 销毁 UI 对象
            Destroy(cardUI.CardObject);

            // 从映射中移除
            cardToUIDic.Remove(card);
        }
    }
}
