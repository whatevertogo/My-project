using UnityEngine;
using UnityEngine.UI;

public class OutGame : MonoBehaviour
{

    public Button button1;


    private void Onclick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
