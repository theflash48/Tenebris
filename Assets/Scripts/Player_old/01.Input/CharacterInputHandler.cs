using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class CharacterInputHandler : MonoBehaviour
{
    [Header("Basic Settings")]
    public Vector2 Move { get; set; }
    public Vector2 Look { get; set; }
    public bool Jump { get; set; }
    public bool CrouchHeld { get; set; }
    
    // --- PlayerInput ---
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Move = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        Look = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        Jump = ctx.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        CrouchHeld = ctx.ReadValueAsButton();
    }

    public void OnFastAttack(InputAction.CallbackContext ctx)
    {
        
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        
    }

    public void OnSelectEnemy(InputAction.CallbackContext ctx)
    {
        
    }
}
