using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private EstadoPlayer estadoActual;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ActualizarAnimacion(float x, bool enSuelo, bool atacando)
    {
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

    private void CambiarEstado(EstadoPlayer nuevoEstado)
    {
        if (estadoActual == nuevoEstado)
            return;

        estadoActual = nuevoEstado;

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
        }
    }
}