using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour
{
    private float x;
    
    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 4f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask sueloLayer;

    public float DireccionX { get; private set; } = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private PlayerState state;
    private bool enSuelo;

    [Header("Animación")]
    private EstadoPlayer estado;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        state = GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (state.EstaMuerto) return;

        Mover();
        ComprobarSuelo();
        Saltar();
        Animacion(x);
    }

    private void Animacion(float x)
    {
        if(x != 0)
        {
            estado = EstadoPlayer.RUN;
        }
        else if (!enSuelo)
        {
            estado = EstadoPlayer.JUMP;
        }
        else
        {
            estado = EstadoPlayer.IDLE;
        }

        switch (estado)
        {
            case EstadoPlayer.RUN:
                animator.Play("Run");
                break;
            case EstadoPlayer.JUMP:
                animator.Play("Jump");
                break;
            case EstadoPlayer.IDLE:
                animator.Play("Idle");
                break;
            default:
                Debug.Log("La animación no funciona");
                break;
        }
        
    }

    private void Mover()
    {
        x = Input.GetAxisRaw("Horizontal");
        CambiarDireccion(x);

        // Mantiene el mismo movimiento que el script original.
        transform.Translate(Vector3.right * x * speed * Time.deltaTime);
        animator.SetBool("IsRunning", x != 0f);
    }

    private void CambiarDireccion(float x)
    {
        if (x < 0f)
        {
            DireccionX = -1f;
            sr.flipX = true;
        }
        else if (x > 0f)
        {
            DireccionX = 1f;
            sr.flipX = false;
        }
    }

    private void ComprobarSuelo()
    {
        if (groundCheck == null) return;
        enSuelo = Physics2D.OverlapCircle(groundCheck.position, groundRadius, sueloLayer);
    }

    private void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

        animator.SetBool("IsJumping", !enSuelo);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
