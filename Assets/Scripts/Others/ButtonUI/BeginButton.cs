using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginButton : MonoBehaviour
{

    public Button button1;
    public string NextScenceName;

    public void Start()
    {
        button1.onClick.AddListener(OnClick);
    }


    private void OnClick()
    {
        SceneManager.LoadSceneAsync(NextScenceName);
    }

}
