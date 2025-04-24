using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TogglePause);
    }

    private void TogglePause()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f; // 暂停游戏
        }
        else
        {
            Time.timeScale = 1f; // 恢复游戏
        }
    }
}
