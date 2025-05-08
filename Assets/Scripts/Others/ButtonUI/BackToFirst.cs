using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackToFirst : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button1;

    private void Start()
    {
        // 获取按钮组件
        button1 = GetComponent<Button>();
        button1.onClick.AddListener(LoadFirstScene);
    }

    // 切换场景的方法
    public void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    // 鼠标悬停事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 加载并切换按钮图片
        Sprite newSprite = Resources.Load<Sprite>("UI/back_Point");
        if (newSprite != null)
        {
            button1.image.sprite = newSprite;
            Debug.Log("按钮悬停，图片已更换！");
        }
        else
        {
            Debug.LogWarning("找不到图片UI/back_Point");
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // 加载并恢复原始按钮图片
        Sprite normalSprite = Resources.Load<Sprite>("UI/back");
        if (normalSprite != null)
        {
            button1.image.sprite = normalSprite;
            Debug.Log("鼠标离开，恢复按钮图片！");
        }
        else
        {
            Debug.LogWarning("找不到正常图片UI/back");
        }
    }
}