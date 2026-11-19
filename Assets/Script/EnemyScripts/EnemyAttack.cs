using UnityEngine;

[RequireComponent(typeof(EnemyState))]
public class EnemyAttack : MonoBehaviour
{
    public int danoContacto = 20;
    private EnemyState estado;

    private void Awake() => estado = GetComponent<EnemyState>();

    private void OnCollisionEnter2D(Collision2D choque)
    {
        if (estado.EstaMuerto || !choque.gameObject.CompareTag("Player")) return;
        if (!choque.gameObject.TryGetComponent(out PlayerState jugador) || jugador.EstaMuerto) return;

        estado.Animacion?.Reproducir(EstadoEnemy.ATTACK);
        jugador.Vida(danoContacto);
    }
}
