using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("DisparoRayo")]
    public float disRayo = 10f;
    public LayerMask layersActive;

    [Header("Salto")]
    public float fuerzaSalto = 4f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask sueloLayer;

    //Variables privados
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator an;

    private bool enSuelo;
    private bool muerto;

    private float direccionX = 1f;
    private float siguienteDanoLava;

    //Variables para la UI
    [Header("UI")]
    public Text texto;
    

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
        MostrarTexto();
        ComprobarSuelo();
        Saltar();
        Disparar();

        if (Input.GetMouseButtonDown(0))
        {
            AtacarConEspada();
        }

        if (hp <= 0)
        {
            Morir();
        }
        
    }

    private void ComprobarSuelo()
    {
        enSuelo = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            sueloLayer
        );
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
        if (!Input.GetMouseButtonDown(1)) return;

        if (balaPrefab == null)
        {
            Debug.LogWarning("No has asignado el prefab de la bala.");
            return;
        }

        Vector3 posicionBala =
            transform.position + Vector3.right * (distanciaDisparo * direccionX);

        GameObject nuevaBala =
            Instantiate(balaPrefab, posicionBala, Quaternion.identity);

        Bala bala = nuevaBala.GetComponent<Bala>();

        if (bala != null)
        {
            bala.EstablecerDireccion(direccionX);
        }
    }

    public void AtacarConEspada()
    {
        // Activar la animación de ataque
        an.SetTrigger("IsAttacking");

        // Dirección del ataque
        Vector2 direccion = new Vector2(direccionX, 0);

        // Detectar objetos
        RaycastHit2D hit2D = Physics2D.Raycast(
            transform.position,
            direccion,
            disRayo,
            layersActive
        );

        if (hit2D.collider != null)
        {
            Debug.Log("Golpeado: " + hit2D.collider.name);

            if (hit2D.collider.CompareTag("Block"))
            {
                Debug.Log("Es un Block: " + hit2D.transform.name);
            }
            else
            {
                hit2D.collider.gameObject.GetComponent<BlocksController>().TakeHp(10);
            }
        }
        else
        {
            Debug.Log("No golpeó nada");
        }

        Debug.DrawRay(
            transform.position,
            direccion * disRayo,
            Color.green,
            1f
        );
    }

    public void FuncionBoton(int id)
    {
        print("Botón por Script y su botón es " + id);
    }

    private void Morir()
    {
        if (muerto) return;

        muerto = true;

        Debug.Log("El personaje ha muerto.");

        enabled = false;
    }

    private void MostrarTexto()
    {
        texto.text = "HP: " + hp;
    }

    private void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");

        Direccion(x);

        Vector3 direccion = new Vector3(x, 0, 0).normalized;

        transform.Translate(direccion * speed * Time.deltaTime);

        bool moviendo = x != 0;

        an.SetBool("IsRunning", moviendo);
    }

    private void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.linearVelocity =
                new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        an.SetBool("IsJumping", !enSuelo);
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

        MostrarTexto();
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

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundRadius
        );
    }
}