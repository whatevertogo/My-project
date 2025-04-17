using UnityEngine;
using UnityEngine.UI;

public class OutGame : MonoBehaviour
{

    private Button button1;

    public void Awake()
    {
        button1 = GetComponent<Button>();
    }
    public void Start()
    {
        button1.onClick.AddListener(Onclick);
    }

    private void Onclick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
