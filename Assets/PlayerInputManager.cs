using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        playerMovement.UpdateMovement(movement.x);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started) playerMovement.JumpPlayer();
    }
}
