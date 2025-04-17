using UnityEngine;
using UnityEngine.UI;

public class OutGame : MonoBehaviour
{

    public Button button1;

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
