using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public void Start()
    {
        Vector3 middleGrid = GridManager.Instance.GetGridCenter();
        mainCamera.gameObject.transform.position = middleGrid + Vector3.forward * -7f;
    }

    private void Update()
    {
        transform.position = new Vector3(Player.Instance.transform.position.x, Player.Instance.transform.position.y-7f, -10f);
    }


}
