using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    PlayerInputMap playerInputMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        playerInputMap = new PlayerInputMap();
    }

    private void OnEnable()
    {
        playerInputMap.Enable();
    }

    private void OnDisable()
    {
        playerInputMap.Disable();
    }

    //Player Input Getters
    public Vector2 GetplayerMovement()
    {
        return playerInputMap.Player.Movement.ReadValue<Vector2>();
    }

    public bool GetJumpPressed()
    {
        return playerInputMap.Player.Jump.WasPressedThisFrame();
    }
}
