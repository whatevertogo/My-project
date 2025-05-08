using UnityEngine;
using DG.Tweening;


public class BirdSquareHoverHandler : ICellHoverHandler
{
    private readonly DefaultHoverHandler defaultHandler = new DefaultHoverHandler();

    // 当鼠标悬停在某个SquareCell上时调用的方法
    public void OnHoverEnter(SquareCell cell)
    {
        // 输出日志信息，显示当前悬停的格子坐标，并提示显示小鸟特效
        Debug.Log(message: $"鸟格子 {cell.Coordinates} 悬停，显示小鸟特效！");
        // 自定义逻辑
        defaultHandler.OnHoverEnter(cell); // 调用默认逻辑
        cell.chatObject.SetActive(true);
        PlayChatAppearAnimation(cell.chatObject); // 显示对话框
    }

    public void OnHoverExit(SquareCell cell)
    {
        Debug.Log($"鸟格子 {cell.Coordinates} 悬停结束！");
        defaultHandler.OnHoverExit(cell); // 调用默认逻辑
        if (cell.IsCreateChatBox)
        {
            cell.chatObject.SetActive(false); // 隐藏对话框
        }
    }
    private void AnimateChatAppear(GameObject chatObject)
    {
        var spriteRenderer = chatObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        // 初始状态
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // 透明
        chatObject.transform.localScale = Vector3.zero;

        // 动画：淡入 + 放大
        spriteRenderer.DOFade(1f, 0.3f);
        chatObject.transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0.3f).SetEase(Ease.OutBack);
    }
    private void PlayChatAppearAnimation(GameObject chatObject)
    {
        var spriteRenderer = chatObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer is null) return;

        DOTween.Kill(chatObject.transform);
        DOTween.Kill(spriteRenderer);

        // 动画
        spriteRenderer.DOFade(1f, 0.3f).From(0f);
        chatObject.transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0.3f)
            .From(Vector3.zero)
            .SetEase(Ease.OutBack);
    }
}
