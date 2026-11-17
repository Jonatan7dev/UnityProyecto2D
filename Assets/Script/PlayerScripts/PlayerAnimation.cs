
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private EstadoPlayer estadoActual;

    [Header("Animación de daño")]
    [SerializeField] private float duracionGolpe = 0.4f;

    private float finAnimacionGolpe = -1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ActualizarAnimacion(
        float x, bool enSuelo, bool atacando)
    {
        // Mientras recibe daño, no cambiar la animación.
        if (Time.time < finAnimacionGolpe)
            return;

        EstadoPlayer nuevoEstado;

        if (atacando)
        {
            nuevoEstado = EstadoPlayer.ATTACK;
        }
        else if (!enSuelo)
        {
            nuevoEstado = EstadoPlayer.JUMP;
        }
        else if (x != 0)
        {
            nuevoEstado = EstadoPlayer.RUN;
        }
        else
        {
            nuevoEstado = EstadoPlayer.IDLE;
        }

        CambiarEstado(nuevoEstado);
    }

    public void RecibirGolpe()
    {
        finAnimacionGolpe = Time.time + duracionGolpe;

        CambiarEstado(EstadoPlayer.TAKE_HIT, true);
    }

    public void AnimDead()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        CambiarEstado(EstadoPlayer.DEAD);
    }

    private void CambiarEstado(EstadoPlayer nuevoEstado, bool reiniciar = false)
    {
        // Si el jugador está muerto, no permitir cambiar de estado.
        if (estadoActual == EstadoPlayer.DEAD)
            return;

        // Evitar repetir la misma animación, excepto si queremos reiniciarla.
        if (estadoActual == nuevoEstado && !reiniciar)
            return;

        // Actualizar el estado actual.
        estadoActual = nuevoEstado;

        // Reproducir la animación correspondiente.
        switch (estadoActual)
        {
            case EstadoPlayer.IDLE:
                animator.Play("Idle");
                break;

            case EstadoPlayer.RUN:
                animator.Play("Run");
                break;

            case EstadoPlayer.JUMP:
                animator.Play("Jump");
                break;

            case EstadoPlayer.ATTACK:
                animator.Play("Attack");
                break;

            case EstadoPlayer.TAKE_HIT:
                animator.Play("DamageTake", 0, 0f);
                break;

            case EstadoPlayer.DEAD:
                animator.Play("Death", 0, 0f);
                break;
        }
    }
}
