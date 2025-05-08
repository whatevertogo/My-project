using TMPro;
using UnityEngine;

public class RemainTimeText : MonoBehaviour
{
    private float remainTime => GameManager.Instance.timeLimit - GameManager.Instance.time;

    private TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        if (textMeshProUGUI == null)
        {
            Debug.LogError("请在Inspector中设置TextMeshProUGUI组件！", this.gameObject);
            enabled = false;
            return;
        }
    }
    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = $"剩余时间:{remainTime}秒";
    }
}
