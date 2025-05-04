using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GoOnButton : MonoBehaviour
{
    public CanvasGroup[] images; // 三张图（拖进 Inspector）
    public float fadeDuration = 1f;

    private Button button1;
    private int clickTimes = 0;

    private void Start()
    {
        button1 = GetComponent<Button>();
        button1.onClick.AddListener(NextPicture);

        // 初始化透明度为 0
        foreach (var img in images)
        {
            img.alpha = 0f;
        }
    }

    public void NextPicture()
    {
        clickTimes++;

        if (clickTimes <= images.Length)
        {
            images[clickTimes - 1].DOFade(1f, fadeDuration);
        }
        else if (clickTimes == images.Length + 1)
        {
            SceneManager.LoadSceneAsync("FirstLevel"); // 异步加载场景
        }
    }
}