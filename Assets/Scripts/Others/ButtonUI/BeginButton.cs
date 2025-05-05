using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginButton : MonoBehaviour
{
    public Button button;
    public int index;

    public void Start()
    {
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(index);
        });
    }



}
