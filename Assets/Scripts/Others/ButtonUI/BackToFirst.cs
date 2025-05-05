using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToFirst : MonoBehaviour
{
    private Button button1;

    private void Start()
    {
        button1 = GetComponent<Button>();
        button1.onClick.AddListener(LoadFirstScene);
    }

    public void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }
}
