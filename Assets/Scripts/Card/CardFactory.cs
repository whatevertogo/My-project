
using UnityEngine;

public class CardFactory
{

    private GameObject cardPrefab;
    private Transform parentTransform;

    public CardFactory(GameObject prefab, Transform parent)
    {
        cardPrefab = prefab;
        parentTransform = parent;
    }


    /// <summary>
    /// 创建一张卡片
    /// </summary>
    /// <param name="cardText">卡片上的文本</param>
    /// <returns>生成的卡片对象</returns>
    /// //todo-完善
    public Card CreateCard(string cardText)
    {
        // 实例化卡片预制件
        GameObject cardObject = Object.Instantiate(cardPrefab, parentTransform);

        // 获取 Card 脚本组件
        Card card = cardObject.GetComponent<Card>();
        if (card == null)
        {
            Debug.LogError("Card prefab is missing the Card component!");
            return null;
        }

        // 初始化卡片内容
        card.UpdateCardText(cardText);

        return card;
    }



}