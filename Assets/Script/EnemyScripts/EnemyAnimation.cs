using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private string animacionActual;
    private float finAccion;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Mover(bool enMovimiento)
    {
        // Deja terminar el ataque o el golpe antes de volver a caminar.
        if (Time.time < finAccion || animacionActual == "Death") return;

        if (enMovimiento)
            CambiarEstado("Move");
        else
            CambiarEstado("Idle");
    }

    private void CambiarEstado(string nombre)
    {
        if (animacionActual == nombre) return;

        // El murciélago solo tiene Fly y sigue usando su animación por defecto.
        if (!animator.HasState(0, Animator.StringToHash("Base Layer." + nombre))) return;

        animator.Play(nombre, 0, 0f);
        animacionActual = nombre;
    }

    public float Reproducir(EstadoEnemy accion)
    {
        if (animacionActual == "Death") return 0f;

        switch (accion)
        {
            case EstadoEnemy.ATTACK:
                if (animacionActual == "Hit" && Time.time < finAccion) return 0f;
                return ReproducirAccion("Attack");
            case EstadoEnemy.HIT:
                return ReproducirAccion("Hit");
            case EstadoEnemy.DIE:
                return ReproducirAccion("Death");
            default:
                return 0f;
        }
    }

    private float ReproducirAccion(string nombre)
    {
        // Cada enemigo reproduce solo las acciones para las que tiene un clip.
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == nombre)
            {
                animator.Play(nombre, 0, 0f);
                animacionActual = nombre;
                finAccion = Time.time + clip.length;
                return clip.length;
            }
        }
        return 0f;
    }
}
