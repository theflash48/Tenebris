using UnityEngine;
using UnityEngine.InputSystem;

public class InputsPlayer : MonoBehaviour
{
    PlayerJump playerJump;

    [Header("Referencias")]
    private PlayerInput playerInput;

    [Header("Variables")]
    public float Input_x;
    public bool derecha = true;
    public bool cambioSpline;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerJump = GetComponent<PlayerJump>();
    }

    //Llamada evento input mover Player
    public void Mover(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            Input_x = Mathf.RoundToInt(playerInput.actions["Mover"].ReadValue<float>());

            //Lectura de imput del movimiento
            if (Input_x != 0)
            {
                if (Input_x < 0)
                {
                    derecha = false;
                } else
                if (Input_x > 0)
                {
                    derecha = true;
                }
            }
        }
        else
            if (callbackContext.canceled)
        {
            Input_x = 0f;
        }


        //    if (!_GameManager.IsGameOver) Input_x = Mathf.RoundToInt(_PlayerInput.actions["Mover"].ReadValue<float>());

        //    if (Input_x != 0)
        //    {
        //        _Animator.SetBool("Move", true);

        //        if (Input_x < 0)
        //        {
        //            _Renderer.flipX = true;
        //        }

        //        if (Input_x > 0)
        //        {
        //            _Renderer.flipX = false;
        //        }
        //    }
        //    else _Animator.SetBool("Move", false);
        //}
        //else
        //if (callbackContext.canceled)
        //{
        //    Input_x = 0f;
        //    _Animator.SetBool("Move", false);
        //}
    }

    //Llamada evento input correr Player
    public void Correr(InputAction.CallbackContext callbackContext)
    {
        //switch (callbackContext.phase)
        //{
        //    case InputActionPhase.Started:
        //        _MovPlayer.MultiplicadorAlCorrer = 2f;
        //        _MovPlayer.CalcularVelocidad();
        //        break;
        //    case InputActionPhase.Canceled:
        //        _MovPlayer.MultiplicadorAlCorrer = 1f;
        //        _MovPlayer.CalcularVelocidad();
        //        break;
        //}
    }

    //Llamada evento input saltar Player
    public void Saltar(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            playerJump.jumpBufferCounter = playerJump.jumpBufferTime;
            playerJump.jumpCancel = false;
        }
        else
        {
            if (callbackContext.canceled)
            {
                playerJump.jumpCancel = true;
                playerJump.coyoteTimeCounter = 0f;
            }
        }
    }

    //Llamada evento input atacar Player
    public void Atacar(InputAction.CallbackContext callbackContext)
    {

    }

    //Llamada evento input cambio spline
    public void CambioSpline(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && !cambioSpline)
        {
            cambioSpline = true;
        }
        else
        {
            if (callbackContext.canceled)
            {
                cambioSpline = false;
            }
        }
    }
}
