
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(EnemyState), typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private enum Estado
    {
        Patrulla,
        Persecucion,
        Regreso
    }

    [Header("Patrulla")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidadPatrulla = 2f;

    [Header("Persecución")]
    public float velocidadPersecucion = 3.5f;
    public float radioVision = 5f;
    public float tiempoPerderVision = 2f;
    public float retrasoCambioObjetivo = 0.25f;

    [Header("Recuperación")]
    public Transform puntoReaparicion;
    public float tiempoMaximoRegreso = 5f;

    [Header("Tolerancia de llegada")]
    public float toleranciaHorizontal = 0.05f;
    public float toleranciaVertical = 0.5f;

    [Header("Detección")]
    public Transform puntoVision;
    public LayerMask paredLayer;

    private Rigidbody2D cuerpo;
    private SpriteRenderer sprite;
    private EnemyState vida;
    private Transform jugador;
    private Transform punto;
    private Estado estado;
    private float ultimaVision;
    private float actualizarObjetivo;
    private float destinoJugador;
    private float limiteRegreso;
    private float xAnterior;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        vida = GetComponent<EnemyState>();
    }

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!puntoA || !puntoB || !puntoReaparicion)
        {
            Debug.LogWarning("Faltan puntos de patrulla o reaparición.", this);
            enabled = false;
            return;
        }
        punto = PuntoMasCercano();
        xAnterior = cuerpo.position.x;
    }

    private void FixedUpdate()
    {
        if (vida.EstaMuerto) return;
        float ahora = Time.fixedTime;
        if (VeJugador())
        {
            if (estado != Estado.Persecucion || ahora >= actualizarObjetivo)
            {
                destinoJugador = jugador.position.x;
                actualizarObjetivo = ahora + retrasoCambioObjetivo;
            }
            estado = Estado.Persecucion;
            ultimaVision = ahora;
        }
        else if (estado == Estado.Persecucion && (!jugador || ahora - ultimaVision >= tiempoPerderVision))
        {
            IniciarRegreso(ahora);
        }

        if (estado == Estado.Persecucion)
            MoverHacia(destinoJugador, velocidadPersecucion);
        else
            SeguirRuta(ahora);
    }

    private void SeguirRuta(float ahora)
    {
        Vector2 distancia = (Vector2)punto.position - cuerpo.position;
        bool mismaAltura = Mathf.Abs(distancia.y) <= toleranciaVertical;
        if (mismaAltura && Mathf.Abs(distancia.x) <= toleranciaHorizontal)
        {
            punto = punto == puntoA ? puntoB : puntoA;
            estado = Estado.Patrulla;
        }
        else if (estado == Estado.Patrulla && !mismaAltura)
            IniciarRegreso(ahora);
        else if (estado == Estado.Regreso && ahora >= limiteRegreso)
        {
            cuerpo.position = puntoReaparicion.position;
            cuerpo.linearVelocity = Vector2.zero;
            xAnterior = cuerpo.position.x;
            punto = PuntoMasCercano();
            estado = Estado.Patrulla;
        }
        MoverHacia(punto.position.x, velocidadPatrulla);
    }

    private void IniciarRegreso(float ahora)
    {
        estado = Estado.Regreso;
        limiteRegreso = ahora + tiempoMaximoRegreso;
        punto = PuntoMasCercano();
    }

    private Transform PuntoMasCercano()
    {
        Vector2 posicion = cuerpo.position;
        return ((Vector2)puntoA.position - posicion).sqrMagnitude <
               ((Vector2)puntoB.position - posicion).sqrMagnitude ? puntoA : puntoB;
    }

    private void MoverHacia(float destino, float velocidad)
    {
        float x = cuerpo.position.x;
        float paso = Mathf.MoveTowards(x, destino, velocidad * Time.fixedDeltaTime) - x;
        cuerpo.linearVelocityX = paso / Time.fixedDeltaTime;
        if (Mathf.Abs(paso) > 0.0001f) sprite.flipX = paso < 0f;
        vida.Animacion?.Mover(Mathf.Abs(x - xAnterior) > 0.0001f);
        xAnterior = x;
    }

    private Vector2 OrigenVision => puntoVision ? puntoVision.position : transform.position;

    private bool VeJugador()
    {
        if (!jugador) return false;
        Vector2 origen = OrigenVision;
        Vector2 destino = jugador.position;
        return (destino - origen).sqrMagnitude <= radioVision * radioVision &&
               !Physics2D.Linecast(origen, destino, paredLayer).collider;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(OrigenVision, radioVision);
        Gizmos.color = Color.green;
        if (puntoA && puntoB) Gizmos.DrawLine(puntoA.position, puntoB.position);
        Gizmos.color = Color.red;
        if (puntoReaparicion) Gizmos.DrawWireSphere(puntoReaparicion.position, 0.25f);
    }
}
