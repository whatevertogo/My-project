using UnityEngine;
using UnityEngine.UI;

public class Back : MonoBehaviour
{

    private Button button;
    [SerializeField] private GameObject SettingsPanel;


    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(() => { SettingsPanel.SetActive(false); });
    }


}
