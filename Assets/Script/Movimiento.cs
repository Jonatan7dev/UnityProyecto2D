using UnityEngine;
using UnityEngine.SceneManagement;

public class Movimiento : MonoBehaviour
{
    [Header("Vida")]
    public int hp = 100;
    public int DmgPincho = 1;
    public int DmgLava = 1;

    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Disparo")]
    public GameObject balaPrefab;
    public float distanciaDisparo = 1f;

    [Header("Salto")]
    public float fuerzaSalto = 4f;
    private Rigidbody2D rb;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask sueloLayer;
    private bool enSuelo;

    private float siguienteDanoLava;
    private bool muerto;

    private float direccionX = 1f;
    private SpriteRenderer sr;
    private Animator an;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        an = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (muerto) return;

        Mover();
        ComprobarSuelo();
        Saltar();
        Disparar();

        if (hp <= 0)
        {
            Morir();
        }
    }

    private void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");
        //float y = Input.GetAxisRaw("Vertical");

        Direccion(x);

        Vector3 direccion = new Vector3(x, 0, 0).normalized;
        transform.Translate(direccion * speed * Time.deltaTime);

        bool moviendo = x != 0;
        an.SetBool("IsRunning", moviendo);
    }

    private void Direccion(float x)
    {
        if (x < 0)
        {
            direccionX = -1f;
            sr.flipX = true;
        }
        else if (x > 0)
        {
            direccionX = 1f;
            sr.flipX = false;
        }
    }

    private void Disparar()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (balaPrefab == null)
        {
            Debug.LogWarning("No has asignado el prefab de la bala.");
            return;
        }

        Vector3 posicionBala =
            transform.position + Vector3.right * (distanciaDisparo * direccionX);

        GameObject nuevaBala = Instantiate(
            balaPrefab, posicionBala, Quaternion.identity
        );

        Bala bala = nuevaBala.GetComponent<Bala>();

        if (bala != null)
        {
            bala.EstablecerDireccion(direccionX);
        }
    }

    private void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        an.SetBool("IsJumping", !enSuelo);
    }

    private void ComprobarSuelo()
    {
        enSuelo = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            sueloLayer
        );
    }

    public void Vida(int dano)
    {
        if (muerto) return;

        hp = Mathf.Max(0, hp - dano);
        Debug.Log("Vida restante: " + hp);

        if (hp <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (muerto) return;

        muerto = true;
        Debug.Log("El personaje ha muerto.");
        enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pincho"))
        {
            Vida(DmgPincho);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            if (Time.time >= siguienteDanoLava)
            {
                Vida(DmgLava);
                siguienteDanoLava = Time.time + 1f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bandera"))
        {
            SceneManager.LoadScene("Scene_one");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

}

