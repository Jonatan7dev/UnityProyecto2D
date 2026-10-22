using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlocksController : MonoBehaviour
{
    public int hp = 30;
    public float speed = 4f;

    private float dirX = 1f;

    private SpriteRenderer sr;

    [Header("Sensores")]
    public Transform sensorIzquierda;
    public Transform sensorDerecha;
    public float radioSensor = 0.1f;
    public LayerMask paredLayer;

    [Header("UI")]
    public Text texto;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        MostrarTexto();
    }
    void Update()
    {
        ComprobarPared();
        CambiarDir(dirX);

        transform.Translate(Vector2.right * dirX * speed * Time.deltaTime);

        DestroyBlock();
    }

    void ComprobarPared()
    {
        bool tocaDerecha = Physics2D.OverlapCircle(
            sensorDerecha.position,
            radioSensor,
            paredLayer
        );

        bool tocaIzquierda = Physics2D.OverlapCircle(
            sensorIzquierda.position,
            radioSensor,
            paredLayer
        );

        if (tocaDerecha)
        {
            dirX = -1;
        }

        if (tocaIzquierda)
        {
            dirX = 1;
        }
    }

    public void CambiarDir(float dirX)
    {
        if(dirX < 0)
        {
            sr.flipX = true;
        } else if(dirX > 0)
        {
            sr.flipX = false;
        }
    }

    public void MostrarTexto()
    {
        texto.text = "Hp: " + hp;
    }

    public void TakeHp(int dmg)
    {
        hp -= dmg;
        MostrarTexto();
    }

    void DestroyBlock()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Movimiento>().Vida(20);
        }
    }

    private void OnDrawGizmos()
    {
        if (sensorDerecha != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(sensorDerecha.position, radioSensor);
        }

        if (sensorIzquierda != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(sensorIzquierda.position, radioSensor);
        }
    }
}