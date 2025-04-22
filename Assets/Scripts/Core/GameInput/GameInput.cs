using CDTU.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    public PlayerController PlayerController;

    public Vector2 MoveInput { get; private set; }
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if(PlayerController.Player.PlayerMove is not null)
        {
            PlayerController.Player.PlayerMove.Enable();
        }
        else
        {
            Debug.LogError("PlayerMove action is null. Please check the Input Action setup.");
        }
        PlayerController.Player.PlayerMove.performed += OnPlayerMove;
    }

    private void OnPlayerMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

}
