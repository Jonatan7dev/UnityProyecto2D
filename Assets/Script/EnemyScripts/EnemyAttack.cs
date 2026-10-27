using UnityEngine;

[RequireComponent(typeof(EnemyState))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque por contacto")]
    public int danoContacto = 20;

    private EnemyState state;

    private void Awake()
    {
        state = GetComponent<EnemyState>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state.EstaMuerto || !collision.gameObject.CompareTag("Player")) return;

        PlayerState playerState = collision.gameObject.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.Vida(danoContacto);
        }
    }
}
