using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(EnemyState))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 4f;

    [Header("Sensores")]
    public Transform sensorIzquierda;
    public Transform sensorDerecha;
    public float radioSensor = 0.1f;
    public LayerMask paredLayer;

    private float dirX = 1f;
    private SpriteRenderer sr;
    private EnemyState state;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        state = GetComponent<EnemyState>();
    }

    private void Update()
    {
        if (state.EstaMuerto) return;

        ComprobarPared();
        CambiarDir(dirX);
        transform.Translate(Vector2.right * dirX * speed * Time.deltaTime);
    }

    private void ComprobarPared()
    {
        if (sensorDerecha != null &&
            Physics2D.OverlapCircle(sensorDerecha.position, radioSensor, paredLayer))
        {
            dirX = -1f;
        }

        if (sensorIzquierda != null &&
            Physics2D.OverlapCircle(sensorIzquierda.position, radioSensor, paredLayer))
        {
            dirX = 1f;
        }
    }

    public void CambiarDir(float direccion)
    {
        if (direccion < 0f) sr.flipX = true;
        else if (direccion > 0f) sr.flipX = false;
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
