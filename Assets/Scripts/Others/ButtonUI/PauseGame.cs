using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    private Button button;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TogglePause);
    }

    private void TogglePause()
    {
        if (Time.timeScale == 1f)
        {
            textMeshProUGUI.text = "继续游戏"; // 更新文本为“继续游戏”
            Time.timeScale = 0f; // 暂停游戏
        }
        else
        {
            textMeshProUGUI.text = "暂停游戏"; // 更新文本为“暂停游戏”
            Time.timeScale = 1f; // 恢复游戏
        }
    }
}
