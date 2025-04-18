using System;
using CDTU.Utils;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    public PlayerController PlayerController;

    public Vector2 MoveInput { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        PlayerController.Enable();
    }

    private void Start()
    {
        PlayerController.Player.PlayerMove.performed += OnPlayerMove;
    }

    private void OnPlayerMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

    }

}
