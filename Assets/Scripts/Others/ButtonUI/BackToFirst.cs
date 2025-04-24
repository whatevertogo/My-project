using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToFirst : MonoBehaviour
{

    public static readonly string SceneName = "开始界面";

    public void LoadFirstScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
