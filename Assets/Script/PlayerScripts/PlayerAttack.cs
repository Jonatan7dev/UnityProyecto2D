using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerState), typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject balaPrefab;
    public float distanciaDisparo = 1f;

    [Header("DisparoRayo")]
    public float disRayo = 10f;
    public LayerMask layersActive;

    private PlayerMovement movement;
    private PlayerState state;
    private Animator animator;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        state = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (state.EstaMuerto) return;

        if (Input.GetMouseButtonDown(1)) Disparar();
        if (Input.GetMouseButtonDown(0)) DisparoRayo();
    }

    private void Disparar()
    {
        if (balaPrefab == null)
        {
            Debug.LogWarning("No has asignado el prefab de la bala.");
            return;
        }

        Vector3 posicionBala = transform.position +
            Vector3.right * (distanciaDisparo * movement.DireccionX);

        GameObject nuevaBala = Instantiate(balaPrefab, posicionBala, Quaternion.identity);
        Bala bala = nuevaBala.GetComponent<Bala>();
        if (bala != null) bala.EstablecerDireccion(movement.DireccionX);
    }

    public void DisparoRayo()
    {
        Vector2 direccion = new Vector2(movement.DireccionX, 0f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, disRayo, layersActive);
        bool attacking = hit.collider != null;

        if (attacking)
        {
            Debug.Log("Golpeado: " + hit.collider.name);
            if (hit.collider.CompareTag("Block"))
            {
                Debug.Log("Es un Block: " + hit.transform.name);
            }
            else 
            {
                hit.collider.gameObject.GetComponent<EnemyState>().TakeHp(10); 
            }
            
        }
        else
        {
            Debug.Log("No golpeó nada");
        }

        Debug.DrawRay(transform.position, direccion * disRayo, Color.green, 1f);

        // Conserva el Bool y la lógica del Animator que utilizas actualmente.
        animator.SetTrigger("IsAttacking");
    }
}
