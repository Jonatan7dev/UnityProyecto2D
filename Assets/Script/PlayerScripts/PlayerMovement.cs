using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour
{
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
    }

    private void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");
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
