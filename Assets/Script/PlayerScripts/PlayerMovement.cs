using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour
{
    public float x;
    
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
    public Animator animator;
    private PlayerState state;
    public bool enSuelo;

    [Header("Animación")]
    private PlayerAnimation anim;
    private bool atacando;
    private bool saltoSolicitado;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        state = GetComponent<PlayerState>();
        anim = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (state.EstaMuerto) return;

        x = Input.GetAxisRaw("Horizontal");
        CambiarDireccion(x);

        if (Input.GetKeyDown(KeyCode.Space))
            saltoSolicitado = true;

        if (Input.GetMouseButtonDown(0) && !atacando)
        {
            StartCoroutine(Atacar());
        }

        anim.ActualizarAnimacion(x, enSuelo, atacando);
    }

    private void FixedUpdate()
    {
        if (state.EstaMuerto)
        {
            saltoSolicitado = false;
            rb.linearVelocityX = 0f;
            return;
        }

        ComprobarSuelo();

        Vector2 velocidad = rb.linearVelocity;
        velocidad.x = x * speed;

        if (saltoSolicitado && enSuelo)
        {
            velocidad.y = fuerzaSalto;
            enSuelo = false;
        }

        saltoSolicitado = false;
        rb.linearVelocity = velocidad;
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
        enSuelo = groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundRadius, sueloLayer) != null;
    }

    private IEnumerator Atacar()
    {
        atacando = true;

        yield return new WaitForSeconds(0.5f);

        atacando = false;
    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
