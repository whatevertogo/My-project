using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject SettingsPanel;
    public Button button1;

    public void Awake()
    {
        button1 = GetComponent<Button>();
    }

    public void Start()
    {
        button1.onClick.AddListener(() => { SettingsPanel.SetActive(false); });
        SettingsPanel.SetActive(false);
    }

}
