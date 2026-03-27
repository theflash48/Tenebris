using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    //PlayerDash playerDashScript;
    CharacterController characterController;

    [SerializeField] float gravity;   // negativa
    [SerializeField] float jumpHeight;  // altura que quieres alcanzar
    public Vector3 velocity; // solo usaremos "y" para salto/gravedad

    float coyoteTime = 0.1f;
    public float coyoteTimeCounter;
    public float jumpBufferTime = 0.1f;
    public float jumpBufferCounter;
    int jumpsMax = 2;
    int jumpsCount;
    public bool isJumping;
    public bool jumpCancel;
    bool isGrounded;

    [Header("Ajustes ray techo")]
    [SerializeField] Vector3 boxSize; //Tamaño de la caja del raycast 
    [SerializeField] float boxOffsetY;   // Cuánto bajo los pies
    [SerializeField] LayerMask layerMask;  // Capa que cuenta como suelo
    bool isCeiling;

    // Start is called before the first frame update
    void Start()
    {
        //playerDashScript = GetComponent<PlayerDash>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = characterController.isGrounded;

        // Aplicar gravedad acumulada
        if (velocity.y > -80f) velocity.y += gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0)
        {
            coyoteTimeCounter = coyoteTime;
            jumpsCount = jumpsMax;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Si realizamos un Dash reinicia los saltos hasta que toquemos suelo
        //if (playerDashScript.isDashJump)
        //{
        //    jumpsCount = jumpsMax;
        //    isJumping = false;
        //}

        //Animacion salto
        if (velocity.y > 0f)
        {
            //playerControlScript.animPLayer.SetFloat("velY", 1f);
            //playerControlScript.animPLayer.SetBool("isJump", true);
        }
        else 
        if (velocity.y < 0f) //&& !playerControlScript.ComprobacionSuelo()
        {
            //playerControlScript.animPLayer.SetFloat("velY", -1f);
            //playerControlScript.animPLayer.SetBool("isJump", false);
        }else
        {
            //playerControlScript.animPLayer.SetBool("isJump", false);
            //playerControlScript.animPLayer.SetFloat("velY", 0f);
        }

        if (jumpBufferCounter > 0) //El buffer del salto controla si se salta o no
        {
            if (!isJumping && isGrounded) //Primer salto desde el suelo
            {
                Salto();
            }
            else
            if (!isJumping && !isGrounded) //Salto desde el precipicio
            {
                if (coyoteTimeCounter > 0f)
                {
                    Salto();
                }
            }
            else
            if (isJumping && jumpsCount > 0)//Saltos a partir del primero //&& (!playerControl.ComprobacionSuelo() || !playerControl.isColliderTopLadder())//
            {
                Salto();
            }
        }

        //Cancela el salto al soltar la tecla o boton
        if (jumpCancel && !isGrounded)
        {
            if (velocity.y > 0 && (jumpBufferCounter + 0.1f) < 0)
            {
                velocity.y = 0f;
                jumpCancel = false;
            }
        }

        // Mantener pegado al suelo
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -5.8f;
        }

        if (jumpBufferCounter > -1) jumpBufferCounter -= Time.deltaTime; //Comprobacion de seguridad para no desbordar la variable

        // Chequea colison con el techo
        if (isJumping) CheckTecho();

        if (isCeiling && velocity.y > 0)
        {
            velocity.y = 0f;
            jumpCancel = false;
        }
    }

    void Salto()
    {
        //if (!playerDashScript.isDashJump)
        //{
            isJumping = true;
            jumpsCount--;
            velocity.y = 0f;

            // v = sqrt(2 * h * -g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpBufferCounter = 0;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && velocity.y > 0f)
        {
            isJumping = false;
            velocity.y = 0f;
        }
    }

    void CheckTecho()
    {
        Vector3 boxCenter = transform.position + Vector3.down * boxOffsetY;

        // CheckBox: caja pequeña bajo el jugador
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize * 0.5f, Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore);

        //Es true si hay colision
        isCeiling = hits.Length > 0;
    }

    //Dibujamos los guizmos para ver el raycast desde la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isCeiling ? Color.green : Color.red;
        Vector3 boxCenter = transform.position + Vector3.down * boxOffsetY;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
