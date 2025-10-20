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

    
}
