using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(EnemyState))]
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

    private Transform jugador;
    private Transform objetivoPatrulla;

    private SpriteRenderer sr;
    private EnemyState state;

    private Estado estado = Estado.Patrulla;

    private float tiempoSinVer;
    private float temporizadorObjetivo;
    private float tiempoRegreso;
    private float objetivoJugador;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        state = GetComponent<EnemyState>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            jugador = player.transform;

        if (puntoA == null || puntoB == null || puntoReaparicion == null)
        {
            Debug.LogWarning(
                "Faltan puntos de patrulla o reaparición.",
                this
            );

            enabled = false;
            return;
        }

        ElegirPuntoPatrullaMasCercano();
    }

    private void Update()
    {
        if (state.EstaMuerto)
            return;

        bool veJugador = PuedeVerJugador();

        if (veJugador)
        {
            if (estado != Estado.Persecucion)
            {
                objetivoJugador = jugador.position.x;
                temporizadorObjetivo = 0f;
                tiempoRegreso = 0f;
            }

            estado = Estado.Persecucion;
            tiempoSinVer = 0f;
        }
        else if (estado == Estado.Persecucion)
        {
            tiempoSinVer += Time.deltaTime;

            if (jugador == null || tiempoSinVer >= tiempoPerderVision)
                IniciarRegreso();
        }

        switch (estado)
        {
            case Estado.Patrulla:
                MoverHacia(
                    objetivoPatrulla.position.x,
                    velocidadPatrulla
                );

                if (HaLlegadoAlPunto(objetivoPatrulla))
                {
                    CambiarPuntoPatrulla();
                }
                else if (!EstaALaAlturaDelPunto(objetivoPatrulla))
                {
                    // Si ha caído durante la patrulla,
                    // también intentará regresar.
                    IniciarRegreso();
                }

                break;

            case Estado.Persecucion:
                temporizadorObjetivo += Time.deltaTime;

                if (veJugador &&
                    temporizadorObjetivo >= retrasoCambioObjetivo)
                {
                    objetivoJugador = jugador.position.x;
                    temporizadorObjetivo = 0f;
                }

                // Si deja de verlo, se dirige a la última
                // posición conocida durante el tiempo de espera.
                MoverHacia(
                    objetivoJugador,
                    velocidadPersecucion
                );

                break;

            case Estado.Regreso:
                tiempoRegreso += Time.deltaTime;

                MoverHacia(
                    objetivoPatrulla.position.x,
                    velocidadPatrulla
                );

                // Coincidir solo en X no significa haber regresado.
                if (HaLlegadoAlPunto(objetivoPatrulla))
                {
                    CambiarPuntoPatrulla();

                    estado = Estado.Patrulla;
                    tiempoRegreso = 0f;
                }
                else if (tiempoRegreso >= tiempoMaximoRegreso)
                {
                    Reaparecer();
                }

                break;
        }
    }

    private void IniciarRegreso()
    {
        estado = Estado.Regreso;
        tiempoRegreso = 0f;
        tiempoSinVer = 0f;

        ElegirPuntoPatrullaMasCercano();
    }

    private void Reaparecer()
    {
        transform.position = puntoReaparicion.position;

        ElegirPuntoPatrullaMasCercano();

        tiempoRegreso = 0f;
        tiempoSinVer = 0f;
        temporizadorObjetivo = 0f;

        estado = Estado.Patrulla;
    }

    private void ElegirPuntoPatrullaMasCercano()
    {
        float distanciaA = Vector2.Distance(
            transform.position,
            puntoA.position
        );

        float distanciaB = Vector2.Distance(
            transform.position,
            puntoB.position
        );

        objetivoPatrulla = distanciaA < distanciaB
            ? puntoA
            : puntoB;
    }

    private void CambiarPuntoPatrulla()
    {
        objetivoPatrulla = objetivoPatrulla == puntoA
            ? puntoB
            : puntoA;
    }

    private bool EstaALaAlturaDelPunto(Transform punto)
    {
        return Mathf.Abs(
            transform.position.y - punto.position.y
        ) <= toleranciaVertical;
    }

    private bool HaLlegadoAlPunto(Transform punto)
    {
        bool cercaEnX = Mathf.Abs(
            transform.position.x - punto.position.x
        ) <= toleranciaHorizontal;

        return cercaEnX && EstaALaAlturaDelPunto(punto);
    }

    private void MoverHacia(float destino, float velocidad)
    {
        float diferencia = destino - transform.position.x;

        if (Mathf.Abs(diferencia) > 0.01f)
            sr.flipX = diferencia < 0f;

        Vector3 posicionDestino = new Vector3(
            destino,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            posicionDestino,
            velocidad * Time.deltaTime
        );
    }

    private bool PuedeVerJugador()
    {
        if (jugador == null)
            return false;

        Vector2 origen = puntoVision != null
            ? (Vector2)puntoVision.position
            : (Vector2)transform.position;

        Vector2 direccion = (Vector2)jugador.position - origen;

        if (direccion.magnitude > radioVision)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(
            origen,
            jugador.position,
            paredLayer
        );

        return hit.collider == null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origen = puntoVision != null
            ? puntoVision.position
            : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origen, radioVision);

        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(puntoA.position, puntoB.position);
        }

        if (puntoReaparicion != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                puntoReaparicion.position,
                0.25f
            );
        }
    }
}