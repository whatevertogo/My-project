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
    }
    private void Update()
    {
        MoveInput = gameInputController.Player.PlayerMoveww.ReadValue<Vector2>();
    }

}
