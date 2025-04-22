using CDTU.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    public GameInputController gameInputController;

    public Vector2 MoveInput { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        gameInputController = new GameInputController();
        gameInputController.Player.Enable();        
    }    private void Start()
    {
        gameInputController.Player.PlayerMoveww.performed += OnPlayerMove;
    }

    private void OnPlayerMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

}
