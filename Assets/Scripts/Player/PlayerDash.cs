using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("DashSalto")]
    [SerializeField] float forceDashJump;
    [SerializeField] float timeDashJump;
    public bool dashJumpSkill;
    //public bool canDashJump;
    public bool isDashJump;

    //float dashJumpTime = 0.1f;
    //float dashJumpTimeCounter;

    CharacterController characterController;
    PlayerJump playerJumpScript;

    IEnumerator ieDash;

    [SerializeField] TrailRenderer trailRenderer;

    // Start is called before the first frame update
    //void Start()
    //{
    //    characterController = GetComponent<CharacterController>();
    //    playerJumpScript = GetComponent<PlayerJump>();

    //    trailRenderer.emitting = false;

    //    ieDash = DashSalto();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //if (dashJumpTimeCounter > -1) dashJumpTimeCounter -= Time.deltaTime;
    //    //if (playerControlScript.ComprobacionSuelo()) dashJumpTimeCounter = 0;
    //}

    ////Dash
    //public void InputDash(InputAction.CallbackContext callbackContext)
    //{
    //    if (callbackContext.started)
    //    {
    //        if (!characterController.isGrounded && playerJumpScript.isJumping && dashJumpSkill) //&& dashJumpTimeCounter <= 0
    //        {
    //            ieDash = DashSalto();
    //            StartCoroutine(ieDash);
    //        }
    //    }
    //}

    //IEnumerator DashSalto()
    //{
    //    //dashJumpTimeCounter = dashJumpTime;
    //    isDashJump = true;
    //    playerControlScript.rbPlayer.gravityScale = 0f;
    //    playerControlScript.rbPlayer.linearVelocity = new Vector2(playerControlScript.playerDirection * forceDashJump, 0);
    //    trailRenderer.emitting = true;
    //    yield return new WaitForSeconds(timeDashJump);
    //    isDashJump = false;
    //    playerControlScript.rbPlayer.gravityScale = playerControlScript.defaultGravity;
    //    trailRenderer.emitting = false;
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    StopCoroutine(ieDash);
    //    isDashJump = false;
    //    playerControlScript.rbPlayer.gravityScale = playerControlScript.defaultGravity;
    //    trailRenderer.emitting = false;
    //}
}
