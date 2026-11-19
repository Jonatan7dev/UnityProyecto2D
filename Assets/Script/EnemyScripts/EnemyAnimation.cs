using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    private Animator anim;

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ActualizarAnimacion(float x, bool atacando)
    {
        EstadoEnemy nuevoEstado;
        if (atacando)
        {
            nuevoEstado = EstadoEnemy.ATTACK;
        }
        
        else if (x != 0)
        {
            nuevoEstado = EstadoEnemy.WALK;
        }
        else
        {
            nuevoEstado = EstadoEnemy.IDLE;
        }

        CambiarEstado(nuevoEstado);
    }

    private void CambiarEstado(EstadoEnemy estado)
    {
        switch (estado)
        {
            case EstadoEnemy.WALK:
                anim.Play("Walk");
                break;

            case EstadoEnemy.IDLE:
                anim.Play("Idle");
                break;

            case EstadoEnemy.ATTACK:
                anim.Play("Attack");
                break;
        }
    }

}
