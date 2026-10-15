using UnityEngine;

public class BlocksController : MonoBehaviour
{
    public int hp = 3;
    public float speed = 4f;

    private float dirX = 1f;

    [Header("Sensores")]
    public Transform sensorIzquierda;
    public Transform sensorDerecha;
    public float radioSensor = 0.1f;
    public LayerMask paredLayer;

    void Update()
    {
        ComprobarPared();

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

    public void TakeHp(int dmg)
    {
        hp -= dmg;
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