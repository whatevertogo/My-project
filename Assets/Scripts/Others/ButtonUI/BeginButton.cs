using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginButton : MonoBehaviour
{
    private Button button;
    public string NextScenceName;

    public void Awake()
    {
        button = GetComponent<Button>();
    }
    public void Start()
    {
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(NextScenceName);
        });
    }



}
